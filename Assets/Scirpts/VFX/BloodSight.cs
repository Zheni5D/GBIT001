using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BloodSight : MonoBehaviour {

	public const int CAPACITY = 200;
	public const int MAX_STAGE_CNT = 6;
	private static Transform decalRoot;

	private static int index;
	private static ParticleSystem sparticleSystem;
	private static readonly ParticleDecalData[,] data = new ParticleDecalData[MAX_STAGE_CNT,CAPACITY];
	private static readonly ParticleSystem.Particle[] particles = new ParticleSystem.Particle[CAPACITY];
	private static readonly int[] allIndex = new int[MAX_STAGE_CNT];
	private static StageID stageID;
	private static int stageInt;

	private void OnEnable() {
		MessageCenter.AddListener(OnGameRestart);
		MessageCenter.AddListener(OnMovNexStage);
	}

	private void Awake() {
		decalRoot = transform;
		sparticleSystem = GetComponent<ParticleSystem>();
		for (int i = 0; i < MAX_STAGE_CNT; i++)
		{
			for (int j = 0; j < CAPACITY; j++) 
				data[i,j] = new ParticleDecalData();
			allIndex[i] = 0;
		}
		index = allIndex[0];
		stageInt = (int)stageID;
		//Debug.Log(data.Length);	=1200
	}

	private void OnDisable() {
		MessageCenter.RemoveListner(OnGameRestart);
		MessageCenter.RemoveListner(OnMovNexStage);
	}

	public static void OnParticleHit(ParticleCollisionEvent @event, float size, Color color) {
		SetParticleData(@event, size, color);
	}

	private static void SetParticleData(ParticleCollisionEvent @event, float size, Color color) {
		if (index >= CAPACITY) index = 0;
		ParticleDecalData data = BloodSight.data[stageInt,index];
		Vector3 euler = Quaternion.LookRotation(@event.normal).eulerAngles;
		euler.z = Random.Range(0, 360);
		data.position = @event.intersection;
		data.rotation = euler;
		data.size = size;
		data.color = color;

		index++;                 
	}

	public static void DisplayParticles() {
		for (int i = 0, l = data.GetLength(1); i < l; i++) {
			ParticleDecalData data = BloodSight.data[stageInt,i];
			particles[i].position = data.position;
			particles[i].rotation3D = data.rotation;
			particles[i].startSize = data.size;
			particles[i].startColor = data.color;
		}
		
		sparticleSystem.SetParticles(particles, CAPACITY);
	}

	void OnGameRestart(CommonMessage msg)
	{
		if(msg.Mid != (int)MESSAGE_TYPE.GAME_RESTART) return;
		sparticleSystem.Clear();
		index = 0;
		//Array.Clear(data,stageInt,CAPACITY);
		for (int i = 0; i < CAPACITY; i++) data[stageInt,i] = new ParticleDecalData();
	}

	public void OnMovNexStage(CommonMessage msg)
	{
		if(msg.Mid != (int)MESSAGE_TYPE.MOV_nSTAGE) return;
		//记录index并更新stageID
		allIndex[stageInt] = index;
		stageID = (StageID)msg.intParam;
		stageInt = (int)stageID;
		index = allIndex[stageInt];
		DisplayParticles();
	}

}

[Serializable]
public class ParticleDecalData {

	public float size;
	public Vector3 position;
	public Vector3 rotation;
	[ColorUsage(true,true)]public Color color;
}