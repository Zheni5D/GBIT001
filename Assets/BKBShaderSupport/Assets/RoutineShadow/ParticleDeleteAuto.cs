using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDeleteAuto : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        //在这里写粒子播放结束时要做的事情
        RoutineShadow.MiniPool.DeleteWindedParticle(this.gameObject);
    }
}
