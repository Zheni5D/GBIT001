using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombBox : Breakable
{
    private GameObject ExplosionPrefab;
    protected override void Start()
    {
        base.Start();
        ExplosionPrefab = TheShitOfReference.AllShitOfReference.CirCleExplison;
    }

    public void ExplodeImmidiately()
    {
        gameObject.SetActive(false);
        SoundManager.PlayAudio(sfxName);
        HearingPoster.PostVoice(transform.position, soundRadius);
        OnBreak();
    }

    protected override void OnBreak()
    {
        Instantiate(ExplosionPrefab, transform.position, Quaternion.identity).GetComponent<CircleReferent>().SetDelay(0.2f);
    }
}
