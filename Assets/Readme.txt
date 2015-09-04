RUNTIME INTERFACE
L - use torch
R - generate new map
T - switch between clock and timer
S - use sonar
V - toggle player always visible

CHANGING SCENE SETTINGS
To set the light fadeout speed, tweak "Over Time Ex" on the Directional Light object.
To set the time for the light to start fading out if the player just sits there, change "Time Until Initial Fade" on the Singletons object.

To set the countdown duration on the timer, change "Countdown Duration" on the Singletons object.

To change map size (for future maps), change "Num Tiles X" and "Num Tiles Y" on the Singletons object.

To prevent walls from hiding bits of the player cube (allowing players to figure out where some walls are), change "Projection" on Main Camera to orthographic.

Wall lethality and sonar characteristics may also be changed via the Singletons object. The Enemy object is also tweakable.

MISC
Note that this project has a custom script execution order (Edit -> Project Settings -> Script Execution Order).