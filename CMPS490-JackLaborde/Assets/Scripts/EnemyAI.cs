using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {

	private NavMeshAgent nav;
	public Transform playerTransform;
	private Transform startingTrans;
	int count;
	public bool isHit = false;
	public int health = 100;
	IEnumerator myRout;

	// Use this for initialization
	void Start () {
		nav = GetComponent<NavMeshAgent>();
		nav.destination = playerTransform.transform.position;
		count = 0;
		startingTrans = this.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
		if (count % 10 == 0 && !isHit) {
			nav.destination = playerTransform.transform.position;
			count = 0;
		}

		if (isHit) {
			//Get up after falling for about 3 seconds.
			if (myRout == null)
				StartCoroutine (WaitTime ());
			else
				StopCoroutine (myRout);

			isHit = false;
			health -= 10;
			Debug.Log (health);
		}

		if (health <= 0) {
			//Play Death Anim
			Destroy (this.gameObject);
		}
		
	}

	IEnumerator WaitTime()
	{
		yield return new WaitForSeconds (3);
		//Get Up to original rotation
		float lerpTime = 2f;
		float StartTime = Time.time;
		float EndTime = StartTime + lerpTime;
		transform.rotation = Quaternion.Lerp(transform.rotation, startingTrans.rotation, Time.deltaTime * 2f); 
		this.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.FreezeAll;
		this.gameObject.GetComponent<NavMeshAgent>().enabled = true;
		isHit = false;

	}

	//Combat Collisions
	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.tag == "Weapon" && col.gameObject.GetComponent<VR_Object>().IsGrabbed()) {
			//Debug.Log ("Hit Enemy!");
			//Turn off Enemy's collider, then tell his AI he's hit.
			//Use Velocity / Mass to get hit power.
			Vector3 force = col.rigidbody.velocity;
			nav.enabled = false;
			isHit = true;
			this.gameObject.GetComponent<Rigidbody> ().constraints = RigidbodyConstraints.None;
			//this.gameObject.GetComponent<Rigidbody> ().AddForce (force);




		}

		//Debug.Log ("Colliding i guess");

	}

	//AI Will Feature:
	/* - Check if player is disguised or not. If disguised, wait until player is close and can be seen
	 * - Check if player is cloaked. Will not be seen.
	 * - Check if player is in view. Chase if so.
	 * - If player is in range, attack the player (give a visual effect)
	 * - Go flying when player hits him.
	 * /

	/*
	AI Combat:
	- If player hits enemy, turn off NavMeshAgent, Activate Rigidbody Non-Kinematic.
	- This makes enemy fly back.



	*/
}
