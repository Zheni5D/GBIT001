using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger_Event : MonoBehaviour
{
    public int NextDialogNodeID;
    void Awake()
    {
        MessageCenter.AddListener(OnLevelClear);
    }
    void OnDestroy()
    {
        MessageCenter.RemoveListner(OnLevelClear);
    }
    void OnLevelClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR) return;
        StartCoroutine(fun());
        
    }
    IEnumerator fun()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        myEventManager.instance.EventProceed();
    }
}
