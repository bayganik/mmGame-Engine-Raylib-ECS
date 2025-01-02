using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;


namespace Entitas
{
    //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
    //               Extending Entity to act like a Node in a Scene
    //ZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZNZN
    public partial class Entity
    {
        public bool IsDirty = true;            //needs to recalculate where the Entity is
        Vector3 _position3D;
        public Vector3 Position3D
        {
            get
            {
                return _position3D;
            }
            set
            {
                _position3D = value;
                IsDirty= true;
            }
        }

        Vector2 _position;
        public Vector2 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                IsDirty = true;
            }
        }

        //public Quaternion Rotation3D
        //{
        //    get
        //    {
        //        return GetRotation();
        //    }
        //    set
        //    {
        //        SetRotation(value);
        //    }
        //}
        float _rotation;
        public float Rotation
        {
            get
            {
                return _rotation;
            }
            set
            {
                _rotation = value;
                IsDirty = true;
            }
        }

        public Vector2 Direction;
        Vector2 _scale;
        public Vector2 Scale
        {
            get
            {
                return _scale;
            }
            set
            {
                _scale = value;
                IsDirty = true;
            }
        }
        /// <summary>
        /// Is this entity visible
        /// </summary>
        bool _isVisible;
        public bool IsVisible { get { return _isVisible; } set { _isVisible = value; } }
        /// <summary>
        /// Tags all enemy entities. this is used for collision detections (missile fired that needs to clear its own buildings)
        /// bool is used for collision detection (so friendly entity cannot collide together)
        /// </summary>
        bool _isEnemy;  
        public bool IsEnemy { get { return _isEnemy; } set { _isEnemy = value; } }
    }
}
