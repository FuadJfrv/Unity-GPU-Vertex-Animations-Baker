https://github.com/user-attachments/assets/a5be1733-7fa7-45bd-9dbb-522f4db8b60e

https://github.com/user-attachments/assets/14d1d56d-b02c-41ef-b43e-c3cbf191bf30

If you want to import this to your existing project, place the "Anim Baker" folder into unity's "Assets" folder. 

How to bake animations:
1.	Attach “Anim Tex Generator” script to the object who animations you want to bake.
2.	Drag the clips you want to be bake into the “Clips” slot in the script.
3.	Press Generate.
New non-skinned gameobject will be created playing your animations.

How to play specific animations:
1.	Create an empty gameobject and attach “Anim Tex Manager” script.
2.	Call its “PlayAnim” function by passing the baked objects material and the animation index to it. Look at “SpiderAnimPlayer” script as an example.

Important notes before baking an animation:
-	Object must have animator component attached to it.
-	Object must consist of only 1 skinned mesh renderer.
-	Object should consist of only 1 material. 

Tip: For multiple animation bakes, its recommended that start and end poses of the animations are similar. Otherwise the jump between the animations might look sudden.
