using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Chronos;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TimeScaledDoMove : SingleTon<TimeScaledDoMove>
{
    public class WorkPackage<T>{
        public bool validate;
        public TimelineChild timelineChild;
        public Timeline timeline;
        public Transform transform;
        public float duration;
        public float Timer;
        public T startValue;
        public T endValue;
        private Action CallBackAction;
        public WorkPackage(TimelineChild timelineChild, float duration, T endValue){
            this.timelineChild = timelineChild;
            this.endValue = endValue;
            this.duration = duration;
            transform = timelineChild.gameObject.transform;
            validate = true;
        }
        public WorkPackage(Timeline timeline, float duration, T endValue){
            this.timeline = timeline;
            this.endValue = endValue;
            this.duration = duration;
            transform = timeline.gameObject.transform;
            validate = true;
        }
        public void SetWorkPackage(){
            Timer = 0;
        }
        public void OnCallBack(Action action){
            CallBackAction = action;
        }
        public void CallBack(){
            if(CallBackAction!=null){
                CallBackAction();
            }
        }
    }
    public class WorkPackageVector3 : WorkPackage<Vector3>
    {
        public WorkPackageVector3(TimelineChild timelineChild, float duration, Vector3 endValue):base(timelineChild,duration,endValue)
        {
            startValue = transform.position;
        }
        public WorkPackageVector3(Timeline timeline, float duration, Vector3 endValue):base(timeline,duration,endValue)
        {
            startValue = transform.position;
        }
        // public void SetWorkPackageVector3(TimelineChild timelineChild, float duration, Vector3 endValue)
        // {
        //     this.timelineChild = timelineChild;
        //     this.endValue = endValue;
        //     this.duration = duration;
        //     transform = timeline.gameObject.transform;
        //     startValue = transform.position;
        //     validate = true;
        //     Timer = 0;
        // }
    }
    public class WorkPackageQuaterion
    {
        public bool validate;
        public TimelineChild timeline;
        public Transform transform;
        public float duration;
        public float Timer;
        public Quaternion startValue;
        public Quaternion endValue;
        public WorkPackageQuaterion(TimelineChild timeline, float duration, Quaternion endValue)
        {
            this.timeline = timeline;
            this.endValue = endValue;
            this.duration = duration;
            transform = timeline.gameObject.transform;
            startValue = transform.rotation;
            validate = true;
        }
        public void SetWorkPackageQuaterion(TimelineChild timeline, float duration, Quaternion endValue)
        {
            this.timeline = timeline;
            this.endValue = endValue;
            this.duration = duration;
            transform = timeline.gameObject.transform;
            startValue = transform.rotation;
            validate = true;
            Timer = 0;
        }
    }
    private List<WorkPackageVector3> WorkListVector3 = new List<WorkPackageVector3>();
    private List<WorkPackageQuaterion> WorkListQuaterion = new List<WorkPackageQuaterion>();

    protected override void Awake()
    {
        base.Awake();
        if (WorkListVector3 == null)
        {
            WorkListVector3 = new List<WorkPackageVector3>();
        }
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    // Update is called once per frame
    void Update()
    {
        if (WorkListVector3.Count > 0)
        {
            foreach (var work in WorkListVector3)
            {
                if (work.validate)
                {
                    //Timeline/TImelineChild&判空判断
                    if(work.timelineChild!=null){
                        work.Timer += work.timelineChild.parent.deltaTime;
                    }
                    else if(work.timeline!=null){
                        work.Timer += work.timeline.deltaTime;
                    }
                    else{
                        work.validate = false;
                        continue;
                    }
                    //插值
                    work.transform.position = easeOutQuint(work.Timer, work.endValue, work.startValue, work.duration);
                    //插值完毕
                    if (work.Timer >= work.duration)
                    {
                        work.CallBack();
                        work.validate = false;
                    }
                }
            }
        }
        if (WorkListQuaterion.Count > 0)
        {
            foreach (var work in WorkListQuaterion)
            {
                if (work.validate)
                {
                    if (work.timeline != null)
                    {
                        work.Timer += work.timeline.parent.deltaTime;
                        float t = work.Timer / work.duration;
                        work.transform.rotation = Quaternion.Lerp(work.startValue, work.endValue, t);
                        if (work.Timer >= work.duration)
                        {
                            work.validate = false;
                        }
                    }
                    else
                    {
                        work.validate = false;
                    }
                }

            }
        }
    }


    private void LateUpdate()
    {
        if (WorkListVector3.Count > 0)
        {
            WorkListVector3.RemoveAll(w => w.validate == false);
        }
        if (WorkListQuaterion.Count > 0)
        {
            WorkListQuaterion.RemoveAll(w => w.validate == false);
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }



    public WorkPackageVector3 DoMove(TimelineChild timeline, Vector3 targetWorldPos, float duration,Action callback = null)
    {
        Transform t = timeline.gameObject.transform;
        WorkPackageVector3 work;
        // work = WorkListVector3.Find(x => x.validate == false);
        // if (work == null)
            work = new WorkPackageVector3(timeline, duration, targetWorldPos);
        // else
        //     work.SetWorkPackageVector3(timeline, duration, targetWorldPos);
        work.OnCallBack(callback);
        WorkListVector3.Add(work);
        return work;
    }
    public WorkPackageVector3 DoMove(Timeline timeline, Vector3 targetWorldPos, float duration, Action callback = null)
    {
        Transform t = timeline.gameObject.transform;
        WorkPackageVector3 work;
        // work = WorkListVector3.Find(x => x.validate == false);
        // if (work == null)
            work = new WorkPackageVector3(timeline, duration, targetWorldPos);       
            work.OnCallBack(callback);

        // else
        //     work.SetWorkPackageVector3(timeline, duration, targetWorldPos);
        WorkListVector3.Add(work);
        return work;
    }

    public void DoRotateBasedOnZ(TimelineChild timeline, Vector3 endEular, float duration)
    {
        Transform t = timeline.gameObject.transform;
        Quaternion targetQ = Quaternion.Euler(endEular) * t.rotation;
        WorkPackageQuaterion work;
        // work = WorkListQuaterion.Find(x => x.validate == false);
        // if (work == null)
            work = new WorkPackageQuaterion(timeline, duration, targetQ);
        // else
        //     work.SetWorkPackageQuaterion(timeline, duration, targetQ);
        WorkListQuaterion.Add(work);
    }

    void OnSceneUnloaded(Scene a1)
    {
        if (WorkListVector3 != null)
        {
            WorkListVector3.Clear();
        }
        if (WorkListQuaterion != null)
        {
            WorkListQuaterion.Clear();
        }
    }
    #region 工具函数
    //a->b with x
    private float easeOutQuint(float t, float end, float begin, float duration)
    {
        t = (t / duration) - 1.0f;
        float chazhi = end - begin;
        return begin + chazhi * (t * t * t * t * t + 1.0f);//(x-1)^5+1
    }
    private Vector3 easeOutQuint(float t, Vector3 end, Vector3 begin, float duration)
    {
        t = (t / duration) - 1.0f;
        Vector3 chazhi = end - begin;
        return begin + chazhi * (t * t * t * t * t + 1.0f);//(x-1)^5+1
    }
    #endregion
}
