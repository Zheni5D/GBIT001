using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEngine.Playables;

public class EventNode : Node {
	[Input]
	public Empty enter;
	[Output]
	public Empty exit;
	public int DialogID;
	public string Name;
	public PlayableAsset timeLineAsset;

	public enum BranchType
	{
		A,B,C,D
	}
	public BranchType branch;
	// Use this for initialization
	protected override void Init() {
		base.Init();
	}

	// Return the correct value of an output port when requested
	public override object GetValue(NodePort port) {
		return null; // Replace this
	}
}