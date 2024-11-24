using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SimpleTimer : MonoBehaviour
{
    [Tooltip("speed=2,真实1s=游戏内2s")]
    [Range(0.1f,10f)]
    public float speed;
    public Image[] minutesImage = new Image[2];
    public Image[] secondsImage = new Image[2];
    public event EventHandler<float> TimerEventHandler;
    private float totalTime;
    public float TotalTime{
        set{
            totalTime = value;
            TimerEventHandler?.Invoke(this,value);
        }
        get{
            return totalTime;
        }
    }
    private float timer;
    private float ratio;
    private int seconds;
    private int minutues;
    private bool pause;

    private void OnEnable() {
        MessageCenter.AddListener(OnPauseOff);
        MessageCenter.AddListener(OnPauseOn);
    }
    // Start is called before the first frame update
    void Start()
    {
        ratio = 1.0f / speed;
        // TimerEventHandler += SetTimerText;
    }

    // Update is called once per frame
    void Update()
    {
        if(pause) return;
        
        timer += Time.deltaTime;
        if(timer > ratio){
            TotalTime += 1.0f;
            timer = 0;
        }
    }

    private void OnDisable() {
        // TimerEventHandler -= SetTimerText;
        MessageCenter.RemoveListner(OnPauseOff);
        MessageCenter.RemoveListner(OnPauseOn);
    }

    public void CaclusTotalTime(){
        int IntegerTotaltime = Mathf.FloorToInt(TotalTime);
        minutues = IntegerTotaltime / 60;
        int ten = minutues / 10;
        if(ten>99) ten = 99;
        int one = minutues % 10;
        minutesImage[0].sprite = UIManager.Instance.numbers[one];
        minutesImage[1].sprite = UIManager.Instance.numbers[ten];

        seconds = IntegerTotaltime % 60;
        ten = seconds / 10;
        one = seconds % 10;
        secondsImage[0].sprite = UIManager.Instance.numbers[one];
        secondsImage[1].sprite = UIManager.Instance.numbers[ten];
        //Debug.Log("[用时]"+minutues+"m "+seconds+"s");
    }

    public void OnPauseOn(CommonMessage msg){
        if (msg.Mid != (int)MESSAGE_TYPE.PAUSE_ON) return;
        PauseTimer();
    }

    public void OnPauseOff(CommonMessage msg){
        if (msg.Mid != (int)MESSAGE_TYPE.PAUSE_OFF) return;
        StartTimer();
    }

    public void PauseTimer(){
        pause = true;
    }

    public void StartTimer(){
        pause = false;
    }

}
