using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class myEventPlayer : MonoBehaviour
{
    [TextArea(1,3)]
    public string testTag;
    private PlayableDirector director;
    private PlayableAsset curPlayableAsset;
    private PlayableAsset nextPlayableAssset;
    // Start is called before the first frame update
    private void Awake() {
        director = GetComponent<PlayableDirector>();
    }

    public void StopCurrentAndPlayNext(PlayableAsset _asset)
    {
        nextPlayableAssset = _asset;
        director.Stop();
        curPlayableAsset = nextPlayableAssset;
        director.Play(curPlayableAsset);
        
    }

    public void StopPlay()
    {
        director.Stop();
        simpleGameManger.Instance.gameMode=simpleGameManger.GameMode.Dialoging;

        simpleUIManager.instance.ToggleDialogBox(false);
    }

    public void PlayCurrent()
    {
        //if(!director.playableGraph.IsPlaying())
        //{
            director.Play(curPlayableAsset);
        //}
    }

    public void SetNextPlayableAsset(PlayableAsset _next)
    {
        nextPlayableAssset = _next;
    }

    public void PlayNext()
    {
        curPlayableAsset = nextPlayableAssset;
        director.Play(curPlayableAsset);
    }

}
