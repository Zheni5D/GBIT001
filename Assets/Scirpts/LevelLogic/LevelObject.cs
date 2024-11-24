using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObject : MonoBehaviour
{
    [SerializeField]private int maxStageCnt;
    public static int curStageIndex;//无奈之举，为了方便和维护代码整洁,牺牲封装性

    protected void OnEnable() {
        MessageCenter.AddListener(OnStageClear);
        MessageCenter.AddListener(OnMovNexStage);
    }

    // Start is called before the first frame update
    protected void Start()
    {
        maxStageCnt = transform.childCount;
        curStageIndex = 0;
        //Debug.Log("The number of Stage is "+maxStageCnt);
    }

    protected void OnDisable()
    {
        MessageCenter.RemoveListner(OnStageClear);
        MessageCenter.RemoveListner(OnMovNexStage);
    }
    #region 事件
    protected virtual void LevelClear()
    {
        if(UIManager.isInitialize && LevelManager.isInitialize)
        {
            if(LevelManager.Instance.enableReplay == false)
                UIManager.Instance.LevelCmpAnim();
        }
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.LEVEL_CLEAR
        });
    }

    protected void OnStageClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.STAGE_CLEAR) return;
        //由于StageID.One的int值为0,所以..直接判断就好
        if(curStageIndex == (int)msg.intParam)
        {
            //确定是当前stage清除
            curStageIndex++;            
            if(curStageIndex >= maxStageCnt)
            {
                LevelClear();
                return;
            }
            if(UIManager.isInitialize)
            {
                UIManager.Instance.StageCmpAnim();
            }
            //Debug.Log("Stage " + curStageIndex + " Incoming!");
        }   
    }

    protected void OnMovNexStage(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
        if(msg.intParam < maxStageCnt)
        {
            transform.GetChild(msg.intParam).gameObject.SetActive(true);
        }
    }
    #endregion
}
