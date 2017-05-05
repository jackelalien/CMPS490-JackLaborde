using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Climbing : MonoBehaviour {

	private SteamVR_TrackedObject tracked;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input ((int)tracked.index); }
	}

	public Transform playArea;

	private Vector3 startControllerPosition;
	private Vector3 startPosition;
	private GameObject grabbingController;
	private GameObject climbingObject;
	private PlayerController bodyPhysics;
	private bool isClimbing = false;



	void Awake()
	{
		tracked = GetComponent<SteamVR_TrackedObject> ();
		bodyPhysics = GetComponent<PlayerController> ();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}




}
