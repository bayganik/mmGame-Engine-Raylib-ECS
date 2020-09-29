using System;
using System.Collections.Generic;
using System.Numerics;
using Raylib_cs;

using mmGameEngine;
using Transform = mmGameEngine.Transform;

using Entitas;


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
        //
        // bullet fire
        //
        SpriteAnimation rocketAnimation;
        
        Texture2D textureImage = new Texture2D();
        Sprite Spr;
        Vector2 position;
        public PlayScene()
        {
            Global.SceneHeight = 800;
            Global.SceneWidth = 800;
            Global.SceneTitle = "Play Scene";
            Global.SceneClearColor = Color.BLUE;
        }
        public override void Play()
        {
            Global.CurrentScene = this;

            //znznznznznznznznznznznznznznznznznzn
            // Camera 2D setup
            //znznznznznznznznznznznznznznznznznzn

            CameraEnabled = true;

            Global.DebugRenderEnabled = true;

            //znznznznznznznznznznznznznznznznzn
            // All game Setup goes at this point
            //znznznznznznznznznznznznznznznznzn
            /*
             * 
             *          Scene Entities
             * 
             */
            //msgEnt = CreateSceneEntity(new Vector2(300, 300));
            //msgEnt.name = "msgbox";
            //MsgBox msb = new MsgBox(new Vector2(300, 300), 200, 200, Color.RED);
            ////
            //// ok button of the msgbox
            ////
            //Vector2 posn = new Vector2((msb.Width / 2) - 20, msb.Height - 45);
            //Button ok = new Button(posn, 35, 35, "OK", -5, 7);
            //ok.TextData.FontSize = 20;
            //ok.Click += OK_ButtonClick;
            //
            // msg
            //
            //Label lblmsg = new Label(new Vector2(10, 10), "Press OK to continue");
            //lblmsg.TextData.FontSize = 20;

            //msb.AddButton(ok);
            //msb.AddMsg(lblmsg);
            //msgEnt.Add(msb);
            //msgEnt.Get<Transform>().Visiable = false;

            //
            // Button
            //
            //entUI = CreateSceneEntity(new Vector2(500, 300));
            //entUI.name = "button";
            //position = entUI.Get<Transform>().Position;
            //Button bt = new Button(position, 75, 30, "Push", -6, 2);
            //entUI.Add(bt);
            //bt.Click += PushButton;
            //-----------------------------
            // Panel (menu type)
            //-----------------------------
            //entPanel = CreateSceneEntity(new Vector2(800, 200));
            //entPanel.name = "panel";
            //position = entPanel.Get<Transform>().Position;

            //Panel menuPanel = new Panel(position, 300, 200, Color.BEIGE);
            //// add a label on top
            //Label lbl = new Label(new Vector2(80, 10), "This is a Menu");
            //menuPanel.AddComponent(lbl);


            //textureImage = Raylib.LoadTexture("Assets/Img/Button_empty.png");
            ////
            //// play button
            ////
            //Button playBt = new Button(new Vector2(100, 40), 74, 35, "Play", -2, +5);
            //playBt.Image = textureImage;
            //playBt.Click += PlayButton;
            //menuPanel.AddComponent(playBt);
            ////
            //// Map button
            ////
            //Button setupBt = new Button(new Vector2(100, 85), 74, 35, "Map", -12, +5);
            //setupBt.Image = textureImage;
            //setupBt.Click += MapButton;
            //menuPanel.AddComponent(setupBt);
            ////
            //// Map button
            ////
            //Button cardBt = new Button(new Vector2(100, 125), 74, 35, "Cards", -12, +5);
            //cardBt.Image = textureImage;
            //cardBt.Click += CardsButton;
            //menuPanel.AddComponent(cardBt);

            //entPanel.Add(menuPanel);
            //---------------------------
            // Panel with tiles
            //---------------------------
            //Entity entTiles = CreateSceneEntity(new Vector2(800, 400));
            //entTiles.name = "tiles";
            //Vector2 pos = entTiles.Get<Transform>().Position;

            //Panel panWithTiles = new Panel(pos, 300, 310, Color.LIME);
            //panWithTiles.BorderThickness = 0;
            ////
            //// Panel of tiles to look like a cross word puzzel
            ////
            //int inc = 0;
            //for (int i = 0; i < 10; i++)
            //{
            //    //if (i != 0)
            //    //    inc = 1;
            //    Vector2 poss = new Vector2((i * 30) + inc, 0);

            //    Button tbut = new Button(poss, 30, 30, i.ToString(), 7, 8);
            //    tbut.Tag = i;
            //    tbut.BackgroundColor = Color.BLANK;
            //    tbut.TextData.FontSize = 13;
            //    tbut.TextData.FontColor = Color.BLACK;
            //    tbut.Click += TileButtonClick;
            //    panWithTiles.AddComponent(tbut);
            //}

            //entTiles.Add(panWithTiles);
            /*
             * 
             *          Game Entities
             * 
             */
            //-------------------------
            // TmxMap
            //-------------------------
            entMap = this.CreateGameEntity(new Vector2(0, 0));
            entMap.name = "txmMap";
            TiledMap tm = new TiledMap("Assets/Map/Desert.tmx");
            tm.RenderLayer = -1000;
            tm.Enabled = true;

            entMap.Get<Transform>().Enabled = true;
            entMap.Add(tm);

            Global.WorldHeight = tm.Map.WorldHeight;
            Global.WorldWidth = tm.Map.WorldWidth;

            //------------------------------
            // Scrolling background
            //------------------------------
            Entity backG = this.CreateGameEntity(new Vector2(0, 0), 1.0f);
            backG.name = "Scrolling";
            textureImage = Raylib.LoadTexture("Assets/Img/background.png");
            ScrollingImage si = new ScrollingImage(textureImage);
            si.ScrollSpeedX = 10;
            backG.Add(si);
            backG.Get<Transform>().Visiable = true;
            //-------------------------
            // Crosshair mouse cursor
            //-------------------------
            Entity CH = this.CreateGameEntity(Vector2.Zero);
            CH.name = "cursor";

            textureImage = Raylib.LoadTexture("Assets/Img/crosshair.png");
            Spr = new Sprite(textureImage);
            Spr.RenderLayer = 100;              //on top of everything
            CH.Add(Spr);
            BoxCollider bxxx = new BoxCollider(8, 8);
            //CircleCollider bxxx = new CircleCollider(0, 0, 8);
            CH.Add(bxxx);
            CH.Add<CrossHairComponent>();
            //--------------------------
            // Tank
            //--------------------------
            Entity tankEnt = this.CreateGameEntity(new Vector2(300, 500), .25f);
            tankEnt.name = "Tank";
            textureImage = Raylib.LoadTexture("Assets/Img/Tank Base.png");
            Spr = new Sprite(textureImage);             // Setup the sprite for ent
            Spr.EnableTracer = true;

            //Spr.HasBoxCollider = true;
            tankEnt.Get<Transform>().Visiable = true;
            tankEnt.Get<Transform>().Rotation = 0;
            Spr.RenderLayer = 0;
            tankEnt.Add(Spr);
            //
            // add tank collider
            //
            BoxCollider bx = new BoxCollider(Spr.Texture.width * 0.25f, Spr.Texture.height * 0.25f);
            tankEnt.Add(bx);
            tankEnt.Add<TankComponent>();
            //----------------------
            // Create turret
            //----------------------
            Entity turret = this.CreateGameEntity(new Vector2(0, 0), 1f);       //position whithin the parent
            turret.name = "Turret";
            turret.Get<Transform>().Parent = tankEnt.Get<Transform>();
            textureImage = Raylib.LoadTexture("Assets/Img/Tank Turret.png");
            Spr = new Sprite(textureImage);                        // Setup the sprite for ent
            Spr.OriginLocal = new Vector2(133, 500);             //when you want a very specific origin point

            turret.Get<Transform>().Visiable = true;
            turret.Get<Transform>().Rotation = 90;
            Spr.RenderLayer = 0;
            turret.Add(Spr);
            //--------------------------
            // Create bullet placement
            //--------------------------
            Entity bullPlace = CreateGameEntity("bullet");
            bullPlace.Modify<Transform>().Parent = turret.Get<Transform>();
            bullPlace.Modify<Transform>().LocalPosition = new Vector2(0, -500);

            TurretComponent tcc = new TurretComponent();
            tcc.BulletPlaceHolder = bullPlace;
            turret.Add(tcc);
            //------------------------
            // Rocket sprite
            //------------------------
            Texture2D rocketImage = Raylib.LoadTexture("Assets/Missile/Rocket9x26.png");

            rocketAnimation = new SpriteAnimation(rocketImage, 9, 26);
            rocketAnimation.AddAnimation("fly", "all");
            rocketAnimation.Play("fly", true);
            //--------------------------------------------------------
            // Tell camera who to follow, where its offset is
            //--------------------------------------------------------
            if (CameraEnabled)
            {
                CameraEntityToFollow = tankEnt;
                CameraType2D = Camera2DType.FollowInsideMap;
            }

            //-----------------------------
            // Setup the words for ent1
            //----------------------------
            Entity ent1 = this.CreateGameEntity(new Vector2(30, 300));
            ent1.name = "Text";
            Text gdc = new Text("stay with tank", TextFontTypes.Default);
            ent1.Get<Transform>().Parent = tankEnt.Get<Transform>();
            gdc.RenderLayer = 10;
            ent1.Add(gdc);


            //
            // setup a Sprite Explosion
            //
            //Entity entAnim = CreateGameEntity(new Vector2(300, 300));
            //entAnim.name = "Exlode";
            Texture2D spriteSheet = Raylib.LoadTexture("Assets/Missile/EXP001.png");

            //SpriteAnimation anim = new SpriteAnimation(spriteSheet, 128, 128);
            //anim.RenderLayer = 11;
            //anim.AddAnimation("explode", "all", 2.5f);
            //anim.Play("explode", false);
            //entAnim.Add(anim);
            //
            // explosionsound
            //
            //Entity entSound = CreateGameEntity();
            //SoundsFx sfx = new SoundsFx(Raylib.LoadSound("Assets/Sound/boom.wav"));
            //sfx.Play();
            //entSound.Add(sfx);
            //
            // setup a Sprite Fire (looping)
            //
            Entity entFire = CreateGameEntity(new Vector2(300, 300), 0.50f);
            entFire.name = "Fire";
            spriteSheet = Raylib.LoadTexture("Assets/Img/flame232x148.png");

            SpriteAnimation fireAnim = new SpriteAnimation(spriteSheet, 232, 148);
            fireAnim.RenderLayer = 11;
            fireAnim.AddAnimation("explode", "all", 1.5f);
            fireAnim.Play("explode", true);
            entFire.Add(fireAnim);

            bx = new BoxCollider(232 * 0.50f, 148 * 0.50f);
            entFire.Add(bx);
            entFire.Add<FireComponent>();

            //
            // Now, scene will cycle thru all componenets and systems to do
            // All display & move & other logic
            //


        }
        public void FireMissile(Vector2 moveTo, Vector2 moveFrom, float rotation)
        {
            Entity bullet = CreateGameEntity("bullet");
            bullet.Get<Transform>().Scale = new Vector2(1, 1);
            bullet.Get<Transform>().Rotation = rotation;
            bullet.Get<Transform>().Position = moveFrom;
            BoxCollider bx = new BoxCollider( 9, 29);
            bullet.Add(bx);

            bullet.Add(rocketAnimation);
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
            msgEnt.Get<Transform>().Visiable = false;
        }
        public void TileButtonClick(object btn)
        {
            msgEnt.Get<Transform>().Visiable = true;

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
