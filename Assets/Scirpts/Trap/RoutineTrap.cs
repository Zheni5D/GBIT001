using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoutineTrap : BaseBehavior
{
    [SerializeField,Tooltip("陷阱触发间隔")]protected float pop_interval;
    [SerializeField,Tooltip("陷阱持续时间")]protected float show_period;
    protected bool active = true;
    protected bool trapShow = false;
    protected float timer;

    // Update is called once per frame
    protected virtual void Update()
    {
        if (active)
        {
            if (!trapShow)
            {
                timer += time.deltaTime;
                if (timer >= pop_interval)
                {
                    timer = 0f;
                    trapShow = true;
                    TrapOn();
                }
            }
            else
            {
                timer += time.deltaTime;
                if (timer >= show_period)
                {
                    timer = 0f;
                    trapShow = false;
                    TrapOff();
                }
            }
        }
    }

    protected virtual void TrapOn()
    {
    }

    protected virtual void TrapOff()
    {
    }
}
