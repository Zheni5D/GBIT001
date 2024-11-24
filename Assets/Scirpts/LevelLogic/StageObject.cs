using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StageID
{
    One,
    Two,
    Three,
    Four,
    Five
}

public class StageObject : MonoBehaviour
{
    public StageID myStageID;
    [SerializeField]private int enemyLeft;
    [SerializeField]private int enemyTotal;

    private void OnEnable() {
        MessageCenter.AddListener(OnEnemyDead);
        MessageCenter.AddListener(OnGameRestart);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyTotal = 0;
        enemyLeft = 0;
        //Debug.Log("The number of enemy is "+ enemyTotal);
        for(int i = 0; i < transform.childCount; i++) {
            var fsm = transform.GetChild(i).GetComponent<simpleFSM>();
            fsm.setStageID(myStageID);
            if(!(fsm is FakeFSM)){
                ++enemyTotal;
            }
        }
        enemyLeft = enemyTotal;
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.J))
        {
            StageClear();
        }
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnEnemyDead);
        MessageCenter.RemoveListner(OnGameRestart);
    }

    public bool HasCleared()
    {
        return enemyLeft == 0;
    }
    
    #region 事件
    void StageClear()
    {
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.STAGE_CLEAR,
            intParam = (int)myStageID
        });
    }

    void OnEnemyDead(CommonMessage msg)
    {
        //检测信息类型&死亡Enemy的StageID
        if(msg.Mid != (int)MESSAGE_TYPE.ADD_SCORE) return;
        if(msg.intParam != (int)myStageID) return;

        simpleFSM enemyFsm = (simpleFSM)msg.content;
        
        if (!enemyFsm.paramater.isSummoned)
        {
            enemyLeft--;
        }
        if(enemyLeft <= 0)
        {
            StageClear();
        }
    }

    void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        
        enemyLeft = enemyTotal;
    }
    #endregion
}
