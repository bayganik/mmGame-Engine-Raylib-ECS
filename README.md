Simple Game Engine using Raylib as the core, Entitas lite as the ECS engine.  The combination works !!

The engine and a sample Menu + few other scenes with sprites, animated sprites, compound sprites + simple card game, 

https://github.com/ChrisDill/Raylib-cs         (use NuGet package, much easier)
https://github.com/rocwood/Entitas-Lite        (I've included Entitas here, since I made some minor additions)

Entity Component System (ECS) is used to allow for separation of concern when coding. 
Everything display on screen is an Entity, added to it are components that do special work.  Sprite adds an image, Text addes a text, BoxCollider allows the Entity to collide with other entities that have a collider.
Systems do the guts of the logic of the game.  They are executed once every frame and process all entities that match what we told it to do.

Below examples have "Debug" flag on.  F9 will flip "Debug" off/on.  The tank will move using arrow keys.  The red boxes are BoxColliders drawn as debug guide.

## Main Menu Screen

![game image](MenuSrn.png)

## Play Screen

![game image](PlaySrn.png)

## Card Game Screen

![game image](CardSrn.png)
