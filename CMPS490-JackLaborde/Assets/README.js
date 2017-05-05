/*
The "Test" scene has the shooter firing the grappling hook and getting (quickly) "pulled"
to the platform above.  Actually it is just being lerped to the grappling hooks position. 
And because it is going so fast it actually goes through the platforms geometry. 

"Test2" is just a simple pendulum setup.

The "Test3" might look light the grappling hook is pulling the shooter, but really I'm
just using addForce, and the shooting and lineRenderer script from above.  The
addForce's on the shooter is basically doing the same thing that Super Mario did when
he was swimming.
*/