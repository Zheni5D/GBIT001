using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleWave : BaseBehavior
{
    public GameObject Enemy;
    simpleFSM fSM;
    float t=0f;
    float tt = 1f;
    // Start is called before the first frame update
    void Start()
    {
        fSM = Enemy.GetComponent<simpleFSM>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Enemy.activeSelf == false)
        {
            t += time.deltaTime;
            if(t >= tt) {
                
                Enemy.SetActive(true);
                fSM.ReLive();
                t -= tt;
            }
        }
    }
}
