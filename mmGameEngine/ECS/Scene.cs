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
using mmGameEngine;

namespace mmGameEngine
{
	public class Scene 
	{
		//
		// Game object
		//
		public mmGame Game;
		//
		// ECS used for Entitis (Game entities & components)
		//
		Entitas.Context EntityContext;
		Entitas.Systems EntitySystems;
		List<Entity> GameEntities;
		//
		// ECS used for this Scene (Scene UI entities & components)
		//
		Entitas.Context SceneContext;
		Entitas.Systems SceneSystems;
		List<Entity> SceneEntities;

		//public Color WindowClearColor;
		//public KeyboardKey ExitKey = KeyboardKey.KEY_ESCAPE;
		public bool ForceEndScene = false;
		//
		// Camera 2D
		//
		public bool CameraEnabled = false;
		public Camera2D Camera;
		public Camera2DType CameraType2D;							//free, inside a map, push bounds
		public Entity CameraEntityToFollow;							//which entity to follow

		float deltaTime = 0;
		/// <summary>
		/// Create a new Window, Reset all GameEntities
		/// </summary>
		/// <param name="winClearColor"></param>
		/// <param name="winWidth"></param>
		/// <param name="winHeight"></param>
		/// <param name="winTitle"></param>
		public Scene()
		{
            Initialize();
			//
			// mmGame will call Begin() method
			// mmGame will call Play() method this is an override in your scene
			//
        }
		/// <summary>
		/// override this in Scene subclasses and do your loading here. This is called from the contructor after the scene sets itself up but
		/// before begin is ever called.
		/// </summary>
		public virtual void Initialize()
		{
			Global.DebugRenderEnabled = false;
			//
			// start ECS (Game entities & components & systems)
			//
			EntityContext = Entitas.Contexts.Default;
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
			CameraType2D = Camera2DType.FollowPlayer;					//free camera no bounds
		}
		/// <summary>
		/// override this in Scene subclasses. this will be called when Core sets this scene as the active scene.
		/// </summary>
		public virtual void OnStart()
		{
			/*
			 * Each scene will set the size of the screen override in its construct.
			 * This is left as a compatibility.  You can still use it for initializations etc.
			 */
		}
		/// <summary>
		/// Create a Game Entity, using a name and scale
		/// </summary>
		/// <returns></returns>
		public Entity CreateGameEntity(string name = "", float initScale = 1.0f)
        {
			//
			// Create the entity at Vector2(0,0), Scale 1.0f
			//
			Entity ent = EntityContext.CreateEntity();
			ent.EntityType = 0;		//game entity
			ent.name = name;
			//
			// fix the Transform
			//
			Transform trm = new Transform();
			trm.Scale = new Vector2(initScale, initScale);
			trm.Rotation = 0;
			ent.Add(trm);

			return ent;
        }
		/// <summary>
		/// Create a Game Entity using initial position and scale
		/// </summary>
		/// <param name="initPosition"></param>
		/// <param name="initScale"></param>
		/// <returns></returns>
		public Entity CreateGameEntity(Vector2 initPosition, float initScale = 1.0f)
		{
			Entity ent = EntityContext.CreateEntity();
			ent.EntityType = 0;			//game entity type
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
		/// Create a Scene Entity (drawn on top of game scene e.g. UI entity)
		/// </summary>
		/// <returns></returns>
		public Entity CreateSceneEntity(string name = "", float initScale = 1.0f)
		{           
			//
			// Create the entity at Vector2(0,0), Scale 1.0f
			//
			Entity ent = SceneContext.CreateEntity();
			ent.EntityType = 1;     //scene entity type
			ent.name = name;
			//
			// fix the Transform
			//
			Transform trm = new Transform();
			trm.Scale = new Vector2(initScale, initScale);
			trm.Rotation = 0;
			ent.Add(trm);

			return ent;
		}
		/// <summary>
		/// Create a Scene Entity (drawn on top of game scene e.g. UI entity)
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
			ent.EntityType = 1;        //scene entity type
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
		internal void Begin()
        {
			//
			// mmGame calls Begin() before a new scene window is created
			//
			SceneColliderDatabase.Initialize();

			//
			// Allow Entitas to init systems, auto collect matched systems, no manual Systems.Add(ISystem) required
			//
			EntitySystems = new Entitas.Feature(null);               //game entities
			EntitySystems.Initialize();

			SceneSystems = new Entitas.Feature(null);               //scene UI entities
			SceneSystems.Initialize();
			OnStart();
		}
		internal void End()
		{
			//
			// mmGame calls End() when scene changes, before new scene is created
			//
			EntitySystems.TearDown();
			EntitySystems.ClearReactiveSystems();
			EntityContext.DestroyAllEntities();

			SceneSystems.TearDown();
			SceneContext.DestroyAllEntities();
		}
		/// <summary>
		/// You MUST override this in Scene classes and do your loading & other logic. 
		/// This is called from mmGame after the scene construct, Begin(), then here
		/// </summary>
		public virtual void Play()
		{
			/*
			 * Game will override this to load assets, setup entities
			 * Components & Systems should be setup separately
			 */
		}
		/// <summary>
		/// mmGame calls this Update() method every frame 
		/// </summary>
		public virtual void Update()
		{
			//
			// Z O O M 
			//
			if (Global.DebugRenderEnabled && CameraEnabled)
			{
				// Camera zoom controls
				Camera.zoom += ((float)Raylib.GetMouseWheelMove() * 0.05f);

				if (Camera.zoom > 3.0f) Camera.zoom = 3.0f;
				else if (Camera.zoom < 0.1f) Camera.zoom = 0.1f;
	
				if (Raylib.IsKeyPressed(KeyboardKey.KEY_R))			// Camera reset (zoom and rotation)
				{
					Camera.zoom = 1.0f;
					Camera.rotation = 0.0f;
				}
			}
			deltaTime = Raylib.GetFrameTime();
			//
			// Find all Entities
			//
			GameEntities = EntityContext.GetEntities().Where(e => e.EntityType == 0).ToList();
			SceneEntities = EntityContext.GetEntities().Where(e => e.EntityType == 1).ToList();
			//
			// Update game entities (Todo: Update order ? like the RenderLayer)
			//
			foreach (Entity ent in GameEntities)
			{
				
				if (!ent.Get<Transform>().Enabled)
					continue;

				Entitas.IComponent[] allComp = ent.GetComponents();         //all entity components
				foreach (Entitas.IComponent comp in allComp)
				{
					if (comp is IRenderable)
					{
						//
						// Renderable components get the transform component of the Entity
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

			Game.ForceEndScene = ForceEndScene;
		}
		public void RemoveDeletedEntities()
        {
			//
			// Remove game entities, all children are included
			//      
			if (Global.GameEntityToDestroy.Count > 0)
			{
				foreach (KeyValuePair<Entity, bool> ent in Global.GameEntityToDestroy)
				{
					ent.Key.RemoveAllComponents();
					ent.Key.Destroy();					//release entity automatically

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
		}
		/// <summary>
		/// Display all Entities that are Visible using thier RenderLayer order
		/// </summary>
		public virtual void Render()
		{

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

			if (CameraEnabled && CameraEntityToFollow != null)
			{
				//    need to know the entity that is target of camera -> target Get<Transform>().Position
				//    Any component that is to be in the camera then is drawn first

				switch(CameraType2D)
                {
					case Camera2DType.FollowPlayer:
						UpdateCameraCenter();
						break;
					case Camera2DType.FollowInsideMap:
						UpdateCameraInsideMap();
						break;
					case Camera2DType.FollowCenterSmooth:
						UpdateCameraCenterSmoothFollow();
						break;
				}
				
				
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
			if (CameraEnabled && CameraEntityToFollow != null)
			{
				if (Global.DebugRenderEnabled)
				{ 
					//
					// display camera position
					//
					int screenHeight = Global.SceneHeight;
					int screenWidth = Global.SceneWidth;

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
				
				if (CameraEnabled && CameraEntityToFollow != null)
                {
					Raylib.DrawFPS(10, 30);
					Raylib.DrawText("Zoom" + Camera.zoom.ToString(), 10, 50, 20, Color.WHITE);
				}
				else
					Raylib.DrawFPS(10, 30);
			}


		}
		private void UpdateCameraCenterSmoothFollow()
		{
			//
			// Not a good effect
			//
			float delta = Raylib.GetFrameTime();
			int width = Global.SceneWidth;
			int height = Global.SceneHeight;
			float minSpeed = 30;
			float minEffectLength = 10;
			float fractionSpeed = 0.8f;
			float speed = 4.0f;

			Camera.offset = new Vector2(width / 2, height / 2);
			Vector2 diff = Vector2.Subtract(CameraEntityToFollow.Get<Transform>().Position, Camera.target);
			float length = Vector2Ext.Length(diff);

			if (length > minEffectLength)
			{
				//float speed = fmaxf(fractionSpeed * length, minSpeed);
				Camera.target = Vector2.Add(Camera.target, Vector2Ext.Scale(diff, speed * delta / length));
			}
		}
		private void UpdateCameraCenter()
        {
			//
			// Keep player in center of screen, no limits on what to display
			//
			int width = Global.SceneWidth;
			int height = Global.SceneHeight;
			Camera.target = CameraEntityToFollow.Get<Transform>().Position;
			Camera.offset = new Vector2(width / 2, height / 2);
		}
		private void UpdateCameraInsideMap()
        {
			//
			// Follow the player, display screen is limited to width & height set
			//
			int width = Global.SceneWidth;
			int height = Global.SceneHeight;

			Camera.target = CameraEntityToFollow.Get<Transform>().Position;
			Camera.offset = new Vector2(width / 2, height/ 2);

			float minX = 0;
			float minY = 0;
			float maxX = Global.WorldWidth;
			float maxY = Global.WorldHeight;

			Vector2 max = Raylib.GetWorldToScreen2D(new Vector2(maxX, maxY), Camera);
			Vector2 min = Raylib.GetWorldToScreen2D(new Vector2(minX, minY), Camera);
			//
			// camera ends at width of world/map
			//
			if (max.X < width) Camera.offset.X = width - (max.X - width / 2);
			if (max.X < height) Camera.offset.X = height - (max.X - height / 2);
			if (min.X > 0) Camera.offset.X = width / 2 - min.X;
			if (min.X > 0) Camera.offset.X = height / 2 - min.X;
			//
			// cameran ends at height of world/map
			//
			if (max.Y < width) Camera.offset.Y = width - (max.Y - width / 2);
			if (max.Y < height) Camera.offset.Y = height - (max.Y - height / 2);
			if (min.Y > 0) Camera.offset.Y = width / 2 - min.Y;
			if (min.Y > 0) Camera.offset.Y = height / 2 - min.Y;
		}
		private void UpdateCameraPlayerBoundsPush()
		{
			Vector2 position = CameraEntityToFollow.Get<Transform>().Position; 
			int width = Global.SceneWidth;
			int height = Global.SceneHeight;
			Vector2 bbox = new Vector2(0.2f, 0.2f);

			Vector2 bboxWorldMin = Raylib.GetScreenToWorld2D(new Vector2((1 - bbox.X) * 0.5f * width, (1 - bbox.X) * 0.5f * height), Camera);
			Vector2 bboxWorldMax = Raylib.GetScreenToWorld2D(new Vector2((1 + bbox.X) * 0.5f * width, (1 + bbox.X) * 0.5f * height), Camera);
			Camera.offset = new Vector2((1 - bbox.X) * 0.5f * width, (1 - bbox.X) * 0.5f * height);

			if (position.X < bboxWorldMin.X) Camera.target.X = position.X;
			if (position.X < bboxWorldMin.X) Camera.target.X = position.X;
			if (position.X > bboxWorldMax.X) Camera.target.X = bboxWorldMin.X + (position.X - bboxWorldMax.X);
			if (position.X > bboxWorldMax.X) Camera.target.X = bboxWorldMin.X + (position.X - bboxWorldMax.X);
		}
		public virtual void EndScene()
		{ }
	}
}
