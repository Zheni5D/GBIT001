using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Chronos;

public class STesterA : MonoBehaviour
{
    public Sprite spritel;
    Timeline timeline;
    Rigidbody2D rigi;
    GameObject g;
    private bool isRotating = false;
    private float curTime = 0;
    private float AllTime = 3;

    private Quaternion oldQ;
    private Quaternion targetQ;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        g = new GameObject("test");
        g.AddComponent<SpriteRenderer>().sprite = spritel;
        rigi=g.AddComponent<Rigidbody2D>();//只要适配组件在Timeline添加前添加就可以?
        timeline = g.AddComponent<Timeline>();
        timeline.mode = TimelineMode.Global;
        timeline.rewindable = false;
        timeline.globalClockKey = Timekeeper.instance.Clock("root").key;
        
        timeline.rigidbody2D.gravityScale = 0;

        //TimeScaledDoMove.Instance.DoMove(timeline,g.transform.position+new Vector3(0,50f,0),5f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            isRotating = true;
            curTime = 0;
            
            //当前旋转
            oldQ = g.transform.rotation;
            //目标旋转
            targetQ = Quaternion.Euler(target.rotation.eulerAngles);
        }

        if (isRotating)
        {
            curTime += timeline.deltaTime; 
            float t = curTime / AllTime;
            t = Mathf.Clamp(t, 0, 1);
            
            //用t进行插值
            Quaternion lerpQ = Quaternion.Lerp(oldQ,targetQ,t);
            //设置到目标旋转
            g.transform.rotation = lerpQ;

            Debug.Log($"{GetType()} curT:{t}");
            if (t >= 1)
            {
                isRotating = false;
            }
        }
    }
}
