using Cinemachine;
using UnityEngine;

public class ShakeController : SingleTon<ShakeController>
{
    private CinemachineImpulseSource source;

    private void Start()
    {
        source = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(Vector3 velocity)
    {
        source.GenerateImpulse(velocity.normalized / 10);
    }
}
