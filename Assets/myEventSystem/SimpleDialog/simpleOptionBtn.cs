using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simpleOptionBtn : MonoBehaviour
{
    public void PlayerInteractionA()
    {
        myEventManager.instance.EventProceed(EventNode.BranchType.A,false);
        simpleUIManager.instance.ToggleOptionBox(false);
    }
    public void PlayerInteractionB()
    {
        myEventManager.instance.EventProceed(EventNode.BranchType.B,false);
        simpleUIManager.instance.ToggleOptionBox(false);
    }
}
