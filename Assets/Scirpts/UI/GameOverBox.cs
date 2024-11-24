using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverBox : MonoBehaviour
{
    private CanvasGroup group;
    private void OnEnable() {
        MessageCenter.AddListener(OnGameRestart);
        MessageCenter.AddListener(OnGameOver);
    }
    // Start is called before the first frame update
    void Start()
    {
        group = GetComponent<CanvasGroup>();
    }

    private void OnDisable() {
        MessageCenter.RemoveListner(OnGameRestart);
        MessageCenter.RemoveListner(OnGameOver); 
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        group.alpha = 0;

    }
    public void OnGameOver(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_OVER) return;
        group.alpha = 1;
    }
}
