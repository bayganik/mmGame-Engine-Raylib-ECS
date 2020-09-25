A Game Engine using Raylib as the core, Entitas lite as the ECS engine.  The combination works !!

The engine and a sample Menu + few other scenes with sprites, animated sprites, compound sprites + simple card game, 

https://github.com/ChrisDill/Raylib-cs         (use NuGet package, much easier)

https://github.com/rocwood/Entitas-Lite        (I've included Entitas here, since I made some minor additions)

Entity Component System (ECS) is used to allow for separation of concern when coding. 

Scene is the base of the game.  Inside the Scene you add Entities that have Components.  Then you add a System to act on those entities.  If you don't add a system, you all you get is a scene displaying a bunch of things (like the Card Scene).  


The are components that have special meaning.  

    Transform component gets added to all entities.
    
    Sprite adds an image
    
    SpriteAnimation adds an animated sprite using sprite sheet
    
    Tiled map - adds a TmxMap that allows you to access all of its levels & objects
    
    Text addes a text 
    
    BoxCollider allows the Entity to collide with other entities that have a collider
    
Systems do the guts of the logic of the game.  They are executed once every frame and process all entities that match certain components (that we give them).

Below examples have "Debug" flag on.  F9 will flip "Debug" off/on.  The tank will move using arrow keys.  The red boxes are BoxColliders drawn as debug guide.

## Main Menu Scene

<img align="right" src="./MenuSrn.png" >

The Menu scene uses the engine GUI component called Panel which has other components in it (buttons & label).
Panel is a Scene Entity (meaning it will be drawn on top of everything) and has Color.BLANK so you can see thru it.

The background image is a Game Entity with a Sprite component.




## Play Scene

![game image](PlaySrn.png)

## Card Game Scene

![game image](CardSrn.png)

Would like to thank Prime31 with his wonderful Nez ECS framework.  https://github.com/prime31/Nez  (although this framework no longer uses ECS)


