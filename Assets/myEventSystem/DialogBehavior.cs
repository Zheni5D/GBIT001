using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class DialogBehavior : PlayableBehaviour
{
    private PlayableDirector playableDirector;

    public string speakerName;
    [TextArea(8,1)]public string line;
    public int fontSize;

    private bool isClipPlayed;
    public bool requirePause;
    private bool pauseSchedule;//当前播放状态

    public override void OnPlayableCreate(Playable playable)
    {
        //                                     .GetResolver() <=> ExposedReference
        playableDirector = playable.GetGraph().GetResolver() as PlayableDirector;
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        if(isClipPlayed == false && info.weight > 0)
        {
            simpleUIManager.instance.SetTextContent(speakerName,line,fontSize);
            if(requirePause)
                pauseSchedule = true;
            isClipPlayed = true;
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        isClipPlayed = false;
        if(pauseSchedule)
        {
            pauseSchedule = false;

            simpleGameManger.Instance.PuaseTimeLine(playableDirector);
        }
        else
        {
            simpleUIManager.instance.ToggleSpaceBar(false);
        }
    }
}
