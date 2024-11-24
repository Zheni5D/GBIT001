using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable,VolumeComponentMenu("TestPostProcessing/Test")]
public class TestPostVolume : VolumeComponent
{
    public ColorParameter changeColor = new ColorParameter(Color.white,true);
    public ClampedFloatParameter Intensity = new ClampedFloatParameter(1.0f,0.1f,5.0f);//强度值限定在了0.1-5.0
}