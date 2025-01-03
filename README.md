## Update Jan 02, 2025
    Updated to Raylib-cs 7.0.  Everything works and there were no breaks.
    
## Update June 06, 2024
    Added a ContentManager to Scene for uniform access to assets.  Assumes you have a folder named "Assets".  
    You can changed that by updating BaseContnetFolder property.

        ContentManager.Load<Texture2D>("Image/xxxxx.png");       //looks for png for file  "Assets/Image/xxxxx.png"
        ContentManager.Load<Model>("xxxxx.obj");                 //looks for 3D model file  "Assets/xxxxx.obj"
        ContentManager.Load<Sound>("xxxxx.wav");                 //looks for sound effects file  "Assets/xxxxx.wav"
        ContentManager.Load<Image>("xxxxx.jpg");                 //looks for jpg 'image' file  "Assets/xxxxx.png"
        ContentManager.Load<TiledMap>("Map/Desert.tmx");         //looks for Tiled map tmx file

    MsgBox is now created automatically with a Label and Button included.
## Update Feb 07, 2024
    Update to Raylib 5.0 (Raylib-cs 6.0) This update completely broke the previous version.  
    
## Update June 14, 2023
    Quick update getting ready to do Raylib 3D games. Moved all TransformComponent def from "Component.cs" class to individual 2D components.
    
    This will allow for creation of 3D components that can inherit from Component.cs class also.
    
    Scene.cs was updated to allow for Raylib.BeginMode3D or Raylib.BeginMode2D (WIP, so stay tuned)
   

A Game Engine using Raylib as the core, Entitas lite as the ECS engine and my own UI components.

The engine and a sample Menu + few other scenes with sprites, animated sprites, compound sprites + simple card game, 

https://github.com/ChrisDill/Raylib-cs         (use NuGet package, much easier)

https://github.com/rocwood/Entitas-Lite        (I've included Entitas here, since I made some minor additions)

Entity Component System (ECS) is used to allow for separation of concern when coding. 

Scene is the base of the game.  Inside the Scene you add Entities that have Components.  Then you add a System to act on those entities.  If you don't add a system, you all you get is a scene displaying a bunch of things (like the Card Scene).  


There are components that have special meaning.  

    Each game is a scene holding Entities.
        
        * Game Entity (what is used to do the game)
        
        * Scene Entity (typically UI elements that are drawn on top of all Game Entities)
        
    Transform component gets added to all entities automatically when they are created.
    
    Sprite component is used to display images
    
    SpriteAnimation component adds an animated sprite using spritesheet
    
    Tiled map component let you have a TmxMap.  You have access all of its levels & objects
    
    Text component addes a text that will follow the entity on the screen 
    
    BoxCollider component allows the Entity to collide with other entities that have a collider
    
Systems do the guts of the logic of the game.  They are executed once every frame and process all entities that match certain components (that we give them).

Below examples have "Debug" flag on.  F9 will flip "Debug" off/on.  The tank will move using arrow keys.  The red boxes are BoxColliders drawn as debug guide.

## To Start a VS2022 project:

    Start with Net 7.0 console app
    
    Using Dependencies add Raylib-cs Nuget package
    
    Add references to Entitas-Lite , mmGameEngine and Sanford.MIDI
 
## Your Program.cs

    using mmGameEngine;

    namespace TestmmGame
    {
        class Program
        {
            static void Main(string[] args)
            {

                TestGame game = new TestGame();

            }
        }
        public class TestGame : mmGame
        {
            public TestGame() : base()
            {
                Scene = new MenuScene();
            }
        }
    }

## Main Menu Scene

<img align="right" src="./MenuSrn.png" >

The Menu scene uses the engine GUI component called Panel which has other components in it (buttons & label).
Panel is a Scene Entity (meaning it will be drawn on top of everything) and has Color.BLANK so you can see thru it.

The background image is a Game Entity with a Sprite component.











## Play Scene

![game image](PlaySrn.png)









## Card Game Scene

![game image](CardSrn.png)

## MIDI Piano Scene

![game image](MidiPiano.png)


