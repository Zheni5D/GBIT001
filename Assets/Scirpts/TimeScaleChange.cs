using System;
using System.Collections.Generic;
using UnityEngine;
using Chronos;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TimeScaleChange : MonoBehaviour
{
    public KeyCode timeStopKey;
    #region 后处理
    public Volume volume;
    #region 时停玩家看到的效果
    private ScreenShockVolume screenShock;
    private float screenShockRadius{
        get{
            
            if(screenShock == null){return 1.0f;}
            return screenShock._Radius.value;
        }
        set{
            if(screenShock._Radius.overrideState){
                screenShock._Radius.SetValue(new FloatParameter(value));
                screenShock._Radius.overrideState = true;
            }else{
                screenShock._Radius.overrideState = true;
            }
        }
    }
    private float screenShockIndensity{
        get{
            
            if(screenShock == null){return 1.0f;}
            return screenShock._Indensity.value;
        }
        set{
            if(screenShock._Indensity.overrideState){
                screenShock._Indensity.SetValue(new FloatParameter(value));
                screenShock._Indensity.overrideState = true;
            }else{
                screenShock._Indensity.overrideState = true;
            }
        }
    }
    #endregion
    #region 时停回放看到的效果
    private UVOffsetVolume TimeStopTransitionVolume;
    private float TimeStopTransitionIndensity{
        get{
            
            if(TimeStopTransitionVolume == null){return 1.0f;}
            return TimeStopTransitionVolume._Indensity.value;
        }
        set{
            if(TimeStopTransitionVolume._Indensity.overrideState){
                TimeStopTransitionVolume._Indensity.SetValue(new FloatParameter(value));
                TimeStopTransitionVolume._Indensity.overrideState = true;
            }else{
                TimeStopTransitionVolume._Indensity.overrideState = true;
            }
        }
    }
    #endregion
    #region 共同看到的效果
    private ChromaticAberration chromaticAberration;
    private float chromaticAberrationIndensity{
        get{
            
            if(chromaticAberration == null){return 1.0f;}
            return chromaticAberration.intensity.value;
        }
        set{
            if(chromaticAberration.intensity.overrideState){
                chromaticAberration.intensity.SetValue(new FloatParameter(value));
                chromaticAberration.intensity.overrideState = true;
            }else{
                chromaticAberration.intensity.overrideState = true;
            }
        }
    }
    #endregion
    private FocusPower focusPower;//弃用的
    #endregion
    [Range(0,100f)]
    public float timeEnergy;
    public const float MAX_TIME_ENERGY = 100.0f;
    #region  高耦合
    [Header("没错，我打破封装啦啊哈哈哈")]
    public GameObject TimeEnergyBox;
    private Image timeEnergyBar;
    private Image timeEnergyIcon;
    #endregion
    [Tooltip("满分100分，每秒减consumeSpeed的分")]
    [SerializeField]
    private float consumeSpeed = 20.0f;
    [SerializeField]
    private float recoverSpeed = 10.0f;
    private GlobalClock VFXClock;
    private GlobalClock enemyClock;
    private GlobalClock rootClock;
    public int consumeFactor=0;//其他原因造成的能量损耗,正数=减少,负数=增加
    private float oldTimeEnergy;
    // private Tween timeStopTweenFarward;
    // private Tween timeStopTweenBackward;
    private Cinemachine.CinemachineImpulseSource MyInpulse;

    bool isStop = false;
    bool isFocusPowerOn = false;
    //hasPlayed_isStop代表Dotween动画是否播放完成
    bool hasPlayed_isStop = true;
    bool hasPlayed_FocusPowenOn = true;
    bool isPlayerDead;
    bool isGamePause = false;
    #region 调试
    public bool canPlayerAlwaysTimeStop;
    #endregion
    private void Awake() {
        if(volume==null){
            volume = GameObject.Find("Global Volume").GetComponent<Volume>();
        }
        if(volume.profile.TryGet<ScreenShockVolume>(out screenShock)){
            //Debug.Log("find screen shock volues");
        }else{
            Debug.LogError("screen shock volumes 没找到");  
        }
        if(volume.profile.TryGet<UVOffsetVolume>(out TimeStopTransitionVolume)){
            //Debug.Log("find TimeStopTransitionVolume");
        }else{
            Debug.LogError("TimeStopTransitionVolume 没找到");  
        }
        if(volume.profile.TryGet<ChromaticAberration>(out chromaticAberration)){
            //find it
        }else{
            Debug.LogError("ChromaticAberration 没找到");  
        }
        
    }
    private void OnEnable() {
        MessageCenter.AddListener(OnGameOver);
        MessageCenter.AddListener(OnGameRestart);
        MessageCenter.AddListener(OnGamePauseOn);
        MessageCenter.AddListener(OnGamePauseOff);
        MessageCenter.AddListener(OnLevelClear);
        MessageCenter.AddListener(OnMoveNextStage);
    }

    void Start()
    {
        MyInpulse = GetComponent<Cinemachine.CinemachineImpulseSource>();
        VFXClock = Timekeeper.instance.Clock("VFX");
        enemyClock = Timekeeper.instance.Clock("Enemy");
        rootClock = Timekeeper.instance.Clock("Root");
        //DOTween.To(() => value, x => value = x, 0f,2f).SetEase(Ease.InCubic);
        timeEnergy = MAX_TIME_ENERGY;
        oldTimeEnergy = timeEnergy;
        #region 高耦合
        timeEnergyBar = TimeEnergyBox.transform.Find("TEBar").GetComponent<Image>();
        timeEnergyIcon = TimeEnergyBox.transform.Find("TEIcon").GetComponent<Image>();
        #endregion
    }

    void Update()
    {
        if(isPlayerDead || isGamePause)
        {
            return;
        }
            

        #region  时间值相关
        if(isStop || isFocusPowerOn)
        {
            if (canPlayerAlwaysTimeStop)
            {
                CalcTimeEnergy(Time.deltaTime * recoverSpeed);
            }
            else
            {
                CalcTimeEnergy(-Time.deltaTime * (1.0f + consumeFactor) * consumeSpeed);
            }
        }
        else
        {
            if (canPlayerAlwaysTimeStop)
            {
                CalcTimeEnergy(Time.deltaTime * recoverSpeed);
            }
            else
            {
                CalcTimeEnergy(Time.deltaTime * (recoverSpeed - consumeFactor * consumeSpeed));
            }           
        }
        #endregion

        #region 时间控制逻辑相关
        //如果时间值不够自动退出时间控制
        //---------------------------
        if(timeEnergy <= 0f)
        {
            return;
        }
        if(Input.GetKeyDown(timeStopKey) && !isPlayerDead)
        {
            if(!isFocusPowerOn)
                if(isStop && hasPlayed_isStop)//hasPlayed_isStop代表Dotween动画是否播放完成
                {//取消时停
                    hasPlayed_isStop = false;
                    DOTween.To(() => VFXClock.localTimeScale, x => VFXClock.localTimeScale = x, 1f,1f).SetEase(Ease.OutSine).OnComplete(()=>
                    {
                        hasPlayed_isStop = true;
                    });
                    DOTween.To(() => screenShockRadius, x => screenShockRadius = x, 0f,.5f).SetEase(Ease.InCubic).OnComplete(()=>{
                        screenShockIndensity = 0;
                    });
                    ResetTimeControlAllShitOfValues();
                }
                else if (!isStop && hasPlayed_isStop)
                {//开启时停
                    if(timeEnergy >= 20.0f)
                    {
                        hasPlayed_isStop = false;
                        DOTween.To(() => VFXClock.localTimeScale, x => VFXClock.localTimeScale = x, 10E-5f,.25f).SetEase(Ease.OutSine);
                        DOTween.To(() => screenShockRadius, x => screenShockRadius = x, 1.0f,1f).SetEase(Ease.OutBack).OnComplete(()=>{
                            hasPlayed_isStop = true;
                        });
                        DOTween.To(()=>chromaticAberrationIndensity,x => chromaticAberrationIndensity = x,.4f,.5f).SetEase(Ease.OutBack);
                        TimeStopTransitionIndensity = .8f;
                        screenShockIndensity = 1.0f;
                        enemyClock.localTimeScale = 10E-3f;
                        SetTimeStop(true);
                        SetTimeEnergyIcon(true);
                    }
                    else
                        SetTimeEnergyIconRed();
                    
                }
        }
        
        // if(Input.GetKeyDown(KeyCode.CapsLock))
        // {
        //     if (isFocusPowerOn)
        //     {
        //         FocusPower();       
        //         return;
        //     }
        //     if(timeEnergy >= 20.0f)
        //         FocusPower();
        //     else
        //         SetTimeEnergyIconRed();
        // }
        #endregion
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnGameOver);
        MessageCenter.RemoveListner(OnGameRestart);
        MessageCenter.RemoveListner(OnGamePauseOn);
        MessageCenter.RemoveListner(OnGamePauseOff);
        MessageCenter.RemoveListner(OnLevelClear);
        MessageCenter.RemoveListner(OnMoveNextStage);
    }


    void FocusPower()
    {
        if(!isStop)
            if(!isFocusPowerOn && hasPlayed_FocusPowenOn)
            {
                hasPlayed_FocusPowenOn = false;
                DOTween.To(() => rootClock.localTimeScale, x => rootClock.localTimeScale = x, .2f,.5f)
                .SetEase(Ease.InOutExpo).OnComplete(()=>{
                    hasPlayed_FocusPowenOn = true;
                });
                isFocusPowerOn = true;
                focusPower.changeColor(true);
                SetTimeEnergyIcon(true);
            }
            else if(isFocusPowerOn && hasPlayed_FocusPowenOn)
            {
                hasPlayed_FocusPowenOn = false;
                DOTween.To(() => rootClock.localTimeScale, x => rootClock.localTimeScale = x, 1.0f,.5f)
                .SetEase(Ease.InOutExpo).OnComplete(()=>{
                    hasPlayed_FocusPowenOn = true;
                });
                isFocusPowerOn = false;
                focusPower.changeColor(false);
                SetTimeEnergyIcon(false);
            }
        
    }
    private void ResetTimeControlAllShitOfValues(){
        TimeStopTransitionIndensity = 0f;
        enemyClock.localTimeScale = 1.0f;
        chromaticAberrationIndensity = 0f;
        //Debug.Log("T键取消时停");
        SetTimeStop(false);
        SetTimeEnergyIcon(false);
    }
    //死亡、能量不够、通关会调用
    void ResetTimeControl()
    {
        ResetTimeControlAllShitOfValues();
        //Debug.Log("死亡取消时停");
        //下面这三个贵物不能放进去是因为它们是用了DoTween
        VFXClock.localTimeScale = 1.0f;
        screenShockRadius = 0.0f;
        screenShockIndensity = 0.0f;


        //弃用的
        if(isFocusPowerOn)
        {
            rootClock.localTimeScale = 1.0f;
            isFocusPowerOn = false;
            focusPower.changeColor(false);
        }
        
        hasPlayed_FocusPowenOn = true;
        hasPlayed_isStop = true;
    }

    #region 代码污染源
    void CalcTimeEnergy(float _value)
    {
        if(timeEnergy == 0f & _value < 0f){
            return;
        }
        timeEnergy += _value;
        if(timeEnergy >= MAX_TIME_ENERGY)
            timeEnergy = MAX_TIME_ENERGY;
        else if(timeEnergy < 0)
        {
            timeEnergy = 0;            
            ResetTimeControl();
            SetTimeEnergyIconRed();
        }
        SetTImeEnergyBar();
    }

    void SetTimeEnergy(float _value)
    {
        timeEnergy = _value;
        SetTImeEnergyBar();
    }

    void SetTImeEnergyBar()
    {
        timeEnergyBar.fillAmount = timeEnergy / MAX_TIME_ENERGY;
    }

    void SetTimeEnergyIcon(bool _isOn)
    {
        if(_isOn)
            timeEnergyIcon.DOColor(Color.yellow,0.5f);
        else
            timeEnergyIcon.DOColor(Color.white,0.5f);
    }

    void SetTimeEnergyIconRed()
    {
        timeEnergyIcon.color = Color.red;
        timeEnergyIcon.DOColor(Color.white,0.25f);
    }
    #endregion

    public void OnGameOver(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_OVER) return;
        isPlayerDead = true;
        // DOTween.CompleteAll();
        // DOTween.Clear();如果取消注释会造成重开没有淡入淡出
        ResetTimeControl();
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        isPlayerDead = false;
        SetTimeEnergy(oldTimeEnergy);
        ResetTimeControl();
        rootClock.localTimeScale = 1.0f;
    }
    public void OnGamePauseOn(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_ON) return;
        rootClock.localTimeScale =  10E-5f;
        isGamePause = true;
    }
    public void OnGamePauseOff(CommonMessage msg){
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_OFF) return;
        rootClock.localTimeScale = 1.0f;
        isGamePause = false;
    }
    public void OnMoveNextStage(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
        oldTimeEnergy = timeEnergy;
    }
    public void OnLevelClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR) return;
        ResetTimeControl();
    }
    void SetTimeStop(bool val)
    {
        if(isStop == true && val == false){//原先开启现在关闭
            SoundManager.PlayAudio("timeExit");
            if(BGMManager.isInitialize)BGMManager.Instance.TimeStopOff();
        }
        isStop = val;
        //Debug.Log("SetStop:"+val);
        if(val)
        {
            TimeStopOn();
            SoundManager.PlayAudio("timeEnter");
            if (BGMManager.isInitialize) BGMManager.Instance.TimeStopOn();
        }
        else
        {
            TimeStopOff();
        }
    }

    void TimeStopOn()
    {
        MyInpulse.GenerateImpulse(new Vector3(1f, 1f, 0)); 
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.TIME_STOP_ON
        });
    }

    void TimeStopOff()
    {   
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.TIME_STOP_OFF
        });
    }
    public void ChangeConsumeFactor(int value){
        consumeFactor += value;
    }

}
