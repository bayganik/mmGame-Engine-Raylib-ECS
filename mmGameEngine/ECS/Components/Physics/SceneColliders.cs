using Entitas;
using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Database of BoxColliders in this scene.  The colliders must get updated when entity position
     * changes.
     */
    public static class SceneColliders
    {
        /// <summary>
        /// collection is just a database of entities that have colliders
        /// </summary>
        public static Dictionary<Entity, int> ColliderCollection;
        public static void Initialize()
        {
            ColliderCollection = new Dictionary<Entity, int>();
        }
        //
        // Give an entity a BoxCollider. If it has one, remove it, add it again
        //
        public static void SetCollider(Entity entity, CollidreShape shapeNum)
        {
            if (ColliderCollection.ContainsKey(entity))
                return;
            //{
            //    //
            //    // found the collider, remove it and add again (in case there was an update)
            //    //
            //    ColliderCollection.Remove(entity);
            //}
            ColliderCollection.Add(entity,(int)shapeNum);

        }
        //
        // Remove the entity's BoxCollider
        //
        public static void RemoveCollider(Entity entity)
        {
            if (ColliderCollection.ContainsKey(entity))
            {
                //
                // found the collider, remove it and add again (in case there was an update)
                //
                ColliderCollection.Remove(entity);
            }
        }
        //
        // Test collision with other "registered" BoxColliders
        //
        public static bool CollidedWithBox(Entity entity,  out CollisionResult _collisionResult)
        {
            _collisionResult = new CollisionResult();
            BoxCollider bx = entity.Get<BoxCollider>();
            //
            // if entity has no boxcollider then exit
            //
            if (bx == null)                         // incase collider was removed
                return false;

            AABB boxA = bx.CollisionBox;

            foreach (KeyValuePair<Entity, int> entry in ColliderCollection)
            {
                if (entry.Key == entity)
                    continue;
                if (entry.Value != (int)CollidreShape.Box)
                    continue;

                Entity ent = entry.Key;
                bx = ent.Get<BoxCollider>();
                if (bx == null)                     //incase collider was removed
                    return false;

                if (boxA.Overlaps(bx.CollisionBox))
                {
                    _collisionResult.CompEntity = entry.Key;
                    //_collisionResult.BoxContainer = entry.Value;
                    //_collisionResult.CollisionArea = Raylib.GetCollisionRec(boxA, entry.Value);
                    return true;
                }
            }
            
            return false;
        }
    }

    public struct CollisionResult
    {
        public Entity CompEntity;
        public AABB BoxContainer;
        public Rectangle CollisionArea;
        public bool Collided;

    }
}
