using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class VR_GrabObject : MonoBehaviour {
	//Also includes combat handling

	private SteamVR_TrackedObject tracked;
	private GameObject collidingObject; //TODO: Check if of type VR_Object, so that we can use different interactions
	private GameObject objectInHand;
	public GameObject ControllerGameObject;
	public GameObject UtilityBelt;

	//Climbing Mechanics
	public Transform playArea;
	public PlayerController player;
	private Vector3 startControllerPosition;
	private Vector3 startPosition;
	private GameObject grabbingController;
	private GameObject climbingObject;
	private bool isClimbing = false;
	private int checkMe = 0;

	public bool holdGrab = false;
	private bool holding = false;
	private static bool otherControllerInUse;
	private bool thisControllerInUse;

	public enum GrabButtons
	{
		Trigger,
		Grip
	}

	public GrabButtons grabbingButtons = GrabButtons.Grip;
	public GrabButtons useButtons = GrabButtons.Trigger;

	private SteamVR_Controller.Device Controller
	{
		get { return SteamVR_Controller.Input ((int)tracked.index); }
	}

	void Awake()
	{
		tracked = GetComponent<SteamVR_TrackedObject> ();

	}

	private void SetCollidingObject(Collider col)
	{
		if (collidingObject || !col.GetComponent<Rigidbody> ())
			return;
		collidingObject = col.gameObject;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (isClimbing) {
			playArea.position = startPosition - (GetPosition (Controller) - startControllerPosition);
		}


			if (grabbingButtons == GrabButtons.Grip) {

				if (!holdGrab) {
					if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
					if (objectInHand != null && !holding) {
						holding = true;
					}

						if (collidingObject && !holding)
							GrabObject ();
						else if (objectInHand && holding)
							ReleaseObject ();
					}
				} else {
					if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
					if (objectInHand != null && !holding) {
						holding = true;
					}
						if (collidingObject && !holding)
							GrabObject ();
					}

					if (Controller.GetPressUp (SteamVR_Controller.ButtonMask.Grip)) {
						if (objectInHand && holding)
							ReleaseObject ();
					}
				}


			} else if (grabbingButtons == GrabButtons.Trigger) {

				if (!holdGrab) {
					if (Controller.GetHairTriggerDown ()) {
						if (collidingObject && !holding)
							GrabObject ();
						else if (objectInHand && holding)
							ReleaseObject ();
					}
				} else {
					if (Controller.GetHairTriggerDown ()) {
					
						if (collidingObject)
							GrabObject ();
					
					}

					if (Controller.GetHairTriggerUp ()) {
						if (objectInHand)
							ReleaseObject ();
					}
				}

			}

		if (useButtons == GrabButtons.Trigger) {
			if (objectInHand) {
				if (Controller.GetHairTriggerDown ()) {
					ExecuteUseScript ();
				}
			}
		} else {
			if (objectInHand) {
				if (Controller.GetPressDown (SteamVR_Controller.ButtonMask.Grip)) {
					ExecuteUseScript ();
				}
			}
		}

		//Check if triggers are down but there are no colliding objects!
		if (!collidingObject && !objectInHand && Controller.GetHairTriggerDown ()) {
			//Turn the colliders to non-trigger for hand to hand combat
			//IF THIS DOESNT WORK CHANGE THE RIGIDBODY TO NON KINEMATIC?
			ControllerGameObject.GetComponent<BoxCollider>().isTrigger = false;
		}
		if (!collidingObject && !objectInHand && Controller.GetHairTriggerUp ()) {
			//Turn the colliders back to trigger
			ControllerGameObject.GetComponent<BoxCollider>().isTrigger = true;
		}




	}

	public void OnTriggerEnter(Collider other)
	{
		SetCollidingObject (other);
	}
		

	public void OnTriggerStay(Collider other)
	{
		SetCollidingObject (other);
	}

	public void OnTriggerExit(Collider other)
	{
		if (!collidingObject)
			return;

		if(!isClimbing)
			collidingObject = null;
	}

	//Handle Combat Collisions
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Enemy") {
			//Debug.Log ("Hit Enemy!");
			//Turn off Enemy's collider, then tell his AI he's hit.
			//Use Velocity / Mass to get hit power.
			Vector3 force = Controller.velocity;


			
			col.gameObject.GetComponent<NavMeshAgent> ().enabled = false;
			col.gameObject.GetComponent<EnemyAI> ().isHit = true;
			col.rigidbody.constraints = RigidbodyConstraints.None;


			col.rigidbody.AddForce (force);

			StartCoroutine (Vibrate ());



		}

		//Debug.Log ("Colliding i guess");
			
	}

	void OnCollisionStay(Collision col)
	{
		if (col.gameObject.tag == "Enemy") {
			//Debug.Log ("Hit Enemy!");
			//Turn off Enemy's collider, then tell his AI he's hit.
			//Use Velocity / Mass to get hit power.
			Vector3 force = Controller.velocity;



			col.gameObject.GetComponent<NavMeshAgent> ().enabled = false;
			col.gameObject.GetComponent<EnemyAI> ().isHit = true;
			col.rigidbody.constraints = RigidbodyConstraints.None;


			col.rigidbody.AddForce (force);

			StartCoroutine (Vibrate ());



		}

	}

	private void GrabObject()
	{
				if (IsClimbableObject(collidingObject)) {
			player.enableBodyCollisions = false;
			player.ToggleOnGround (false);
			//Prevent Snap to Floor
			isClimbing = true;
			climbingObject = collidingObject;

			startControllerPosition = GetPosition (Controller);
			startPosition = playArea.position;


			objectInHand = collidingObject;
			collidingObject = null;
			holding = true;

		} else {
			ObjectIsGrabbed (collidingObject);
			objectInHand = collidingObject;
			collidingObject = null;
			holding = true;

			var joint = AddFixedJoint ();
			joint.connectedBody = objectInHand.GetComponent<Rigidbody> ();


		}

	}

	IEnumerator Vibrate()
	{
		for (int i = 0; i < 100; i++)
			Controller.TriggerHapticPulse (500);
		yield return new WaitForSeconds (3);
	}

	private FixedJoint AddFixedJoint()
	{
		FixedJoint fx = gameObject.AddComponent<FixedJoint> ();
		fx.breakForce = 50000;
		fx.breakTorque = 50000;
		return fx;
	}

	private void ReleaseObject()
	{
		if (IsClimbableObject (objectInHand)) {
			
			//Prevent Snap to Floor FALSE
			player.enableBodyCollisions = true;

			Vector3 velocity = Vector3.zero;

			velocity = -Controller.velocity;
			velocity = Vector3.Scale (velocity, playArea.localScale);

			//Apply Body Velocity?
			player.ApplyBodyVelocity(velocity, true);
			isClimbing = false;
			grabbingController = null;
			climbingObject = null;
			holding = false;


		} else {
			ObjectIsReleased (objectInHand);
			holding = false;
			if (GetComponent<FixedJoint> ()) {
				GetComponent<FixedJoint> ().connectedBody = null;
				Destroy (GetComponent<FixedJoint> ());

				objectInHand.GetComponent<Rigidbody> ().velocity = Controller.velocity;
				objectInHand.GetComponent<Rigidbody> ().angularVelocity = Controller.angularVelocity;

			}
		}
		objectInHand = null;
	}

	//Climbing Mechs
	private Vector3 GetPosition(SteamVR_Controller.Device control)
	{
		return playArea.localRotation * Vector3.Scale(control.transform.pos, playArea.localScale);

		//return playArea.localRotation * objTransform.localPosition;
	}

	private bool IsClimbableObject(GameObject obj)
	{
		var interact = obj.GetComponent<VR_Object> ();
		return (interact && interact.isClimbable);
	}

	private bool ObjectIsGrabbable(GameObject obj)
	{
		var interact = obj.GetComponent<VR_Object> ();
		return (interact && interact.isGrabbable);
	}

	private void ObjectIsGrabbed(GameObject obj)
	{
		var interact = obj.GetComponent<VR_Object> ();
		if (interact) {
			interact.ToggleGrab (true);
		}
	}

	private void ObjectIsReleased(GameObject obj)
	{
		var interact = obj.GetComponent<VR_Object> ();
		if (interact) {
			interact.ToggleGrab (false);
		}
	}

	//////////Use Scripts
	private void ExecuteUseScript()
	{
		if (objectInHand != null) {
			objectInHand.GetComponent<VR_Object> ().ItemScript ();
		}
	}
		

		
}
