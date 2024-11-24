using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class BaseBehavior : MonoBehaviour
{
    public Timeline time
    {
        get
        {
            return GetComponent<Timeline>();
        }
    }
    /* 定时触发
    float dt = Time.deltaTime;
    this.passedTime += dt;
    if(this.passedTime >= 阈值) {
            doSomeThing();
            this.passedTime -= 阈值;
    }
    */
}
