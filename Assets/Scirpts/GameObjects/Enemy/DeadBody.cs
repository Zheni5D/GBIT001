using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Chronos;
public class DeadBody : BaseBehavior
{
    public float speed = 2f;
    [Tooltip("飞出去的距离")]
    [SerializeField]private float distance;
    [Tooltip("飞出去的时间")]
    [SerializeField]private float duration;
    private bool _canExplode;
    public Sprite deadSprite{
        get{
            if(spriteRenderer==null){
                spriteRenderer = transform.Find("body").GetComponent<SpriteRenderer>();
            }
            return spriteRenderer.sprite;
        }
        set{
            if(spriteRenderer==null){
                spriteRenderer = transform.Find("body").GetComponent<SpriteRenderer>();
            }
            spriteRenderer.sprite = value;
        }
    }
    private SpriteRenderer spriteRenderer;
    private StageID stageID;
    Vector3 targetPoint;
    static int bodyIndex = 0;
    private void Awake() {
        MessageCenter.AddListener(OnGameRestart);
    }
    private void OnEnable() {
        
    }
    private void Update() {
        // if(Vector3.Distance(transform.position,targetPoint) > .1f)
        // {
        //     transform.position = Vector3.Lerp(transform.position,targetPoint,time.deltaTime * speed);
        // }
    }

    private void OnDestroy() {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        if(msg.intParam != (int)stageID) return;
        Destroy(gameObject);
    }

    public void setStageID(StageID id)
    {
        stageID = id;
    }

    public void BodyMove(bool showBlood)
    {
        targetPoint = transform.position + -transform.up * distance;
        if (showBlood)
        {
            bodyIndex = bodyIndex % 4 + 1;
            //Debug.Log("blood"+bodyIndex.ToString());
            GetComponent<Animator>().Play("blood" + bodyIndex.ToString());
        }
        TimeScaledDoMove.Instance.DoMove(GetComponent<Timeline>(), targetPoint, duration).OnCallBack(Explode);
    }

    private void Explode()
    {
        if(_canExplode)
            Instantiate(TheShitOfReference.AllShitOfReference.CirCleExplison,transform.position,Quaternion.identity);
    }

    public void SetExplode(bool v)
    {
        _canExplode = v;
    }
}
