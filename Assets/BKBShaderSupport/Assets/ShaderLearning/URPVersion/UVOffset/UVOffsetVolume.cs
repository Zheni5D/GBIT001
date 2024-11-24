using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable,VolumeComponentMenu("TestPostProcessing/UVOffsetVolume")]
public class UVOffsetVolume : VolumeComponent
{
    public ClampedFloatParameter _Indensity = new ClampedFloatParameter(0f,0f,2f);
}
