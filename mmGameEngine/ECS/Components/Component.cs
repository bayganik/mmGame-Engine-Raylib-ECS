using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

using Entitas;
namespace mmGameEngine
{
    /*
     * The basic data structure attached to an Entity.  Using Entitas.IComponent
	 * allows us to ask Entitas for a collection of components that we can iterate and
	 * invoke Update() method in our Scene class.
	 * 
	 * if you need to draw anything, then use RendereComponent
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
