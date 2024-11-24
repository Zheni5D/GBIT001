using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
[System.Serializable,VolumeComponentMenu("TestPostProcessing/ScreenShockVolume")]
public class ScreenShockVolume : VolumeComponent
{
    public ClampedFloatParameter _Radius = new ClampedFloatParameter(1.0f,0f,1.2f);
    public ClampedFloatParameter _Aspect = new ClampedFloatParameter(1.78f,0.1f,4.0f);
    public Vector4Parameter _Center = new Vector4Parameter(new Vector4(.5f,.5f,0,0));
    public ClampedFloatParameter _Width = new ClampedFloatParameter(0.0542f,0f,0.2f);
    public ClampedFloatParameter _Indensity = new ClampedFloatParameter(1.0f,0,1.0f);
}
