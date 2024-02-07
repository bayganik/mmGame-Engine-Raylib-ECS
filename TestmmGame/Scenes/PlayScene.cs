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
     *                                           Global.GetMouseX()
     *                                           Global.GetMouseY()
     *                                           Global.WorldPosition(Vector2)
     */
    public class PlayScene : Scene
    {
        Entity entMap;              //TmxMap
        Entity msgEnt;              //msg box to inform
        Entity entPanel;
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
            //Global.ExitKey = KeyboardKey.KEY_BACKSPACE;
        }
        //znznznznznznznznznznznznznznznznzn
        // All game Setup is done in Play
        //znznznznznznznznznznznznznznznznzn
        public override void Play()
        {
            Global.CurrentScene = this;
            Global.DebugRenderEnabled = true;                       //F9 toggles this
            //------------------------
            // Camera 2D setup
            //------------------------
            Camera2dEnabled = true;

            /*
             * 
             *          Scene Entities
             * 
             */
            //msgEnt = CreateSceneEntity(new Vector2(300, 300));
            //msgEnt.Name = "msgbox";
            //MsgBox msb = new MsgBox( 200, 200, Raylib.BLUE);
            ////
            //// ok button of the msgbox
            ////
            //Vector2 posn = new Vector2((msb.Width / 2) - 20, msb.Height - 45);
            //Entity okEnt = CreateSceneEntity(posn);

            //Button okBtn = new Button( 35, 35, "OK", 7, 7);
            //okBtn.TextData.FontSize = 20;
            //okBtn.Click += OK_ButtonClick;
            //okEnt.Add(okBtn);
            //okEnt.Get<Transform>().Parent = msgEnt.Get<Transform>();
            
            //znznznznznznznznznznznznznznznznzn
            // msg lable of the message box
            //znznznznznznznznznznznznznznznznzn
            //Entity lblmsgEnt = CreateSceneEntity(new Vector2(10, 10));
            //Label lblmsg = new Label( "A panel with a button.\nPress OK to continue");
            //lblmsg.TextData.FontSize = 20;

            ////msb.AddButton(okBtn);
            //msb.AddMsg(lblmsg, new Vector2(10, 10));
            //msgEnt.Add(msb);
            //msgEnt.IsVisible = true;
            //lblmsgEnt.Get<Transform>().Parent = msgEnt.Get<Transform>();
            /*
             * 
             *          Game Entities
             * 
             */
            //-------------------------
            // TmxMap
            //-------------------------
            entMap = Global.CreateGameEntity(new Vector2(0, 0));
            entMap.Name = "txmMap";
            TiledMap tm = new TiledMap("Assets/Map/Desert.tmx");
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
            Entity tankEnt = Global.CreateGameEntity(new Vector2(300, 500), .25f);
            tankEnt.Name = "Tank";
            tankEnt.Tag = 1000000;
            textureImage = Raylib.LoadTexture("Assets/Img/Tank Base.png");
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
            // Create tank turret & attach it
            // Its position is relative to origin of the parent
            // Set sprite origin to lower position in its own texture
            //------------------------------------------------------------------
            Entity turret = Global.CreateGameEntity(new Vector2(0, 0), 1f);  //position whithin the parent
            turret.Name = "Turret";
            turret.Tag = 1000000;
            turret.Get<Transform>().Parent = tankEnt.Get<Transform>();       //attached to parent
            turret.Add<TurretComponent>();
            //
            // turret sprite
            //
            textureImage = Raylib.LoadTexture("Assets/Img/Tank Turret.png");
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
            Text gdc = new Text("stay with tank", TextFontTypes.Arial);
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
            // rocket animated sprite
            //
            Texture2D rocketImage = Raylib.LoadTexture("Assets/Missile/Rocket9x26.png");
            rocketAnimation = new SpriteAnimation(rocketImage, 9, 26);
            rocketAnimation.AddAnimation("fly", "all");
            rocketAnimation.Play("fly", true);
            bullet.AddComponent(rocketAnimation);            //add animation sprite

            //bullet.Get<Transform>().Parent = turret.Get<Transform>();

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
            Texture2D spriteSheet = Raylib.LoadTexture("Assets/Missile/EXP001.png");
            SpriteAnimation anim = new SpriteAnimation(spriteSheet, 128, 128);
            anim.RenderLayer = 11;

            anim.AddAnimation("explode", "all", 2.5f);
            explosion.Add(anim);
            //
            // explosion soundfx
            //
            SoundsFxComponent sfx = new SoundsFxComponent(Raylib.LoadSound("Assets/Sound/boom.wav"));
            explosion.Add(sfx);
            //--------------------------------------------------------
            // Animated fire sprite on a loop
            //--------------------------------------------------------
            Entity entFire = Global.CreateGameEntity(new Vector2(300, 300), 0.50f);
            entFire.Name = "Fire";
            spriteSheet = Raylib.LoadTexture("Assets/Img/flame232x148.png");

            SpriteAnimation fireAnim = new SpriteAnimation(spriteSheet, 232, 148);
            fireAnim.RenderLayer = 11;
            fireAnim.AddAnimation("fireburn", "all", 1.5f);
            fireAnim.Play("fireburn", true);
            entFire.Add(fireAnim);

            bx = new BoxCollider(232 * 0.50f, 148 * 0.50f);
            entFire.Add(bx);
            entFire.Add<FireComponent>();
            entFire.IsVisible = true;

            //znznznznznznznznznznznznznzn
            //   Add systems to update
            //znznznznznznznznznznznznznzn

            AddSystem(new CrossHairMoveSystem());
            AddSystem(new BulletMoveSystem());
            AddSystem(new FireMoveSystem());
            AddSystem(new TurretMovementSystem());
            AddSystem(new TankMovementSystem());
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
        public void CardsButton(object btn)
        {
            ForceEndScene = true;
            Global.NextScene = "TestmmGame.CardScene";
            Scene otherScene = new CardScene();
            mmGame.Scene = otherScene;
        }
        public void MapButton(object btn)
        {
            entMap.Get<Transform>().Enabled = !entMap.Get<Transform>().Enabled;
        }
        public void OK_ButtonClick(object btn)
        {
            msgEnt.IsVisible = false;

        }
        public void TileButtonClick(object btn)
        {
            msgEnt.IsVisible = true;

            Button tile = (Button)btn;
        }
        public void PushButton(object btn)
        {

            entUI.Get<Transform>().Position = new Vector2(500, 300);
        }
        public void PlayButton(object btn)
        {

            //entUI.Get<Transform>().Position = new Vector2(700, 100);
            Button bt = (Button)btn;
            bt.Enabled = false;
            entPanel.Get<Transform>().Enabled = false;
            entPanel.Get<Transform>().Visiable = false;

        }
        public void ChangeSprite(Entity ent)
        {
            Sprite spr = ent.Get<Sprite>();
            textureImage = Raylib.LoadTexture("Assets/Img/TankAlter.png");
            spr.Texture = textureImage;

        }

    }

}
