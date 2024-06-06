using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;
using Entitas;
using mmGameEngine;
using Transform = mmGameEngine.TransformComponent;
using System.Diagnostics.Contracts;

namespace TestmmGame
{
    /*
     * Scene entities do not obey camera.  They are always within the screen space
     * Game  entities do obey camera.  Must call Global.GetMousePosition() or
     * ContentManager added to make a general way of loading all content
     */
    public class PlayScene : Scene
    {
        Entity entMap;              //TmxMap
        Entity msgEnt;              //msg box to inform
        Entity entPanel;
        Entity tankEnt;
        Entity entUI;
        Entity bullet;              //missle
        Entity explosion;           //explosion animation/sound
        //
        // bullet fire
        //
        SpriteAnimation rocketAnimation;
        
        Texture2D textureImage = new Texture2D();
        Sprite Spr;
        Vector2 position;
        public PlayScene()
        {
            //znznznznznznznznznznznznznznznznznznznznznznznznzn
            // This is done prior to game window opening
            //znznznznznznznznznznznznznznznznznznznznznznznznzn
            Global.SceneHeight = 800;
            Global.SceneWidth = 800;
            Global.SceneTitle = "Play Scene 800x800";
            Global.SceneClearColor = Color.Blue;
            //
            // base forlder for our contents (default)
            //
            ContentManager.BaseContnetFolder = "Assets";
        }

        public override void Play()
        {
            /*
             * 
             *          Scene window is open so setup what to display
             *          and systems that will act on them
             *          This scene opens with debug mode ON
             * 
             */
            Global.CurrentScene = this;
            Global.DebugRenderEnabled = true;                       //F9 toggles this
            //------------------------
            // Camera 2D setup
            //------------------------
            Camera2dEnabled = true;

            /*
             * 
             *          Messagebox display/Button action
             * 
             */
            msgEnt = Global.CreateSceneEntity(new Vector2(300, 300));
            msgEnt.Name = "msgbox";
            MsgBox msb = new MsgBox(200, 200, Color.Gray);
            msb.MsgLabel.TextData.Content = "Press OK button to\n\n start OK?";
            msgEnt.Add(msb);
            msb.MsgButton.Click += OK_ButtonClick;

            /*
             *          Game Entities
             */

            //-------------------------
            // TmxMap
            //-------------------------
            entMap = Global.CreateGameEntity(new Vector2(0, 0));
            entMap.Name = "txmMap";
            //TiledMap tm = new TiledMap("Assets/Map/Desert.tmx");
            TiledMap tm = ContentManager.Load<TiledMap>("Map/Desert.tmx");
            tm.RenderLayer = -1000;
            tm.Enabled = true;

            entMap.Get<Transform>().Enabled = true;
            entMap.Add(tm);
            //
            // used as test to make sure player doesn't step out
            //
            Global.WorldHeight = tm.Map.WorldHeight;
            Global.WorldWidth = tm.Map.WorldWidth;

            //-------------------------
            // Crosshair mouse cursor
            //-------------------------
            Entity CH = PrefabEntity.CreateCursorEntity("Assets/Img/crosshair.png");
            CH.Add<CrossHairComponent>();
            
            //--------------------------
            // Tank
            //--------------------------
            tankEnt = Global.CreateGameEntity(new Vector2(300, 500), .25f);
            tankEnt.Name = "Tank";
            tankEnt.Tag = 1000000;

            textureImage = ContentManager.Load<Texture2D>("Img/Tank Base.png");
            Spr = new Sprite(textureImage);             // Setup the sprite for entity
            Spr.EnableTracer = false;                   // draws a line to entity
            Spr.RenderLayer = 0;
            tankEnt.Add(Spr);
            //
            // add a collider
            //
            BoxCollider bx = new BoxCollider(Spr.Texture.Width * 0.25f, Spr.Texture.Height * 0.25f);
            tankEnt.Add(bx);
            //
            // add component that will identify tank in TankMovementSystem
            //
            tankEnt.Add<TankComponent>();
            //
            // add component that will move tank
            //
            InputController ic = new InputController(true);         //using WASD to move tank 
            tankEnt.Add(ic);

            //------------------------------------------------------------------
            // Create tank turret & attach it to tank base
            // Its position is relative to origin of its parent (tank base)
            // Set sprite origin of turret to lower position in its own texture
            //    so that it pivots lower to make more natural.
            //------------------------------------------------------------------
            Entity turret = Global.CreateGameEntity(new Vector2(0, 0), 1f);  //position whithin the parent
            turret.Name = "Turret";
            turret.Tag = 1000000;
            turret.Get<Transform>().Parent = tankEnt.Get<Transform>();       //attached to parent
            turret.Add<TurretComponent>();
            //
            // turret sprite
            //
            textureImage = ContentManager.Load<Texture2D>("Img/Tank Turret.png");

            Spr = new Sprite(textureImage);
            Spr.Origin = new Vector2(133, 500);             //when you want a very specific origin point (not center)

            turret.Get<Transform>().Visiable = true;
            turret.Get<Transform>().Rotation = 90;
            Spr.RenderLayer = 0;
            turret.Add(Spr);

            //--------------------------------------------------
            // Setup text to stay with tank
            // its location is relative to tank's origin
            //--------------------------------------------------
            Entity ent1 = Global.CreateGameEntity(new Vector2(-60, 100));
            ent1.Name = "Text";
            Text gdc = new Text("stay with tank", TextFontTypes.Default);
            gdc.FontSize = 20;
            gdc.TextData.FontColor = Color.DarkBlue;
            ent1.Get<Transform>().Parent = tankEnt.Get<Transform>();
            gdc.RenderLayer = 10;
            ent1.Add(gdc);
            //--------------------------
            // Create bullet placement
            //--------------------------
            //Entity bullPlace = CreateGameEntity( "bullet", new Vector2(0, 0));
            //bullPlace.Modify<Transform>().Parent = turret.Get<Transform>();
            //bullPlace.Modify<Transform>().Position = new Vector2(0, 0);

            //TurretComponent tcc = new TurretComponent();
            //tcc.BulletPlaceHolder = bullPlace;
            //turret.Add(tcc);


            //--------------------------------------------------------
            // Tell camera who to follow (player = tank)
            //--------------------------------------------------------
            if (Camera2dEnabled)
            {
                CameraEntityToFollow = tankEnt;
                CameraType2D = Camera2DType.FollowInsideMap;
            }

            //--------------------------------------------------
            // bullet entity that will fire Rocket animated sprite
            //--------------------------------------------------
            bullet = Global.CreateGameEntity("bullet");
            bullet.Tag = 1000000;
            bx = new BoxCollider(9, 29);
            bullet.AddComponent(bx);                         //add boxcollider
            //
            // bullet animated sprite
            //
            Texture2D rocketImage = ContentManager.Load<Texture2D>("Missile/Rocket9x26.png");
            rocketAnimation = new SpriteAnimation(rocketImage, 9, 26);
            rocketAnimation.AddAnimation("fly", "all");
            rocketAnimation.Play("fly", true);
            bullet.AddComponent(rocketAnimation);            //add animation sprite
            //
            // bullet has an automated mover component
            //
            EntityMover em = new EntityMover();
            em.Enabled = false;
            em.IsMoving = false;
            bullet.AddComponent(em);                         //auto mover
            bullet.IsVisible = true;
            //--------------------------------------------
            // Create explosion (animation & soundfx)
            //        we could use bullet to add these, but
            //        bullet has animation already
            //--------------------------------------------
            explosion = Global.CreateGameEntity(new Vector2(100, 300));
            explosion.Name = "Explode"; 
            Texture2D spriteSheet = ContentManager.Load<Texture2D>("Missile/EXP001.png");
            SpriteAnimation anim = new SpriteAnimation(spriteSheet, 128, 128);
            anim.RenderLayer = 11;

            anim.AddAnimation("explode", "all", 2.5f);
            explosion.Add(anim);
            //
            // explosion soundfx
            //
            SoundsFxComponent sfx = new SoundsFxComponent(ContentManager.Load<Sound>("Sound/boom.wav"));
            explosion.Add(sfx);
            //--------------------------------------------------------
            // Animated fire sprite & its collider box
            //--------------------------------------------------------
            Entity entFire = Global.CreateGameEntity(new Vector2(300, 300), 0.50f);
            entFire.Name = "Fire";
            spriteSheet = ContentManager.Load<Texture2D>("Img/flame232x148.png");

            SpriteAnimation fireAnim = new SpriteAnimation(spriteSheet, 232, 148);
            fireAnim.RenderLayer = 11;
            fireAnim.AddAnimation("fireburn", "all", 1.5f);
            fireAnim.Play("fireburn", true);
            entFire.Add(fireAnim);

            bx = new BoxCollider(232 * 0.50f, 148 * 0.50f);
            entFire.Add(bx);
            entFire.Add<FireComponent>();
            entFire.IsVisible = true;

            /*
             *          Systems to act on entities
             */
            AddSystem(new TurretMovementSystem());
            AddSystem(new TankMovementSystem());
            AddSystem(new CrossHairMoveSystem());
            AddSystem(new BulletMoveSystem());
            AddSystem(new FireMoveSystem());

        }
        public void PlayExplosion(Vector2 _pos)
        {
            explosion.Get<Transform>().Position = _pos;
            explosion.Get<Transform>().Enabled = true;
            explosion.Get<SpriteAnimation>().Play("explode", false);
            explosion.Get<SoundsFxComponent>().Play();
        }
        public void FireMissile(Vector2 moveTo, Vector2 moveFrom, float rotation)
        {
            //
            // Tank has fired a missile from turret
            //
            {
                EntityMover em = bullet.GetComponent<EntityMover>();
                em.Enabled = true;
                em.MoveStart= moveFrom ;
                em.MoveEnd= moveTo;
                em.IsMoving = true;
            }

            bullet.Get<Transform>().Scale = new Vector2(1, 1);
            bullet.Get<Transform>().Rotation = rotation;
            bullet.Get<Transform>().Position = moveFrom;
            bullet.Get<Transform>().Enabled = true;
            bullet.IsVisible = true;
        }
        public void OK_ButtonClick(object btn)
        {
            //
            // MsgBox ok button pushed
            //
            msgEnt.IsVisible = false;
            msgEnt.Destroy();
        }
        public void ChangeSprite()
        {
            //
            // change sprite of tank base (press C)
            //
            Sprite spr = tankEnt.Get<Sprite>();
            textureImage = ContentManager.Load<Texture2D>("Img/TankAlter.png");
            spr.Texture = textureImage;

        }

    }

}
