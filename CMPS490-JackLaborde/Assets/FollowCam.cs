using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

	public GameObject self;
	public GameObject followingObject;
	public GameObject secondFollowing;
	public Vector3 offset;
	public float dist;

	private Transform starting;
	// Use this for initialization
	void Start () {
		starting = self.transform;
	}

	// Update is called once per frame
	void Update () {
		//self.transform.position = followingObject.transform.position + offset;
		self.transform.position = followingObject.transform.position + offset;
		self.transform.rotation = Quaternion.Euler (0, secondFollowing.transform.rotation.eulerAngles.y, 0);
		//self.transform.Rotate(0, , 0, Space.Self);
	}


}
