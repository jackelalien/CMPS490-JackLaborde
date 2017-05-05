using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VR_Object : MonoBehaviour {

	public enum AllowedController
	{
		Both,
		Left_Only,
		Right_Only
	}

	public enum ItemType
	{
		Grappling_Hook,
		Melee_Weapon,
		Combat_Stick,
		Ranged_Weapon,
		Grenade
	}

	//For Interaction Purposes:
	public bool disableWhenIdle = true;

	[Header("Touch Options", order = 1)]
	public Color HighlightColor = Color.clear;
	public AllowedController allowedControllers = AllowedController.Both;

	[Header("Grab Options", order = 2)]
	public bool isGrabbable = true;
	public bool holdToGrab = false;
	public AllowedController allowedGrabControllers = AllowedController.Both;
	//Base Attach and Grab Scripts?
	public bool isClimbable = false;
	public bool isWeapon = false;

	[Header("Weapon Settings", order = 3)]
	public Transform snapPosition;
	public Transform rotationPosition;

	[Header("Use Options", order = 4)]
	public bool canUse = false;
	public bool holdUse = false;
	public bool useOnlyIfGrabbed = true;
	public bool pointerStartsUse = false; //So that the pointer activates the use function
	//Possible Override Button Here?
	public AllowedController allowedUseControllers = AllowedController.Both;

	[Header("Specific Options", order = 5)]
	public ItemType itemType;
	public GameObject self;

	private bool isGrabbed;
	private bool wasGrabbed = false;
	private FixedJoint fixedJ;
	int timer = 0;

	void Awake()
	{

		if (disableWhenIdle && enabled) {
			enabled = false;
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (wasGrabbed && timer < 100)
			timer++;


		if (timer >= 100) {
			wasGrabbed = false;
			timer = 0;
		}
	}

	//Settings
	private void ToggleEnableState(bool state)
	{
		if (disableWhenIdle)
		{
			enabled = state;
		}
	}

	public bool IsGrabbed()
	{
		return isGrabbed;
	}

	public void ToggleGrab(bool state)
	{
		isGrabbed = state;

		if (!state)
			wasGrabbed = true;
		else {
			if (GetComponent<FixedJoint> ()) {
				fixedJ = null;
				GetComponent<FixedJoint> ().connectedBody = null;
				Destroy (GetComponent<FixedJoint> ());

			}
		}
	}

	public void ItemScript()
	{
		switch (itemType) {
		case ItemType.Grappling_Hook:
			self.GetComponent<GrapplingHook> ().ShootGrapple ();
			break;
		case ItemType.Grenade:
			break;
		case ItemType.Melee_Weapon:
			break;
		case ItemType.Ranged_Weapon:
			break;
		default:
			break;
		}
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag.Equals ("Belt") && wasGrabbed) {
			if (!isGrabbed && fixedJ == null) {
				var joint = AddFixedJoint ();
				joint.connectedBody = other.gameObject.GetComponent<Rigidbody> ();
				fixedJ = joint;
			} else {
				
			}
		}
	}

		private FixedJoint AddFixedJoint()
		{
			FixedJoint fx = gameObject.AddComponent<FixedJoint> ();
			fx.breakForce = 20000;
			fx.breakTorque = 20000;
			return fx;
		}
}
