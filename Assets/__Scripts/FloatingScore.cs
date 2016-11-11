using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//an enum to track the possible states of a FloatingScore
public enum FSState{
	idle,
	pre,
	active,
	post
}

//FloatingScore can move itself on screen following a Bezier curve
public class FloatingScore : MonoBehaviour {
	public FSState state = FSState.idle;
	[SerializeField]


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
