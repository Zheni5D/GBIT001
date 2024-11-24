using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Chronos;

public class PlayerControl : BaseBehavior
{
    enum State
    {
        Normal,
        Roll
    }
    public float speed;
    public LayerMask floorMask;
    public float attackSpeed;
    public float excuteRadius;
    [Tooltip("角色一直盯着看的物体")]
    [HideInInspector]
    public Transform AimingTarget;
    private Vector3 mov;

    //private Transform weaponTrans;
    //private Animator weaponAnim;

    private ReplaySystem replaySystem;

    private Rigidbody2D rigi;

    private bool isDead;
    private Vector3 oriPos;
    private Animator anim;
    #region Roll
    private Vector3 rollDir;
    private State state;
    [SerializeField] private float rollSpeed;
    private LayerMask oriLayer;
    float _rollspeed;
    private Vector3 lastMov;
    private bool isStageCleared;
    private TimeScaleChange timeScaleChange;
    private ComboBox comboBox;
    private bool furyMode = false;  //愤怒模式 1.5倍移速  无敌
    private float furySpeedFactor = 1f;
    [SerializeField]private float furySpeed = 1.75f;
    #endregion

    #region 手感优化待用
    // [HideInInspector]
    // [System.Runtime.InteropServices.DllImport("user32.dll")]
    // public static extern int SetCursorPos(int x, int y);
    // [HideInInspector]
    // [System.Runtime.InteropServices.DllImport("user32.dll")]
    // private static extern bool GetCursorPos(out POINT pt);
    // private void KeepCursorCenter(){
    //     if (HideCursor){
    //         SetCursorPos((int)Screen.width / 2, (int)Screen.height / 2);
    //     }

    //     if (Input.GetKeyDown(KeyCode.Escape)){
    //         HideCursor = false;
    //         Cursor.visible = true;
    //     }
    // }
    #endregion

    #region 调试模式
    private bool canPlayerRoll;
    private bool canPlayerHurt;
    private bool canPlayerAlwaystopTime;
    #endregion

    private void Awake() {
        MessageCenter.AddListener(OnGameRestart);
        MessageCenter.AddListener(OnMovNexStage);
        MessageCenter.AddListener(OnStageClear);
        MessageCenter.AddListener(OnFuryModeOff);
        MessageCenter.AddListener(OnFuryModeOn);
    }

    // Start is called before the first frame update
    void Start()
    {
        //weaponTrans = transform.GetChild(0);
        //weaponAnim = weaponTrans.GetComponent<Animator>();
        anim = GetComponent<Animator>();
        rigi = GetComponent<Rigidbody2D>();
        timeScaleChange = Timekeeper.instance.gameObject.GetComponent<TimeScaleChange>();
        
        MouseMove mouseMove;
        if(TryGetComponent<MouseMove>(out mouseMove))
        {
            AimingTarget = mouseMove.target.transform;
        }

        oriPos = transform.position;
        oriLayer = gameObject.layer;
        state = State.Normal;
        _rollspeed = rollSpeed;
        replaySystem = FindObjectOfType<ReplaySystem>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            canPlayerRoll = !canPlayerRoll;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            canPlayerHurt = !canPlayerHurt;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            GameObject go = GetComponent<PlayerWeapon>().rangedWeapon;
            if (go != null)
            {
                go.GetComponent<WeaponItem>().GetData().Ammo += 100;
                GetComponent<PlayerWeapon>().AmmoAmountChanged(100);
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            canPlayerAlwaystopTime = !canPlayerAlwaystopTime;
            timeScaleChange.canPlayerAlwaysTimeStop = canPlayerAlwaystopTime;
        }
        if (replaySystem.IsReplaying())
        {
            mov = rollDir = Vector3.zero;
        }
        if (!replaySystem.IsReplaying())
        {
            switch (state)
            {
                case State.Normal:
                    if (isDead)
                    {
                        return;
                    }
                    float h = Input.GetAxisRaw("Horizontal");
                    float v = Input.GetAxisRaw("Vertical");
                    mov = new Vector3(h, v);
                    anim.SetBool("Moving", mov.normalized != Vector3.zero);
                    if (h != 0 || v != 0)
                    {
                        //not idle
                        lastMov = mov;
                    }

                    turn();

                    //if (Input.GetKeyDown(KeyCode.E))
                    //{
                    //    Excute();
                    //}
                    if (Input.GetKeyDown(KeyCode.Space) && canPlayerRoll)
                    {
                        rollDir = lastMov;
                        state = State.Roll;
                        rollSpeed = _rollspeed;
                        gameObject.layer = 3;//gameobect.layer和Phycis.Raycast的layer赋
                                             //值好像不一样，前者使用【位掩码的序号】来赋值
                                             //我刚才的操作LayerMask.GetMask()是把二进制掩码转换为十进制整数来赋值了
                                             //比如DownEnemy位掩码 = 0000······01000，转换为十进制整数就是8
                                             //而8正好对应位掩码0000···100000000是Enemy层所以才会出现bug
                                             //正确方法是用0000······01000对应的【位掩码的序号】3
                    }
                    break;
                case State.Roll:
                    float multiplier = 5f;
                    rollSpeed -= rollSpeed * multiplier * time.deltaTime;
                    if (rollSpeed <= 10f)
                    {
                        state = State.Normal;
                        gameObject.layer = oriLayer;
                    }
                    break;
            }
        }
    }

    private void FixedUpdate() {
        switch(state)
        {
            case State.Normal:
            time.rigidbody2D.velocity = mov.normalized * speed * furySpeedFactor;
            break;
            case State.Roll:
            time.rigidbody2D.velocity = rollDir.normalized * rollSpeed;
            break;
        }
    }

    private void OnDestroy() {
        MessageCenter.RemoveListner(OnGameRestart);
        MessageCenter.RemoveListner(OnMovNexStage);
        MessageCenter.RemoveListner(OnStageClear);
        MessageCenter.RemoveListner(OnFuryModeOff);
        MessageCenter.RemoveListner(OnFuryModeOn);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawWireSphere(transform.position,excuteRadius);
    }

    public void PlayFootAudio()
    {
        SoundManager.PlayAudio("footstep1");
    }

    #region 动作,行动
    //角色y轴始终朝向鼠标
    void turn()
    {
        //transform.rotation = Quaternion.LookRotation(); 不能用因为LookAt是Z轴朝向目标方向
        if(AimingTarget == null)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit ,1000f,floorMask))
            {
                Vector3 player2mouse = hit.point - transform.position;
                player2mouse.z = 0;
                //计算角度
                float angle = Vector3.SignedAngle(transform.up,player2mouse,Vector3.forward);
                Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
                transform.rotation *= quaternion;
            }
        }else
        {
            Vector3 player2mouse = AimingTarget.position - transform.position;
            float angle = Vector3.SignedAngle(transform.up,player2mouse,Vector3.forward);
            Quaternion quaternion = Quaternion.AngleAxis(angle,Vector3.forward);
            transform.rotation *= quaternion;
        }
    }

    //void attack()
    //{
    //    weaponAnim.Play("attack");
    //}

    void Excute()
    {
        Collider2D collider= Physics2D.OverlapCircle(transform.position,excuteRadius,LayerMask.GetMask("DownEnemy"));
        if(collider != null)
        {
            simpleFSM fsm = collider.gameObject.GetComponent<simpleFSM>();
            if(fsm != null)
                if(fsm.GetExcuted())
                {
                    Vector3 target = collider.transform.position;
                    transform.position = new Vector3(target.x,target.y,transform.position.z);
                    StartCoroutine(ExcuteSound());
                }
        }
    }

    IEnumerator ExcuteSound()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSecondsRealtime(0.25f);
            SoundManager.PlayAudio("slash");
        }
    }

    private void FuryModeBegin(){
        furyMode = true;
        furySpeedFactor = furySpeed;
    }

    private void FuryModeFin(){
        furyMode = false;
        furySpeedFactor = 1f;
    }
    #endregion

    #region 额..事件源?or公共函数
    public void GetDamaged(Object param)
    {
        MessageCenter.SendMessage(new CommonMessage(){
            Mid = (int)MESSAGE_TYPE.PLAYER_GET_HURT,
            content = param
        });
        if(isStageCleared || canPlayerHurt || furyMode) return;
        isDead = true;
        //生成尸体
        gameObject.SetActive(false);
        

        if (GetComponent<PlayerWeapon>().rangedWeapon && GetComponent<PlayerWeapon>().rangedWeapon.GetComponent<WeaponItem>().GetData() is MagazineLaserGun ls)
        {
            Destroy(ls.laser);
        }

        GameOver();
    }

    public void ChangeTimeEnergyConsumeFactor(int value){
        timeScaleChange.ChangeConsumeFactor(value);
    }
    #region  事件源
    public void GameOver()
    {
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.GAME_OVER,
            content = null
        });
    }

    #endregion

    void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        transform.position = oriPos;
        isDead = false;
        GetComponent<PlayerWeapon>().EndAttack(); // 保证玩家攻击碰撞体关闭
        //重置分数
        gameObject.SetActive(true);
    }

    void OnMovNexStage(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
        oriPos = transform.position;
        isStageCleared = false;
    }

    public void OnStageClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.STAGE_CLEAR) return;
        isStageCleared = true;
    }

    public void OnFuryModeOn(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.FURY_MODE_ON) return;
        FuryModeBegin();
    }
    public void OnFuryModeOff(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.FURY_MODE_OFF) return;
        FuryModeFin();
    }
    #endregion
}
