// Line start width
var startWidth = 0.05;
// Line end width
var endWidth = 0.05;
//or you could use...
//var aMaterial : Material;


// get line going...
private var line : LineRenderer;

function Start ()
{
   line = this.gameObject.AddComponent(LineRenderer);
   line.SetWidth(startWidth, endWidth);
   line.SetVertexCount(2);
   line.material.color = Color.red;
//we need to see the line... 
   line.GetComponent.<Renderer>().enabled = true;
}

function Update ()
{
//get the shooter object...
bob = GameObject.Find("GrapplingGun");
//set starting point of line to this object, in this case the grappling hook prefab
   line.SetPosition(0, this.gameObject.transform.position);
//set the ending point of the line to the shooter object
   line.SetPosition(1, bob.transform.position);
}