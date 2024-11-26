using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using Pathfinding;

public enum StateType
{
    Idle,
    Partol,
    React,
    Chase,
    Attack,
    //Hurt,
    Dead,
    ExcuteDead,
    Down,
    Confused,
    Reach
}
[Serializable]
public class AIParamater
{
    public int MAX_HP = 2;
    public int HP;
    public float speedCoff = 1.0f;
    public bool canBodyExplode = true;
    public float BlockOffset;
    public float partolSpeed;
    public float chaseSpeed;
    public float turnSpeed;
    //public Transform target;
    public float arriveDistance;
    public float maxChaseDistance;
    public float chaseViewAngle;//2xchaseViewAngle = trueAngleBasedOnY -degree
    public float idleTime;
    [HideInInspector]
    public bool isDown;
    [HideInInspector]
    public float downTime;
    [HideInInspector]
    public Collider2D bodyCollinder;
    public bool reachedEndOfPath = false;
    public ABPath chasePath;
    public int chaseIndex;
    public ABPath[] partolPath;
    [HideInInspector]
    public Animator animator;
    [HideInInspector]
    public Rigidbody2D rigidbody;
    [HideInInspector]
    public Seeker seeker;
    public Transform[] portalTarget;
    public uint portalIndex;
    public Transform target;
    public LayerMask targetLayer;
    public LayerMask obstacleLayer;
    [HideInInspector]
    public bool isTargetInArea;
    [HideInInspector]
    public AttackArea attackArea;
    [HideInInspector]
    public bool isDead;
    public GameObject deadBodyPrefab;
    public DeadBodySO deadBodySO;
    public GameObject BloodPrefab;
    public StageID stageID;
    public bool isFreeze;
    public float reactTime;
    [HideInInspector]
    public bool isHeared;//听觉相关
    [HideInInspector]
    public Vector3 heardPos;//听觉相关，记录【第一次】听见声音的位置，而非上一次，没错听见之后他就聋了，但没有瞎
    public GameObject lootPrefab; // 掉落的枪
    public GameObject questionMark, exclamationMark;
    [HideInInspector]
    public bool targeted; // 是否被玩家瞄准
    public bool isSummoned;
    public bool showBlood = true;
    public bool canHeard = true;
}
public class simpleFSM : BaseBehavior,IHearingReceiver
{
    //public StateType curState;
    public AIParamater paramater;
    public StateType curType;
    protected float observerOffsetY = -1.0f;
    protected Dictionary<StateType,BaseState> statesDic = new Dictionary<StateType, BaseState>();
    protected BaseState curState;
    protected Vector3 oriPos;
    protected Quaternion oriRot = Quaternion.identity;
    protected GameObject Player, loot;
    protected int index = 0;
    protected Vector3 lastTargetPos;//记录时停触发时，目标所处的位置

    protected void Awake() {
        StateRegister();
        //观察者模型——观察者
        EventRegister();
        Player = GameObject.FindWithTag("Player");
        if (Player)
            paramater.target = Player.transform;
        //获取组件
        paramater.animator = GetComponent<Animator>();
        paramater.rigidbody = GetComponent<Rigidbody2D>();
        paramater.seeker = GetComponent<Seeker>();
        paramater.bodyCollinder = GetComponent<CapsuleCollider2D>();
        paramater.attackArea = GetComponentInChildren<AttackArea>();

        StateTransition(StateType.Idle);

        oriPos = transform.position;
        oriRot = transform.rotation;
        paramater.HP = paramater.MAX_HP;
    }


    private void Start() {
        PortalPathInit();
    }
    protected void Update() {
        curState.OnUpdate();
    }

    protected void FixedUpdate() {
        //玩家是否在视野内，与玩家是否在视野的视角内搭配
        //----------------------------------------
        if (Physics2D.OverlapCircle(transform.position,paramater.maxChaseDistance,paramater.targetLayer))
        {
            paramater.isTargetInArea = true;
        }
        else
        {
            paramater.isTargetInArea = false;
        }
    }

    private void OnDestroy() {
        EventRemover();
    }

    protected virtual void StateRegister()
    {
        if(statesDic.Count != 0)
        {
            //Debug.LogError("名字为 [" + gameObject.name + "] 的物体已经注册过一次所有状态");
            return;
        }
        else
        {
            statesDic.Add(StateType.Idle,new IdleState(this));
            statesDic.Add(StateType.Partol,new PartolState(this));
            statesDic.Add(StateType.Chase,new ChaseState(this));
            statesDic.Add(StateType.React,new ReactState(this));
            statesDic.Add(StateType.Attack,new AttackState(this));
            statesDic.Add(StateType.Dead,new DeadState(this));
            statesDic.Add(StateType.Down,new DownState(this));
            statesDic.Add(StateType.ExcuteDead,new ExcuteDeadState(this));
            statesDic.Add(StateType.Confused,new ConfusedState(this));
            statesDic.Add(StateType.Reach,new ReachState(this));
        }
    }
    #region 寻路
    protected void PortalPathInit()
    {
        int N = paramater.portalTarget.Length;
        paramater.partolPath = new ABPath[N];
        if(AstarPath.active == null)
        {Debug.LogError("A* not exist!!"); return;}
        for (int i = 0; i < N; i++)
        {
            var p = ABPath.Construct(
                paramater.portalTarget[i].position,paramater.portalTarget[(i+1)%N].position,OnPartolPathComplete
            );
            AstarPath.StartPath(p);
        }
    }

    protected void OnPartolPathComplete(Path p)
    {
        if(!p.error)
        {
            paramater.partolPath[index] = p as ABPath;
            index++;
        }
    }
    //传统版本,默认目的地是target
    public void UpdatePath()
    {
        if(paramater.seeker.IsDone() && !paramater.reachedEndOfPath)//防止重复更新
            //                                              确保后台加载路径不造成卡顿
            if(paramater.target != null)
                paramater.seeker.StartPath(transform.position,paramater.target.position,OnChasePathComplete);
            else
                paramater.seeker.StartPath(transform.position,oriPos,OnChasePathComplete);
    }
    //输入目的地版本
    public void UpdatePath(Vector3 targetPos)
    {
        if(paramater.seeker.IsDone() && !paramater.reachedEndOfPath)//防止重复更新
            //                                              确保后台加载路径不造成卡顿
            paramater.seeker.StartPath(
                transform.position,targetPos,OnChasePathComplete);
    }
    protected void OnChasePathComplete(Path p)
    {
        if(!p.error)
        {
            paramater.chasePath = p as ABPath;
            paramater.chaseIndex = 1;
        }
    }
    
    #endregion
    #if UNITY_EDITOR
    #region GIZMOS
    protected void OnDrawGizmosSelected() {
        Vector3 offset = transform.up * observerOffsetY;
        #region 绘制视角范围
        Gizmos.color = Color.cyan;
        Vector3 lView,rView;
        //运算公式法
        //---------
        // float x = transform.up.x;
        // float y = transform.up.y;
        // float rad = Mathf.Deg2Rad * paramater.chaseViewAngle;
        // float sin = Mathf.Sin(rad);
        // float cos = Mathf.Cos(rad);
        // lView = new Vector3(x * cos + y * sin,-x * sin + y * cos,0f);
        // rView = new Vector3(x * cos - y * sin,x * sin + y * cos,0f);
        
        //API法
        //----------
        lView = Quaternion.Euler(0,0,paramater.chaseViewAngle) * transform.up;
        rView = Quaternion.Euler(0,0,-paramater.chaseViewAngle) * transform.up;
        lView = lView.normalized * paramater.maxChaseDistance;
        rView = rView.normalized * paramater.maxChaseDistance;
        Gizmos.DrawLine(transform.position + offset,lView + transform.position);
        //Gizmos.DrawRay(transform.position,lView);
        Gizmos.DrawLine(transform.position + offset,rView + transform.position);
        #endregion
        
        #region 绘制视野圆圈
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position,paramater.maxChaseDistance);
        #endregion
    
        #region 绘制杂七杂八的线
        // Gizmos.color = Color.magenta;
        // if(paramater.isTargetInArea)
        //     Gizmos.DrawLine(transform.position + offset,paramater.target.position);
        // Gizmos.color = Color.yellow;
        // Gizmos.DrawLine(transform.position + offset,hitPos);
        //边就业边学习，如果搞不定，那说明你不是那个料。
        //怕履历花掉，怕兼职毁掉自己简历的体面，幻想履历具有增值性(去过什么nb公司律所等)
        //履历鄙视链   下大棋鄙视链
        #endregion
    }
    #endregion
    #endif
    
    #region 状态相关
    public void StateTransition(StateType _type)
    {
        if(curState != null)
        {
            curState.OnExit();
        }
        // if (_type == StateType.Reach)
        // {
        //     Debug.Log("["+gameObject.name+"]:State Transition <"+curType+"> to <"+_type+">");
        // }
        curState = statesDic[_type];
        curType = _type;
        curState.OnEnter();
    }

    public void turn(Vector3 targetDir)//一般版本：物体的y轴始终朝向targetDir方向
    {
        targetDir = Vector3.RotateTowards(transform.up,targetDir,paramater.turnSpeed * time.deltaTime,0f);
        targetDir.z = 0;
        float angle = Vector3.SignedAngle(transform.up,targetDir,Vector3.forward);
        Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
        transform.rotation *= quaternion;
    }
    public void turn()//默认版本：物体的y轴始终朝向速度方向
    {
        Vector3 targetDir = paramater.rigidbody.velocity;
        targetDir = Vector3.RotateTowards(transform.up,targetDir,paramater.turnSpeed * time.deltaTime,0f);
        targetDir.z = 0;
        float angle = Vector3.SignedAngle(transform.up,targetDir,Vector3.forward);
        Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
        transform.rotation *= quaternion;
    }

    Sprite GetDeadSprite(AttackArea.AttackType attackType){
        if(paramater.deadBodySO == null) return null;
        foreach (var dictionary in paramater.deadBodySO.deadBodyDictionary)
        {
            if(dictionary.type == attackType){
                int random = UnityEngine.Random.Range(0,dictionary.spriteList.Count);
                return dictionary.spriteList[random];
            }
        }
        return null;
    }
    public void CreateBodyAndDestoryMyself()
    {
        //生成尸体
        GameObject bodyObj = Instantiate(paramater.deadBodyPrefab, transform.position, Yquaternion);
        DeadBody body = bodyObj.GetComponent<DeadBody>();
        if(body!=null){
            body.setStageID(paramater.stageID);
            body.SetExplode(paramater.canBodyExplode);
            Sprite sprite = GetDeadSprite(AttackArea.AttackType.jinzhan);//一直是近战造成的尸体
            if(sprite!=null) body.deadSprite = sprite;
            body.BodyMove(paramater.showBlood);
        }
        if (paramater.lootPrefab)
        {
            loot = Instantiate(paramater.lootPrefab, transform.position, Yquaternion); // 生成武器
            WeaponItem lootItem = loot.GetComponent<WeaponItem>();
            lootItem.haveOwner = true;
            if (paramater.attackArea is NormalAttack rangedAttack)
                lootItem.GetComponent<WeaponItem>().SetAmmoAmount(rangedAttack.bulletNum);
            else if (paramater.attackArea is LaserAttack laserAttack)
                lootItem.GetComponent<WeaponItem>().SetAmmoAmount(laserAttack.bulletNum);
        }

        if (paramater.isSummoned)
        {
            EventRemover();
            Destroy(gameObject);
        }
        else
        {
            gameObject.SetActive(false);
        }
        //Destroy(gameObject);
    }

    /***
    *已废弃
    */
    public void CreateBodyJustHereAndDestoryMyself()
    {
        //生成血迹&尸体
        Instantiate(paramater.BloodPrefab,transform.position,transform.rotation);
        GameObject body = Instantiate(paramater.deadBodyPrefab,transform.position,Yquaternion);
        body.GetComponent<DeadBody>().setStageID(paramater.stageID);
        gameObject.SetActive(false);
        //Destroy(gameObject);
        //延迟执行
    }

    public void ReLive()
    {
        StateTransition(StateType.Idle);
        paramater.isHeared = false;
        paramater.HP = paramater.MAX_HP;
    }

    public void Attack()
    {
        paramater.attackArea.Attack();
    }

    public void Exclamation()
    {
        Instantiate(paramater.exclamationMark, transform.position - transform.up * 0.25f, Quaternion.identity);
    }

    public void Question()
    {
        Instantiate(paramater.questionMark, transform.position - transform.up * 0.25f, Quaternion.identity);
    }

    //玩家是否在视野的视角内并且没有不透明物体阻碍，与玩家是否在视野内搭配
    //----------------------------------------
    protected bool showSightedLine;
   protected Vector3 hitPos;
    public bool IsPlayerSighted()
    {
        if(paramater.target == null || !paramater.isTargetInArea)
            return false;
        else
        {
            //Vector2 dir = Player.transform.position - transform.position;旧版本
            Vector2 dir = Player.transform.position - (transform.position + transform.up * observerOffsetY);//加上偏移量的新版本
            float angle = Vector2.SignedAngle(transform.up,dir.normalized);
            if(Mathf.Abs(angle) < paramater.chaseViewAngle)
            {
                //如果有不透明物体阻碍
                RaycastHit2D hit2D;
                hit2D = Physics2D.Raycast(transform.position,dir,dir.magnitude,paramater.obstacleLayer);
                if(hit2D.collider == null)//射线没有射到物体
                {
                    return true;
                }
                else
                {
                    hitPos = hit2D.point;
                    //修bug:玩家离障碍物过近，即使看上去没有阻碍，AI也判定为看不见
                    if(dir.magnitude < paramater.BlockOffset)//只要玩家到敌人的距离<BlockOffset敌人就能看得见玩家
                        return true;
                    //Debug.Log("blocked");
                    return false;
                }
            }
            else
                {   //Debug.Log("out of angle");
                    return false;}
        }
    }
    //判断时停结束后,根据玩家时停前后的位置,AI会进入到什么状态:
    //8=困惑 or 2=惊觉 or -1=Nothing
    public int TimeStopOffJudge(Vector3 curTargetPos)
    {
        float len1,len2;//分别是时停结束前后位置距离,时停结束后目标距离AI的距离
        
        len2 = Vector3.Distance(curTargetPos,transform.position);
        //如果目标距离AI太远
        if(len2 > paramater.maxChaseDistance){
            return (int)StateType.Confused;
        }
        else{
            len1 = Vector3.Distance(lastTargetPos,curTargetPos);
            //len1足够小
            if(len1 < 0.1f){
                return -1;
            }else{
                //如果目标在视野范围内
                if(IsPlayerSighted()){
                    return (int)StateType.React;
                }else{
                    return (int)StateType.Confused;
                }
            }
        }
    }

#endregion
    
    #region 事件相关

    protected void EventRegister()
    {
        MessageCenter.AddListener(OnGameRestart);
        MessageCenter.AddListener(OnLevelClear);
        MessageCenter.AddListener(OnTimeStopOff);
        MessageCenter.AddListener(OnTimeStopOn);
    }

    protected void EventRemover()
    {
        MessageCenter.RemoveListner(OnGameRestart);
        MessageCenter.RemoveListner(OnLevelClear);
        MessageCenter.RemoveListner(OnTimeStopOff);
        MessageCenter.RemoveListner(OnTimeStopOn);
    }

    public virtual void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        if(msg.intParam != (int)paramater.stageID) return;
        transform.position = oriPos;
        transform.rotation = oriRot;
        paramater.portalIndex = 0;
        
        if (paramater.attackArea is MeleeAttack)
            GetComponentInChildren<BoxCollider2D>().enabled = false; // 防止动画播放到一半时被玩家砍死，导致AttackArea的碰撞没有关闭，从而在Restart后”撞“死玩家

        if (paramater.attackArea is ExplosionAttack)
            paramater.chaseSpeed = 12;

        gameObject.SetActive(true);
        if (loot)
        {
            Destroy(loot);
            loot = null;
        }
        ReLive();
    }
    
    public void OnLevelClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR) return;
        EventRemover();
    }
    
    protected float Yangle;protected Quaternion Yquaternion;
    public void GetDamaged(Vector3 dir,int damage = 2)
    {
        if(!paramater.isDown && !paramater.isDead)
        {
            paramater.HP -= damage;
            if (paramater.HP <= 0)
            {
                paramater.isDead = true;
                MessageCenter.SendMessage(new CommonMessage()
                {
                    Mid = (int)MESSAGE_TYPE.ADD_SCORE,
                    intParam = (int)paramater.stageID,
                    content = this
                });
                if(paramater.showBlood)
                    SoundManager.PlayAudio("slash");
                else
                    SoundManager.PlayAudio("metalstep");
            }
            //生成血迹
            //Bloody
            //angle = Vector3.SignedAngle(Vector3.right,dir,Vector3.forward);
            Yangle = Vector3.SignedAngle(-Vector3.up,dir,Vector3.forward);//-V3.up是因为尸体的朝向与transform.up朝向相反的缘故;
            //2024.4.27:溅血朝向也与up相反
            //quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
            Yquaternion = Quaternion.AngleAxis(Yangle,Vector3.forward);
            if (paramater.showBlood)
            {
                Instantiate(paramater.BloodPrefab, transform.position, Yquaternion);
            }
        }
    }

    protected void OnTimeStopOn(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_ON) return;
        paramater.isFreeze = true;
        lastTargetPos = paramater.target.position;
        time.animator.speed = 0f;
    }

    protected void OnTimeStopOff(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.TIME_STOP_OFF) return;
        paramater.isFreeze = false;
        time.animator.speed = 1f;
    }

    #endregion

    public Vector3 getLastTargetPos()
    {
        return lastTargetPos;
    }

    public void OnHeard(Vector3 msg)
    {
        if(!paramater.isHeared && paramater.canHeard)
        {
            paramater.isHeared = true;
            paramater.heardPos = (Vector3)msg;
        }
    }

    public Vector3 getOriPos()
    {
        return oriPos;
    }
    public void setStageID(StageID _id)
    {
        paramater.stageID = _id;
    }
}
