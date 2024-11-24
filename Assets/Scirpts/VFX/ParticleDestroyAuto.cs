using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDestroyAuto : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        //在这里写粒子播放结束时要做的事情
        Destroy(gameObject);
    }
}
