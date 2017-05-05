
function OnCollisionEnter(collision : Collision) {
/* when the spawned grappling hook makes contact with a rigidbody,
in this case the Ceiling ojbect, we turn off the grappling hook's gravity,
and other kinematic forces, thus "freezing" it in space making it look like it sticks*/

/*If you really want to set the anchor where the grappling hook hit
you could use the firstContactPoint variable
var firstContact = collision.contacts[0];
var firstContactPoint = firstContact.point;
*/

/* when the spawned grappling hook makes contact with a rigidbody,
in this case the Ceiling ojbect, we turn off the grappling hook's gravity,
and other kinematic forces, thus "freezing" it in space making it look like it sticks*/
GetComponent.<Rigidbody>().isKinematic = true;


/* this just forces the shooter to jump a bit, like tarzan jumping
up before swinging out off the ledge...*/
//This gets the shooter...
bob = GameObject.Find("Shooter");
//this makes him jump
bob.GetComponent.<Rigidbody>().AddForce(0,250,250);



/*this peice of code isn't working, its suppose to add a SpringJoint to the
grappling hook and connect it to the shooter, it's having no effect.  To
judge for yourself you'll want to comment out the jump code above and
the transform and destroy at the bottom of the script.

gameObject.AddComponent ("SpringJoint"); 
anchor = this.transform.position;
spring = 200;
damper = 0.2;
connectedBody = bob;
minDistance = 0;
maxDistance = 0.2;
breakForce = Mathf.Infinity;
breakTorque = Mathf.Infinity;
*/




//get shooter object...
/*set the shooter object's position to this ojbect,
in this case the grappling hook's position.  Also note
it actually is being set to just above the grappling hook's position*/
//bob.transform.position = Vector3.Lerp(bob.transform.position, this.transform.position + Vector3(0,1,0), 1);

//Give time for addForce on the shooter to work it's magic
yield WaitForSeconds (1.5);
/*now we destroy this object, in this case the grappling hook as it has
been enough time for the shooter to move */
Destroy (gameObject);

}


/* I can't figure out how to prevent multiple grappling throws so
let's at least kill those that we can.
*/
function Update(){
bob = GameObject.Find("Shooter");
//If they are at the same Y height or less as the shooter, they get killed
if(this.transform.position.y<=bob.transform.position.y){
Destroy (gameObject);
}
//If they are greater than 10 units over the height of shooter, they get killed
if(this.transform.position.y>bob.transform.position.y+10){
Destroy (gameObject);
}
}

