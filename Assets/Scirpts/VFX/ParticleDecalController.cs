using System.Collections.Generic;
using UnityEngine;

public class ParticleDecalController : MonoBehaviour {

	[Range(0f, 1f)]
	public float decalRate = 1f;        //留下血迹的概率
	public float minSize;               //血迹大小范围
	public float maxSize;
	[GradientUsage(true)]
	public Gradient colorGradient;      //颜色变化范围
	
	private ParticleSystem _particleSystem;
	private readonly List<ParticleCollisionEvent> _collisionEvents = new List<ParticleCollisionEvent>(4);

	private void Awake() {
		_particleSystem = GetComponent<ParticleSystem>();
		//Invoke("InObjectPool",1f);
	}
	
	private void OnParticleCollision(GameObject other) {
		int count = _particleSystem.GetCollisionEvents(other, _collisionEvents);

		for (int i = 0; i < count; i++) {
			float r = Random.Range(0f, 1f);
			if (r <= decalRate) BloodSight.OnParticleHit(_collisionEvents[i], Random.Range(minSize, maxSize), colorGradient.Evaluate(r));
		}
		
		BloodSight.DisplayParticles();
	}

	void InObjectPool()
	{
		//ObjectPool.push(this);
	}
}