using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Entitas;
namespace mmGameEngine
{
    /*
	 * Every component inherits from this to give us the ability
	 * to ask Entitas for a collection that we can iterate and
	 * invoke Update() and Render() methods in our Entity class
	 */
    public abstract class Component : Entitas.IComponent
    {
        public int Tag;
        public bool Enabled = true;                 //is the component enabled (default true)
        public Vector2 CompPosition;                //This is used by UI components
        public Entity CompEntity;                   //This is updated in Scene loop
        public Transform Transform;                 //Transform component of entity
        public virtual void Update(float deltaTime)
        {
            if (CompEntity != null)
                Transform = CompEntity.Get<Transform>();
            else
                Transform = new Transform();
        }
    }
}
