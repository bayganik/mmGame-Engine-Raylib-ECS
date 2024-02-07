using Entitas;
using System;
using System.Collections.Generic;
using System.Text;

using System.Numerics;

using Raylib_cs;

namespace mmGameEngine
{
    /*
     * Database of BoxColliders in this scene. The manager checks to see if an entity collided with anyother.
     * The colliders must get updated when entity position changes.
     */
    public static class SceneColliderManager
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
            ColliderCollection.Add(entity, (int)shapeNum);

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
        public static bool CollidedWithBox(Entity entity, out CollisionResult _collisionResult)
        {
            //
            // currently not used
            //
            _collisionResult = new CollisionResult();
            //
            // Find the Entity in question, if not in database then no collision
            //
            if (!ColliderCollection.ContainsKey(entity))
                return false;
            //
            // Find out what kind of collider entity has?
            //
            BoxCollider bx = entity.Get<BoxCollider>();
            //CircleCollider cx = entity.Get<CircleCollider>();

            BoxAABB boxA = bx.CollisionBox;
            //Vector2 boxA = new Vector2(bx.CollisionBox.X, bx.CollisionBox.Y);
            //
            // Find entity and ask

            //
            // Test collision with other "registered" BoxColliders
            //
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
                    _collisionResult.OwnerEntity = entry.Key;
                    _collisionResult.Collided = true;

                    //_collisionResult.BoxContainer = entry.Value;
                    //_collisionResult.CollisionArea = Raylib.GetCollisionRec(boxA, entry.Value);
                    return true;
                }
            }

            return false;
        }
        //
        // Test collision with other "registered" BoxColliders
        //

    }

    public struct CollisionResult
    {
        public Entity OwnerEntity;
        public BoxAABB BoxContainer;
        public Rectangle CollisionArea;
        public bool Collided;
        public CollisionResult(bool _collided = false)
        {
            Collided = false;
            OwnerEntity = null;
            BoxContainer = new BoxAABB();
            CollisionArea = new Rectangle(0, 0, 0, 0);
        }
    }
}
