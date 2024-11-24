using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextLevelArea : MonoBehaviour
{
    private SpriteRenderer sprite;
    private void OnEnable() {
        MessageCenter.AddListener(OnLevelClear);
    }
    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
    }
    private void OnDisable() {
        MessageCenter.RemoveListner(OnLevelClear);
    }

    public void OnLevelClear(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.LEVEL_CLEAR) return;
        sprite.color = new Color(1f,0.92f,0.016f,sprite.color.a);
        GetComponent<Collider2D>().enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.CompareTag("Player"))
        {
            if(LevelManager.isInitialize)
            {
                if (LevelManager.Instance.enableReplay && ReplaySystem.isReplayFin)
                {
                    LevelManager.Instance.Async_LoadNextLevel();
                }
                else
                {
                    LevelManager.Instance.Async_LoadNextLevel();
                }
            }
        }
    }
}
