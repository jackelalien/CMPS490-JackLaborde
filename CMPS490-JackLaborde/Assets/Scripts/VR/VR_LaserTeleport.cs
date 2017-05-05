using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_LaserTeleport : MonoBehaviour {

	private SteamVR_TrackedObject tracked;
	public GameObject laserPrefab;
	private GameObject laser;
	private Transform laserTransform;
	private Vector3 hitPoint;

	//Teleport Vars
	public Transform cameraRigTransform; //HeadSet
	public GameObject teleportReticlePrefab;
	private GameObject reticle;
	private Transform teleportReticleTransform;
	public Transform headTransform;
	public Vector3 teleportReticleOffset;

	private bool shouldTeleport;

	//Advanced Options
	[Header("Advanced Options")]
	public float blinkTransitionSpeed = 0.6f;
	[Range(0f, 32f)]
	public float distanceBlinkDelay = 0f; //Range determines how long blink transition will stay blacked out depending on the distane being teleported. Value of 0 will not delay the blink effect over any distance. 
	//public bool headsetPositionCompensation = true;
	public LayerMask teleportMask;
	public float navMeshLimitDistance = 0f; //Limits range outide the nav mesh to be considered. 0 ignores this. 

	private bool Teleporting;
	private bool teleported;
	protected bool adjustYForTerrain = false;
	public bool enableTeleport = true;

	private float blinkPause = 0f;
	private float fadeInTime = 0f;
	private float maxBlinkTransitionSpeed = 1.5f;
	private float maxBlinkDistance = 33f;


	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input ((int)tracked.index); }
	}

	void Awake()
	{
		tracked = GetComponent<SteamVR_TrackedObject> ();
	}
	// Use this for initialization
	void Start () {
		laser = Instantiate (laserPrefab);
		laserTransform = laser.transform;
		reticle = Instantiate (teleportReticlePrefab);
		teleportReticleTransform = reticle.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (Controller.GetPress (SteamVR_Controller.ButtonMask.Touchpad)) {
			RaycastHit hit;
			if (Physics.Raycast (tracked.transform.position, transform.forward, out hit, 100, teleportMask)) {
				hitPoint = hit.point;
				ShowLaser (hit);
				reticle.SetActive (true);
				teleportReticleTransform.position = hitPoint + teleportReticleOffset;
				shouldTeleport = true;
			}
		} else {
			laser.SetActive (false);
			reticle.SetActive (false);
		}

		if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Touchpad) && shouldTeleport) {
			Teleport ();
		}
	}

	//Show The Laser
	private void ShowLaser(RaycastHit hit)
	{
		laser.SetActive (true);
		laserTransform.position = Vector3.Lerp (tracked.transform.position, hitPoint, .5f);
		laserTransform.LookAt (hitPoint);
		laserTransform.localScale = new Vector3 (laserTransform.localScale.x, laserTransform.localScale.y, hit.distance);
	}

	//Teleport Functions
	private void Blink(float transSpeed)
	{
		fadeInTime = transSpeed;
		if (transSpeed > 0f) {
			//TODO: HeadSet Fade
			VRTK.VRTK_SDK_Bridge.HeadsetFade(Color.black, 0);
		}
		ReleaseBlink ();
	}

	private void ReleaseBlink()
	{
		//Headset Fade
		VRTK.VRTK_SDK_Bridge.HeadsetFade(Color.clear, fadeInTime);
		fadeInTime = 0f;
	}

	private void CalculateBlinkDelay(float blinkSpeed, Vector3 newPosition)
	{
		blinkPause = 0f;
		if (distanceBlinkDelay > 0f)
		{
			float distance = Vector3.Distance(cameraRigTransform.position, newPosition);
			blinkPause = Mathf.Clamp((distance * blinkTransitionSpeed) / (maxBlinkDistance - distanceBlinkDelay), 0, maxBlinkTransitionSpeed);
			blinkPause = (blinkSpeed <= 0.25 ? 0f : blinkPause);
		}
	}


	private void Teleport()
	{
		if (enableTeleport) {
			shouldTeleport = false;
			reticle.SetActive (false);
			Vector3 difference = cameraRigTransform.position - headTransform.position;
			difference.y = 0;
			cameraRigTransform.position = hitPoint + difference;
		}

	}
}
