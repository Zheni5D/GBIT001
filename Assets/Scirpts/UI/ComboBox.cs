using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ComboBox : BaseBehavior
{
    public int score {get; private set;}
    private Image ComboTimerBar;
    private TMP_Text ComboText;
    private Animator comboTextAnim;
    [SerializeField]
    private float comboExpiredTime;
    private float lastComboTime;
    [SerializeField]private int comboCount;
    private int savedComboCount;
    private float savedFillAmount;
    private string savedText;
    private float savedDeadTime;
    private bool isGamePause;
    private float localTime;
    public const int FURY_COMBO_MINIMUM_LIMIT = 5;
    private bool isFuryMode = false;
    private float comboLossTime = 2f; //指定受伤一次损失的连击时间
    #region 实现损失连击时间用
    private float comboLossTimeFactor = 1f; //speed会在一瞬间(lossComboPeriod的时间内)让percent减去 comboLossTime/comboExpiredTime的值
    private float comboLossTimer = 0f;
    private const float lossComboPeriod = 0.01f;
    private bool comboLossBegin = false;
    #endregion

    private void Start() {
        ComboTimerBar = GetComponentInChildren<Image>();
        ComboText = GetComponentInChildren<TMP_Text>();
        comboTextAnim = ComboText.gameObject.GetComponent<Animator>();

        comboCount = 0;
        //score = 0;
        ComboText.text = " ";
        ComboTimerBar.fillAmount = 0f;

        savedComboCount = comboCount;
        savedFillAmount = ComboTimerBar.fillAmount;
        savedText = ComboText.text;
        savedDeadTime = 0;
    }

    private void OnEnable() {
        MessageCenter.AddListener(OnGamePauseOn);
        MessageCenter.AddListener(OnGamePauseOff);
        MessageCenter.AddListener(OnKillEnemy);
        MessageCenter.AddListener(OnGameRestart);
        MessageCenter.AddListener(OnMovNextStage);
        MessageCenter.AddListener(OnLevelClear);
        MessageCenter.AddListener(OnPlayerGetHurt);
    }

    private void Update() {
        if(!isGamePause){
            localTime = time.time;
        }

        if(comboLossBegin){
            if(comboLossTimer < lossComboPeriod){
                comboLossTimer += Time.deltaTime;
            }else{
                comboLossTimer = 0f;
                //call the function
                ComboLossEnd();
            }
        }
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnKillEnemy);
        MessageCenter.RemoveListner(OnGameRestart);
        MessageCenter.RemoveListner(OnMovNextStage);
        MessageCenter.RemoveListner(OnLevelClear);
        MessageCenter.RemoveListner(OnGamePauseOn);
        MessageCenter.RemoveListner(OnGamePauseOff);
        MessageCenter.RemoveListner(OnPlayerGetHurt);
    }
    #region 监听
    void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        comboCount = savedComboCount;
        //score = 0;
        ComboText.text = savedText;
        ComboTimerBar.fillAmount = savedFillAmount;
        if(comboCount >= FURY_COMBO_MINIMUM_LIMIT && !isFuryMode){
            FuryMode(true); // step in Fury Mode
        }else{
            FuryMode(false);
        }
        float dTime = time.time - savedDeadTime;
        lastComboTime += dTime;
        StopCoroutine("UpdateComboCo");
        StartCoroutine("UpdateComboSav");
    }

    void OnKillEnemy(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.ADD_SCORE) return;
        ComboLossEnd();
        //在规定时间内击杀敌人
        if(time.time <= lastComboTime + comboExpiredTime) //连击成功
        {
            comboCount++;
            StartCoroutine("UpdateComboCo");
            ComboText.text = "X " + comboCount.ToString();
            comboTextAnim.SetTrigger("trigger");
        }
        else //连击失败
        {
            comboCount=0;
            comboCount++;
            ComboTimerBar.fillAmount = 0f;
            StartCoroutine("UpdateComboCo");
            
        }
        if(comboCount >= FURY_COMBO_MINIMUM_LIMIT && !isFuryMode){
            FuryMode(true); // step in Fury Mode
        }
        lastComboTime = time.time;//IMPORTANT!!
    }
    
    public void OnLevelClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR) return;
        savedComboCount = comboCount;
    }
    public void OnMovNextStage(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
        savedComboCount = comboCount;
        savedFillAmount = ComboTimerBar.fillAmount;
        savedText = ComboText.text;
        savedDeadTime = time.time;
    }

    public void OnGamePauseOn(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_ON) return;
        isGamePause = true;   
    }
    public void OnGamePauseOff(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.PAUSE_OFF) return;
        isGamePause = false;
    }

    public void OnPlayerGetHurt(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.PLAYER_GET_HURT) return;
        ComboLossBegin();
    }

    private void ComboLossBegin(){
        if(comboLossBegin) return;
        comboLossTimeFactor = comboLossTime / Time.deltaTime;
        comboLossBegin = true;
    }
    private void ComboLossEnd(){
        if(!comboLossBegin) return;
        comboLossBegin = false;
        comboLossTimeFactor = 1f;
    }
    #endregion
    #region 事件源
    private void FuryMode(bool yes){
        isFuryMode = yes;
        if(yes){
            MessageCenter.SendMessage(new CommonMessage(){
                Mid = (int)MESSAGE_TYPE.FURY_MODE_ON,
                content = null
            });
            ComboText.color = Color.red;
        }else{
            MessageCenter.SendMessage(new CommonMessage(){
                Mid = (int)MESSAGE_TYPE.FURY_MODE_OFF,
                content = null
            });
            ComboText.color = Color.white;
        }
    }
    #endregion
    //携程模拟异步
    IEnumerator UpdateComboCo()
    {
        float percent = 1.0f;//1=100%
        int curComboCnt= comboCount;
        while(percent > 0)
        {
            if(curComboCnt == comboCount)
            {
                if(!isGamePause){
                    percent -= time.deltaTime / comboExpiredTime * comboLossTimeFactor;// / comboCount * 0.5f
                    ComboTimerBar.fillAmount = percent;
                }
            }
            else
            {
                StopCoroutine("UpdateComboCo");
                StartCoroutine("UpdateComboCo");
            }

            yield return null;
        }
        ComboText.text = " ";
        comboCount = 0;
        FuryMode(false); //Step out Fury Mode
    }
    IEnumerator UpdateComboSav()
    {
        float percent = savedFillAmount;//1=100%
        int curComboCnt= comboCount;
        while(percent > 0)
        {
            if(curComboCnt == comboCount)
            {
                percent -= time.deltaTime / comboExpiredTime;// / comboCount * 0.5f
                ComboTimerBar.fillAmount = percent;
            }
            else
            {
                StopCoroutine("UpdateComboSav");
                StartCoroutine("UpdateComboCo");
            }

            yield return null;
        }
        ComboText.text = " ";
        comboCount = 0;
        FuryMode(false);//Step out Fury Mode
    }

}
