using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Raylib_cs;

using Entitas;
using System.Data;
using System.Dynamic;
using System.Numerics;


namespace mmGameEngine
{
	public class SceneOriginal
	{
		//
		// ECS used for Entitis (Game objects & components)
		//
		Entitas.Context EntityContext;
		Entitas.Systems EntitySystems;
		List<Entity> GameEntities;
		//
		// ECS used for this Scene (Game UI & components)
		//
		Entitas.Context SceneContext;
		Entitas.Systems SceneSystems;
		List<Entity> SceneEntities;

		//public int WindowWidth = 800;
		//public int WindowHeight = 640;
		//public string WindowTitle = "Game Scene";

		public Color WindowClearColor;
		public KeyboardKey ExitKey = KeyboardKey.KEY_ESCAPE;
		public bool ForceEndScene = false;
		//
		// Camera 
		//
		public bool CameraEnabled = false;
		public Camera2D Camera;
		public Entity CameraEntityToFollow;

		float deltaTime = 0;
		/// <summary>
		/// Create a new Window, Reset all GameEntities
		/// </summary>
		/// <param name="winClearColor"></param>
		/// <param name="winWidth"></param>
		/// <param name="winHeight"></param>
		/// <param name="winTitle"></param>
		public SceneOriginal(Color winClearColor, int winWidth = 800, int winHeight = 640, string winTitle = "Game Scene")
		{
			WindowClearColor = winClearColor;
            //
            // setup game window
            //
            Raylib.SetConfigFlags(ConfigFlag.FLAG_WINDOW_RESIZABLE);
            //
            // This allows all other operations for RayLib 
            //
            Raylib.InitWindow(winWidth, winHeight, winTitle);
            Raylib.SetWindowMinSize(600, 4000);
            Raylib.SetWindowPosition(200, 100);
            Raylib.SetTargetFPS(Global.TARGET_FPS);
            Raylib.InitAudioDevice();

            Global.ViewPortWidth = winWidth;
            Global.ViewPortHeight = winHeight;
			Global.WindowCenter = new Vector2(winWidth / 2, winHeight / 2);

            Initialize();
        }
		//public void ResetWindow(int _width, int _height, string _title)
  //      {
		//	//
		//	// Exit key or forced out
		//	//
		//	Raylib.CloseAudioDevice();
		//	Raylib.CloseWindow();
		//	//
		//	// setup game window
		//	//
		//	Raylib.SetConfigFlags(ConfigFlag.FLAG_WINDOW_RESIZABLE);
		//	//
		//	// This allows all other operations for RayLib 
		//	//
		//	Raylib.InitWindow(_width, _height, _title);
		//	Raylib.SetWindowMinSize(600, 4000);
		//	Raylib.SetWindowPosition(200, 100);
		//	Raylib.SetTargetFPS(Global.TARGET_FPS);
		//	Raylib.InitAudioDevice();

		//	Camera = new Camera2D();
		//	Camera.rotation = 0;
		//	Camera.zoom = 1.0f;

		//	Global.ViewPortWidth = _width;
		//	Global.ViewPortHeight = _height;
		//}
		/// <summary>
		/// override this in Scene subclasses and do your loading here. This is called from the contructor after the scene sets itself up but
		/// before begin is ever called.
		/// </summary>
		public virtual void Initialize()
		{
			Global.DebugRenderEnabled = false;
			Raylib.SetExitKey(ExitKey);
			//
			// start ECS (Game entities & components & systems)
			//
			EntityContext = Entitas.Contexts.Default;
			//EntityContext = Contexts.Get("EntityContext");



			Global.EntityContext = EntityContext;
			//
			// List of all entities, List of entities destroyed
			//
			GameEntities = new List<Entity>() ;
			Global.GameEntityToDestroy = new Dictionary<Entity, bool>();
			SceneEntities = new List<Entity>();
			Global.SceneEntityToDestroy = new Dictionary<Entity, bool>();
			//
			// start ECS (Scene UI & components)
			//
			SceneContext = Entitas.Contexts.Default;
			//SceneSystems = new Feature(null);

			Global.SceneContext = SceneContext;

			//znznznznznznznznznznznznznznznznznzn
			// Camera 2D setup
			//znznznznznznznznznznznznznznznznznzn

			Camera = new Camera2D();
			Camera.target = Global.WindowCenter;
			Camera.offset = Global.WindowCenter;
			Camera.rotation = 0;
			Camera.zoom = 1.0f;
			CameraEnabled = false;
		}
		/// <summary>
		/// Create an Entity, Add a Transform component
		/// </summary>
		/// <returns></returns>
		public Entity CreateGameEntity(string name = "")
        {
			//
			// Create the entity at Vector2(0,0), Scale 1.0f
			//
			Entity ent = EntityContext.CreateEntity();
			ent.EntityType = 0;		//game entity
			ent.name = name;
			ent.Add<Transform>();
			
			return ent;
        }
		/// <summary>
		/// Game Entity is used to create objects/sprites/images/etc.  as part of the game
		/// </summary>
		/// <param name="initPosition"></param>
		/// <param name="initScale"></param>
		/// <returns></returns>
		public Entity CreateGameEntity(Vector2 initPosition, float initScale = 1.0f)
		{
			//
			// reate the entity at initPosition, Scale initScale
			//
			Entity ent = EntityContext.CreateEntity();
			ent.EntityType = 0;			//game entity
			//
			// fix the Transform
			//
			Transform trm = new Transform();
			trm.Position = initPosition;
			trm.Scale = new Vector2(initScale, initScale);
			trm.Rotation = 0;

			ent.Add(trm);
			return ent;
		}
		/// <summary>
		/// Scene Entity is typically a UI element drawn on top of game scene
		/// </summary>
		/// <returns></returns>
		public Entity CreateSceneEntity()
		{
			Entity ent = SceneContext.CreateEntity();
			ent.EntityType = 1;		//scene entity
			SceneEntities.Add(ent);
			ent.Add<Transform>();
			return ent;
		}
		/// <summary>
		/// Scene Entity is typically a UI element drawn on top of game scene
		/// </summary>
		/// <param name="initPosition"></param>
		/// <param name="initScale"></param>
		/// <returns></returns>
		public Entity CreateSceneEntity(Vector2 initPosition, float initScale = 1.0f)
        {
			//
			// reate the entity at initPosition, Scale initScale
			//
			Entity ent = SceneContext.CreateEntity();
			ent.EntityType = 1;        //scene entity
			//
			// fix the Transform
			//
			Transform trm = new Transform();
			trm.Position = initPosition;
			trm.Scale = new Vector2(initScale, initScale);
			trm.Rotation = 0;

			ent.Add(trm);
			SceneEntities.Add(ent);
			return ent;
		}
		/// <summary>
		/// Every scene MUST call DispScene() to start the logic.  Do this after all initializations are done
		/// </summary>
		public void DisplayScene()
		{
			SceneColliderDatabase.Initialize();

			//
			// Allow Entitas to init systems, auto collect matched systems, no manual Systems.Add(ISystem) required
			//
			EntitySystems = new Entitas.Feature(null);			//game entities
			EntitySystems.Initialize();

			SceneSystems = new Entitas.Feature(null);				//scene UI entities
			SceneSystems.Initialize();


			//znznznznznznznznznznznznzn
			//      GAME LOOP
			//znznznznznznznznznznznznzn
			while (!Raylib.WindowShouldClose())
			{
				//
				// If the scene is done
				//
				if (ForceEndScene)
				{
					EntitySystems.TearDown();
					EntityContext.DestroyAllEntities();

					SceneSystems.TearDown();
					SceneContext.DestroyAllEntities();
					break;
				}
				//
				// Test for debug key F9 (F12 is used for screenshot by RayLib)
				//
				if (Raylib.IsKeyPressed(KeyboardKey.KEY_F9))
				{
					Global.DebugRenderEnabled = !Global.DebugRenderEnabled;
					if (!Global.DebugRenderEnabled)
						Camera.zoom = 1.0f;

				}
				if (Global.DebugRenderEnabled && CameraEnabled)
						Camera.zoom += ((float)Raylib.GetMouseWheelMove() * 0.05f);

				deltaTime = Raylib.GetFrameTime();
				//
				// Find all Entities
				//
				GameEntities = EntityContext.GetEntities().Where(e => e.EntityType == 0).ToList();
				SceneEntities = EntityContext.GetEntities().Where(e => e.EntityType == 1).ToList();

				Update(deltaTime);      // Update all enabled components


				Render();               // Draw all visible components

				//
				// Remove game entities, all children are included
				//      
				if (Global.GameEntityToDestroy.Count > 0)
                {
                    foreach (KeyValuePair<Entity, bool> ent in Global.GameEntityToDestroy)
                    {
						ent.Key.RemoveAllComponents();
						ent.Key.Destroy(); 
						
                        //GameEntities.Remove(ent.Key);
                    }
					Global.GameEntityToDestroy = new Dictionary<Entity, bool>();

				}
				//
				// Remove Scene specific entities (UI?)
				//
				if (Global.SceneEntityToDestroy.Count > 0)
				{
					foreach (KeyValuePair<Entity, bool> ent in Global.SceneEntityToDestroy)
					{
						ent.Key.RemoveAllComponents();
						ent.Key.Destroy();

						//GameEntities.Remove(ent.Key);
					}
					Global.SceneEntityToDestroy = new Dictionary<Entity, bool>();

				}
				//
				// Do ECS update/cleanup
				//
				EntitySystems.Execute();
				EntitySystems.Cleanup();

				if (Raylib.IsKeyPressed(KeyboardKey.KEY_ESCAPE))
					ForceEndScene = true;
			}
			//
			// Exit key or forced out
			//
			Raylib.CloseAudioDevice();
			Raylib.CloseWindow();
		}
		/// <summary>
		/// Update all Entities that are enabled
		/// </summary>
		/// <param name="deltaTime"></param>
		public virtual void Update(float deltaTime)
		{
			//
			// Update game entities (Todo: Update order ? like the RenderLayer)
			//
			foreach (Entity ent in GameEntities)
			{

                if (!ent.Get<Transform>().Enabled)
                    continue;

                Entitas.IComponent[] allComp = ent.GetComponents();			//this entity's component
				foreach (Entitas.IComponent comp in allComp)
				{
					if (comp is IRenderable)
					{
						//
						// Renderable components need to know what happened to Entity Transform
						//
						RenderComponent myComp = (RenderComponent)comp;
						myComp.CompEntity = ent;
						myComp.Update(deltaTime);
					}
					else
					{
						Component myComp = (Component)comp;
						myComp.Update(deltaTime);                           //force it to update()
					}
				}
			}
            EntitySystems.Execute();
            EntitySystems.Cleanup();
            //
            // Update scene UI entities
            //
            foreach (Entity ent in SceneEntities)
			{
                if (!ent.Get<Transform>().Enabled)
                    continue;

                Entitas.IComponent[] allComp = ent.GetComponents();         //this entity's component
				foreach (Entitas.IComponent comp in allComp)
				{
					if (comp is IRenderable)
					{
						//
						// Renderable components need to know what happened to Entity Transform
						//
						RenderComponent myComp = (RenderComponent)comp;
						myComp.CompEntity = ent;
						myComp.Update(deltaTime);
					}
					else
					{
						Component myComp = (Component)comp;
						myComp.Update(deltaTime);                          //force it to update()
					}
				}
			}
			
			SceneSystems.Execute();
			SceneSystems.Cleanup();
		}
		/// <summary>
		/// Display all Entities that are Visible using thier RenderLayer order
		/// </summary>
		public virtual void Render()
		{
			Raylib.BeginDrawing();
			Raylib.ClearBackground(WindowClearColor);
			//
			// Get all RenderComponent, sort them, low -> high
			//
			List<RenderComponent> ComponentsToRender = new List<RenderComponent>();

			foreach (Entity ent in GameEntities)
			{
				if (!(ent.Get<Transform>().Enabled && ent.Get<Transform>().Visiable))
					continue;

				//
				// Ask Entitas for all components attached to "ent" entity
				//
				Entitas.IComponent[] allComp = ent.GetComponents();       //this entity's component
				foreach (Entitas.IComponent comp in allComp)			  //get the renderable ones
				{
					if (comp is IRenderable)
					{
						RenderComponent myComp = (RenderComponent)comp;
						ComponentsToRender.Add(myComp);
					}
				}
			}

			//
			//   C A M E R A  D I S P L A Y  begins
			//

			if (CameraEnabled)
			{
				//    need to know the entity that is target of camera -> target Get<Transform>().Position
				//    Any component that is to be in the camera then is drawn first
				Camera.target = CameraEntityToFollow.Get<Transform>().Position;// + new Vector2(100, 0);

				Raylib.BeginMode2D(Camera);
			}

			//
			//   R E N D E R  O R D E R   sorting (low to high)/render
			//
			foreach (RenderComponent myComp in ComponentsToRender.OrderBy(e => e.RenderLayer))
			{
				myComp.Render();									//call draw method
			}
			//
			//   C A M E R A  D I S P L A Y  ends
			//
			if (CameraEnabled)
			{
				if (Global.DebugRenderEnabled)
				{ 
					//
					// display camera position
					//
					int screenHeight = Global.ViewPortHeight;
					int screenWidth = Global.ViewPortWidth;

                    int tx = (int)CameraEntityToFollow.Get<Transform>().Position.X;
                    int ty = (int)CameraEntityToFollow.Get<Transform>().Position.Y;

                    Raylib.DrawLine(tx, -screenHeight * 10, tx, screenHeight * 10, Color.GREEN);    //Verticval
					Raylib.DrawLine(-screenWidth * 10, ty, screenWidth * 10, ty, Color.GREEN);      //Horizontal

					//tx = (int)Global.WindowCenter.X;
					//ty = (int)Global.WindowCenter.Y;
					//Raylib.DrawLine(tx, -screenHeight * 10, tx, screenHeight * 10, Color.RED);    //Verticval
					//Raylib.DrawLine(-screenWidth * 10, ty, screenWidth * 10, ty, Color.RED);      //Horizontal

				}
				Raylib.EndMode2D();
			}
			//
			//   U I  E N T I T I E S , they are drawn on top of all other game entities
			//
			foreach (Entity ent in SceneEntities)
			{
				if (!ent.Get<Transform>().Enabled)
					continue;

				if (!ent.Get<Transform>().Visiable)
					continue;

				Entitas.IComponent[] allComp = ent.GetComponents();         //this entity's component
				foreach (Entitas.IComponent comp in allComp)
				{
					if (comp is IRenderable)
					{
						RenderComponent myComp = (RenderComponent)comp;
						myComp.Render();                                    //call draw method
					}
				}
			}

			//
			// Scene debug
			//
			if (Global.DebugRenderEnabled)
			{
				Raylib.DrawText(Raylib.GetMousePosition().ToString(), 10, 10, 20, Color.WHITE);
				
				if (CameraEnabled)
                {
					Raylib.DrawFPS(10, 30);
					Raylib.DrawText("Zoom" + Camera.zoom.ToString(), 10, 50, 20, Color.WHITE);
				}
				else
					Raylib.DrawFPS(10, 30);
			}

			Raylib.EndDrawing();
		}
		public virtual void EndScene()
		{ }
	}
}
