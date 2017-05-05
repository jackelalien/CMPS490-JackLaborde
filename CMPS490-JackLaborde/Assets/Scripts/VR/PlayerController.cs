using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	private SteamVR_TrackedObject tracked;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input ((int)tracked.index); }
	}

	public enum FallingRestrictions
	{
		None,
		EitherController,
		BothControllers
	}

	public GameObject controllerLeft;
	public GameObject controllerRight;

	//Body Collision Settings
	[Header("Body Collision Settings")]
	public bool enableBodyCollisions = true;
	public bool ignoreGrabbedCollisions = true;
	public float headsetYOffset = 0.2f;
	public float movementThreshold = 0.0015f; 
	public int standingHistorySamples = 5;
	public float leanYThreshold = 0.5f;
	public LayerMask layer;

	[Header("Snap to Floor Settings")]
	public LayerMask layersToIgnore = Physics.IgnoreRaycastLayer; //Layers to ignore when raycasting to find floors.
	public FallingRestrictions fallRestrictor = FallingRestrictions.None;
	public float gravityFallThreshold = 1.0f;
	public float blinkYThreshold = 0.15f;
	public float floorHeightTolerance = 0.001f;

	//Transforms for Entire Play Area and Just the headset
	public Transform playArea;
	public Transform headset;

	private CapsuleCollider bodyCollider;
	private bool currentBodyCollisionsSetting;
	private Rigidbody bodyRigidbody;
	private GameObject currentCollidingObject = null;
	private GameObject currentValidFloorObject = null;

	private VR_LaserTeleport teleporter;
	private float lastFrameFloorY;
	private float hitFloorYDelta = 0f;
	private bool initialFloorDrop = false;
	private bool resetPhysicsAfterTeleport = false;
	private bool storedCurrentPhysics = false;
	private bool retogglePhysicsOnCanFall = false;
	private bool storedRetogglePhysics;
	private Vector3 lastPlayAreaPosition = Vector3.zero;
	private Vector2 currentStandingPosition;
	private List<Vector2> standingPositionHistory = new List<Vector2>();
	private float playAreaHeightAdjustment = 0.009f;

	private bool isFalling = false;
	private bool isMoving = false;
	private bool isLeaning = false;
	private bool onGround = true;
	private bool preventSnapToFloor = false;
	private bool generateCollider = false;
	private bool generateRigidbody = false;
	private float counter = 1f;

	public GameObject grapplingHook;

	public List<GameObject> enemyList;

	void Awake()
	{
		tracked = GetComponent<SteamVR_TrackedObject> ();
		teleporter = GetComponent<VR_LaserTeleport> ();
	}

	// Use this for initialization
	void Start () {
		foreach (GameObject item in enemyList) {
			item.GetComponent<EnemyAI> ().enabled = false;
		}
			
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Fall ();
		CheckBodyCollisionsSetting ();

		lastPlayAreaPosition = (playArea ? playArea.position : Vector3.zero);

		//Update Collider?
	}

	void Fall()
	{
		float distance = 1f;
		float maxDistance = 2.6f;
		Vector3 direction = new Vector3 (0, -1, 0);
		float step = 5f * Time.deltaTime;


		if(!onGround) {
			if (Physics.Raycast (playArea.transform.position, Vector3.up * -1, distance, layer) && Physics.Raycast (headset.transform.position, Vector3.up * -1, maxDistance, layer)) {
				onGround = true;
				counter = 1f;
			} else {
				counter++;
				if (counter > 5f)
					counter = 5f;
				playArea.transform.position = Vector3.MoveTowards (playArea.transform.position, new Vector3 (playArea.transform.position.x, -1, playArea.transform.position.z), step * counter);

			}
		}

		if (!grapplingHook.GetComponent<GrapplingHook> ().getMoving ())
			CheckGround ();
	}

	bool CheckGround()
	{
		//First check if playArea is on Ground
		float distance = 1f;
		float maxDistance = 2.6f;
		Vector3 direction = new Vector3 (0, -1, 0);

		// || (Physics.Raycast (controllerLeft.transform.position, Vector3.up * -1, maxDistance) || Physics.Raycast (controllerRight.transform.position, Vector3.up * -1, maxDistance))
		// || (Physics.Raycast (controllerLeft.transform.position, Vector3.up * -1, maxDistance) || Physics.Raycast (controllerRight.transform.position, Vector3.up * -1, maxDistance))


		if (Physics.Raycast (playArea.transform.position, Vector3.up * -1, distance, layer)) {
			//Play area is on the ground, but that doesn't mean that the player is.
			if (Physics.Raycast (headset.transform.position, Vector3.up * -1, maxDistance, layer)) {
				//Player is on Ground
				onGround = true;
			} else {
				onGround = false;
			}
		}
		else
		{
			//Fall
			if (Physics.Raycast (headset.transform.position, Vector3.up * -1, maxDistance, layer) ) 
				{
					onGround = true; }
				else {
					onGround = false;
				}
			
		}

		return onGround;
	}


	//Enable Body Physics
	public bool ArePhysicsEnabled()
	{
		return(bodyRigidbody ? !bodyRigidbody.isKinematic : false);
	}

	public void ApplyBodyVelocity(Vector3 velocity, bool forcePhysicsOn = false)
	{
		if (enableBodyCollisions && forcePhysicsOn)
		{
			TogglePhysics(true);
		}

		if (ArePhysicsEnabled())
		{
			var gravityPush = -0.001f;
			var appliedGravity = new Vector3(0f, gravityPush, 0f);
			bodyRigidbody.velocity = playArea.TransformVector(velocity) + appliedGravity;
			//StartFall(currentValidFloorObject);
		}
	}

	private void CheckBodyCollisionsSetting()
	{
		if (enableBodyCollisions != currentBodyCollisionsSetting)
		{
			TogglePhysics(enableBodyCollisions);
		}
		currentBodyCollisionsSetting = enableBodyCollisions;
	}

	public void TogglePhysics(bool state)
	{
		if (bodyRigidbody)
		{
			bodyRigidbody.isKinematic = !state;
		}
		if (bodyCollider)
		{
			bodyCollider.isTrigger = !state;
		}
	}

	public void ToggleOnGround(bool val)
	{
		onGround = val;
	}

	public bool GetOnGround()
	{
		return onGround;
	}

	public bool IsFalling()
	{
		return isFalling;
	}

	public bool IsLeaning()
	{
		return isLeaning;
	}

	public bool IsMoving()
	{
		return isMoving;
	}
		
		


	//Falling Functions
	//Trackpad Motion Functions




	//COLLISIONS
	private void EnableBodyPhysics()
	{
		currentBodyCollisionsSetting = enableBodyCollisions;

		CreateCollider();
	}

	private void DisableBodyPhysics()
	{
		DestroyCollider();
	}

	private void CreateCollider()
	{
		generateCollider = false;
		generateRigidbody = false;

		if (!playArea)
		{
			Debug.LogError("No play area could be found. Have you selected a valid Boundaries SDK in the SDK Manager? If you are unsure, then click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and select a Boundaries SDK from the dropdown.");
			return;
		}

		//VRTK_PlayerObject.SetPlayerObject(playArea.gameObject, VRTK_PlayerObject.ObjectTypes.CameraRig);
		bodyRigidbody = playArea.GetComponent<Rigidbody>();
		if (bodyRigidbody == null)
		{
			generateRigidbody = true;
			bodyRigidbody = playArea.gameObject.AddComponent<Rigidbody>();
			bodyRigidbody.mass = 100f;
			bodyRigidbody.freezeRotation = true;
		}

		bodyCollider = playArea.GetComponent<CapsuleCollider>();
		if (bodyCollider == null)
		{
			generateCollider = true;
			bodyCollider = playArea.gameObject.AddComponent<CapsuleCollider>();
			bodyCollider.center = new Vector3(0f, 1f, 0f);
			bodyCollider.height = 1f;
			bodyCollider.radius = 0.15f;
		}

		if (playArea.gameObject.layer == 0)
		{
			playArea.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
		}
		TogglePhysics(enableBodyCollisions);
	}

	private void DestroyCollider()
	{
		if (generateRigidbody)
		{
			Destroy(bodyRigidbody);
		}

		if (generateCollider)
		{
			Destroy(bodyCollider);
		}
	}

	private void UpdateCollider()
	{
		if (bodyCollider)
		{
			var newpresenceColliderYSize = (headset ? headset.transform.localPosition.y - headsetYOffset : 0f);
			var newpresenceColliderYCenter = Mathf.Max((newpresenceColliderYSize / 2) + playAreaHeightAdjustment, bodyCollider.radius + playAreaHeightAdjustment);

			if (headset && bodyCollider)
			{
				bodyCollider.height = Mathf.Max(newpresenceColliderYSize, bodyCollider.radius);
				bodyCollider.center = new Vector3(headset.localPosition.x, newpresenceColliderYCenter, headset.localPosition.z);
			}
		}
	}

	private IEnumerator RestoreCollisions(GameObject obj)
	{
		yield return new WaitForEndOfFrame();
		if (obj)
		{
			var objScript = obj.GetComponent<VR_Object>();
			if (objScript && !objScript.IsGrabbed())
			{
				IgnoreCollisions(obj.GetComponentsInChildren<Collider>(), false);
			}
		}
	}

	private void IgnoreCollisions(Collider[] colliders, bool state)
	{
		if (playArea)
		{
			var collider = playArea.GetComponent<Collider>();
			if (collider.gameObject.activeInHierarchy)
			{
				foreach (var controllerCollider in colliders)
				{
					if (controllerCollider.gameObject.activeInHierarchy)
					{
						Physics.IgnoreCollision(collider, controllerCollider, state);
					}
				}
			}
		}
	}



}
