using UnityEngine;
using UnityEngine.Playables;

public class simpleGameManger : SingleTon<simpleGameManger>
{
    private bool isGameOver;
    public enum GameMode
    {
        Dialoging,
        DialogPause
    }
    public GameMode gameMode = GameMode.Dialoging;
    private PlayableDirector curPlayableDirector;
    // Start is called before the first frame update
    void OnEnable()
    {
        MessageCenter.AddListener(OnGameOver);
    }

    // Update is called once per frame
    void Update()
    {
        if(isGameOver)
        {
            if(Input.GetKeyDown(KeyCode.R))
            {
                GameRestart();
            }
        }
        if(Input.GetKeyDown(KeyCode.Y))
        {
            if(LevelManager.isInitialize)
            {
                LevelClear();
                LevelManager.Instance.Async_LoadNextLevel();
            }
        }
        if(gameMode == GameMode.DialogPause)
        {
            if(Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
            {
                ResumeTimeLIne();
            }
        }
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnGameOver);
    }

    public void GameRestart()
    {
        isGameOver = false;
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.GAME_RESTART,
            intParam = LevelObject.curStageIndex
        });
    }

    void OnGameOver(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_OVER) return;
        isGameOver = true;
    }

    //这一关卡清除
    void LevelClear()
    {
        MessageCenter.SendMessage(new CommonMessage()
        {
            Mid = (int)MESSAGE_TYPE.LEVEL_CLEAR
        });
    }
    //这一小节清除
    void StageClear()
    {

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
}
