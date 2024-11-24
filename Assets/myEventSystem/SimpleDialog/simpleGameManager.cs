using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class simpleGameManager : MonoBehaviour
{
    public static simpleGameManager instance;

    public enum GameMode
    {
        Dialoging,
        DialogPause
    }

    public GameMode gameMode;
    private PlayableDirector curPlayableDirector;

    private void Awake() {
        if(instance == null)
        {
            instance = this;
        }
        else
            if(instance != this)
                Destroy(this.gameObject);
        DontDestroyOnLoad(this);

        gameMode = GameMode.Dialoging;
    }

    private void Update() {
        if(gameMode == GameMode.DialogPause)
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                ResumeTimeLIne();
            }
        }

    }

    public void PuaseTimeLine(PlayableDirector director)
    {
        curPlayableDirector = director;
        gameMode = GameMode.DialogPause;
        curPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(0d);//暂停TImeLIne播放

        simpleUIManager.instance.ToggleSpaceBar(true);
    }

    public void ResumeTimeLIne()
    {
        gameMode = GameMode.Dialoging;
        curPlayableDirector.playableGraph.GetRootPlayable(0).SetSpeed(1d);//暂停TImeLIne播放

        simpleUIManager.instance.ToggleSpaceBar(false);
    }

    // void ChangeCurrentPlayMovie()
    // {
    //     curPlayableDirector.playableAsset = aa;
    //     curPlayableDirector.Play();
    // }
}
