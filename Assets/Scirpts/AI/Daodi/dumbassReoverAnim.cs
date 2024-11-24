using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dumbassReoverAnim : MonoBehaviour
{
    private Animator animator;
    int hash;
    private void Awake() {
        MessageCenter.AddListener(OnGameRestart);
    }
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        hash = animator.GetCurrentAnimatorClipInfo(0).GetHashCode();
    }

    private void OnDestroy() {
        MessageCenter.RemoveListner(OnGameRestart);
    }

    public void OnGameRestart(CommonMessage msg)
    {
        if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
        // animator.Play("idle");
        animator.Play(hash);
    }
}
