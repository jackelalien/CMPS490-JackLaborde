using System.Collections;
using System.Collections.Generic;
using UnityEngine;

	public class GrapplingHook : MonoBehaviour {

	public GameObject gHook;
	public GameObject hookAttach;
	//Transform grapple;
	Transform initGrapple;
	public Transform direction = null;
	public float strength = 5000.0f;
	Vector3 newPos;
	Vector3 targetPosition;

	public float startWidth = 0.05f;
	public float endWidth = 0.05f;

	public LineRenderer line;
	private bool shooting;
	private bool moving;

	//Teleport Vars
	public Transform cameraRigTransform; //HeadSet
	public Transform headTransform;

	private SteamVR_TrackedObject tracked;

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
		
		line.enabled = false;
		newPos = new Vector3 (.124f + .134f,.052f + .05f, 0);
	}
	
	// Update is called once per frame
	void Update () {
		initGrapple = hookAttach.transform;
		if (shooting) {
			line.SetPosition (0, gHook.transform.position);
			line.SetPosition (1, this.gameObject.transform.position);

			if (gHook.GetComponent<CollisionStopper> ().finishedShot) {
				//gHook.GetComponent<Rigidbody> ().isKinematic = true;
	
				if(shooting)
					targetPosition = gHook.transform.position;
				shooting = false;
				gHook.GetComponent<CollisionStopper> ().Finished ();

				moving = true;
			}


		}

		if (gHook.GetComponent<CollisionStopper> ().lineRemoved) {
			line.SetVertexCount (0);
			line.GetComponent<Renderer> ().enabled = true;
			gHook.GetComponent<CollisionStopper> ().lineRemoved = false;

			gHook.transform.localPosition = initGrapple.localPosition - newPos;
			//gHook.transform.position = initGrapple.transform.position - newPos;
			gHook.transform.rotation = this.gameObject.transform.rotation;
		}

		if (moving) {
			Vector3 difference = cameraRigTransform.position - headTransform.position;
			difference.y = 0;
			cameraRigTransform.position = Vector3.MoveTowards (headTransform.position, targetPosition, 2f);

			if(Vector3.Distance(headTransform.position, targetPosition) < 3f)
			{
				moving = false;
			}
		}
	}

	public void ShootGrapple()
	{
		gHook.GetComponent<CollisionStopper> ().Shoot ();
		gHook.GetComponent<Rigidbody> ().isKinematic = false;
		gHook.GetComponent<Rigidbody> ().AddForce (transform.right * strength);
		line.enabled = true;
		line.startWidth = startWidth;
		line.endWidth = endWidth;
		line.numPositions = 2;
		line.material.color = Color.white;
		line.GetComponent<Renderer> ().enabled = true;
			//Transform InstanceGrapple = Instantiate (prefabGrapple, transform.position, Quaternion.identity);
			//InstanceGrapple.GetComponent<Rigidbody> ().AddForce(direction.forward * 3.0f);
		shooting = true;
	}

	public bool getMoving()
	{
		return moving;
	}

	public bool getShooting()
	{
		return shooting;
	}
}

