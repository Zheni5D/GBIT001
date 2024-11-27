using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    public string sfxName = "doorBroken";
    public int maxHp = 3;
    protected int currentHp;
    protected bool getHurtThisFrame;
    protected Split split;
    [SerializeField]protected float soundRadius;

    protected virtual void Start()
    {
        currentHp = maxHp;
        split = GetComponent<Split>();
    }

    protected virtual void Awake()
    {
        MessageCenter.AddListener(OnGameRestart);
    }

    protected virtual void OnDestroy()
    {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    protected void Update()
    {
        if (getHurtThisFrame)
        {
            getHurtThisFrame = false;
        }
    }

    public virtual void GetHurt(Vector3 murderPoint)
    {
        if (!getHurtThisFrame)
        {
            currentHp--;
            getHurtThisFrame = true;
        }

        if (currentHp == 0)
        {
            gameObject.SetActive(false);
            if(split != null) split.Trigger(murderPoint);
            SoundManager.PlayAudio(sfxName);
            // ???完??+?????完
            HearingPoster.PostVoice(transform.position, soundRadius);
            OnBreak();
        }
    }

    public virtual void GetHurt(Vector3 murderPoint,Vector3 collidePoint)//?????GetHurt??????,????????????????
    {
        if (!getHurtThisFrame)
        {
            currentHp--;
            getHurtThisFrame = true;
        }

        if (currentHp == 0)
        {
            gameObject.SetActive(false);
            if(split != null) split.Trigger(murderPoint);
            SoundManager.PlayAudio(sfxName);
            HearingPoster.PostVoice(transform.position, soundRadius);
            OnBreak();
            // ???完??+?????完
        }
    }

    protected virtual void OnBreak()
    {
        
    }

    public virtual void OnGameRestart(CommonMessage msg)
    {
        if (msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        gameObject.SetActive(true);
        currentHp = maxHp;
    }
}