https://github.com/user-attachments/assets/a5be1733-7fa7-45bd-9dbb-522f4db8b60e

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
-	Do not delete or change the location of “Anim Baker” folder.
