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
}
