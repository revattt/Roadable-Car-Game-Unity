#Roadable aircraft project 
Environment is based off unity asset store.
Carcontroller script handles the drivable car and allows it to fly using F key to switch
Car asset from unity asset store
#Rope & Magnet Pickup System – Flying Car Feature

This system simulates a deployable physics based rope with a magnet at the end, allowing the flying car to pick up and drop physical objects in the world.

##Features

- **Deployable Rope Chain**  
  Press G to spawn a rope made of connected segments (rigidbodies + hinge joints) hanging from the aircraft.

- **Magnet **  
  At the end of the rope is a magnet that detects and attaches to objects tagged Pickup using a fixed joint.

- **Retract System**  
  Press H to fully retract the rope and remove the magnet.

- **Pickup & Drop**  
  If the magnet is holding an object, press J to drop it. This breaks the joint and detaches the object from the magnet.

AI Aircraft that follows when pressed K
Simple AI that follows based on distance difference between target and its transform.

Added a speedometer that tracks Ground and Air Speed.
