using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageReverseObject : MonoBehaviour
{
    [SerializeField]private StageID stageID;
        
    private void OnEnable() {
        MessageCenter.AddListener(OnGameRestart);
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
}
