using System;
using System.Collections.Generic;
using System.Text;
using Entitas;
using mmGameEngine;
using Raylib_cs;

namespace TestmmGame
{
    public class PileDispComponent : Component
    {
        public PileDispComponent()
        {

        }
    }
    public class EntityCapturedComponent : Component
    {
        public Entity CurrentEntityChosen;
        public EntityCapturedComponent()
        {

        }
    }
    public class CrossHairComponent : Component
    {
        public Entity CurrentEntityChosen;
        public CrossHairComponent()
        {

        }
    }
    public class TankComponent : Component
    {
        public float Speed = 20f;
        public TankComponent()
        { }
    }
    public class FireComponent : Component
    {
        public float Speed = 20f;
        public FireComponent()
        { }
    }
    public class TurretComponent : Component
    {
        public Entity BulletPlaceHolder;
        public TurretComponent()
        { }
    }
    public class TextMoveComponent : Component
    {
        public int x;
        public int y;

        public TextMoveComponent()
        {
            x = 0;
            y = 0;
        }
        public override void Update(float deltaTime)
        { }

    }

    // if no context declaration, it comes into Default context
    //public class GameDrawComponent : RenderComponent
    //{
    //	public int x;
    //	public int y;
    //	public GameDrawComponent()
    //       {
    //		RenderLayer = 10;
    //       }
    //	// don't be afraid of writing helper accessor
    //	public void SetValue(int nx, int ny)
    //	{
    //		x = nx;
    //		y = ny;
    //	}
    //	public override void Render()
    //	{
    //		//Raylib.DrawTexture(Sprite, x, y, Color.WHITE);
    //		Raylib.DrawText("PlayScene, ..", (int)Transform.Position.X, (int)Transform.Position.Y, 30, Color.BLACK);

    //	}
    //}
}
