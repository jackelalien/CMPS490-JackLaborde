using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionStopper : MonoBehaviour {

	public GameObject player;
	public float speed;
	private bool moving;
	public bool finishedShot;
	public bool lineRemoved;
	public bool isShooting;
	// Use this for initialization
	void Start () {
		lineRemoved = false;
		isShooting = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (moving) {
			//Forces the shooter to reel up to the grappling hook.

			float step = speed * Time.deltaTime;
			//player.transform.position = Vector3.MoveTowards (player.transform.position, transform.position, step);  
			moving = false;
		}
	}

	void OnCollisionEnter(Collision collision)
	{
		//Turn off Grappling hook's gravity to make it stick
		if (isShooting) {
			isShooting = false;
			GetComponent<Rigidbody> ().isKinematic = true;
			finishedShot = true;

			StartCoroutine (wait ());

			RemoveLine ();
		}
		//Destroy (this);

	}

	IEnumerator wait()
	{
		Debug.Log ("Called");
		yield return new WaitForSeconds(5);
	}

	public void Finished()
	{
		finishedShot = false;
	}

	public void RemoveLine()
	{
		lineRemoved = true;
	}

	public void Shoot()
	{
		isShooting = true;
	}
}
