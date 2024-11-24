using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransGate : MonoBehaviour
{
    [Header("这扇门本身的stageID")]
    [SerializeField]private StageID stageID;
    [Header("这扇门将会激活的stageID")]
    [SerializeField]private StageID targetStageID;
    [SerializeField]private Transform targetTrans;
    private GameObject arrow;
    //private Collider2D barrier;
    private Collider2D triggerField;
    

    private void OnEnable() {
        MessageCenter.AddListener(OnStageClear);
    }

    // Start is called before the first frame update
    void Start()
    {
        //barrier = transform.Find("barrier").GetComponent<Collider2D>();
        triggerField = GetComponent<Collider2D>();
        arrow = Instantiate(TheShitOfReference.AllShitOfReference.TransGate_arrow,transform.position,Quaternion.identity);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.DrawLine(transform.position,targetTrans.position);
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnStageClear);
    }

    #region  事件

    public void OnStageClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.STAGE_CLEAR) return;
        if(msg.intParam != (int)stageID) return;

        DoorOpen();
    }

    void MoveNextStage()
    {
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.MOV_nSTAGE,
            intParam = (int)targetStageID,
            target = targetTrans
        });
    }

    #endregion

    void DoorOpen()
    {
        triggerField.enabled = true;
        GetComponent<Animator>().Play("TransGate_Open");
        if (arrow != null)
        {
            arrow.SetActive(true);
        }
    }

    void DoorClosed()
    {
        triggerField.enabled = false;
        GetComponent<Animator>().Play("TransGate_idle");
        if (arrow != null)
        {
            arrow.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            SoundManager.PlayAudio("transGate");
            StartCoroutine(MovePlayer(other.transform));
            if(UIManager.isInitialize)
            {
                UIManager.Instance.FadeInOut(.7f);
            }
        }
    }

    IEnumerator MovePlayer(Transform player)
    {
        yield return new WaitForSeconds(.4f);
        player.transform.position = new Vector3(
                targetTrans.position.x,targetTrans.position.y,player.transform.position.z);
        MoveNextStage();

    }
}
