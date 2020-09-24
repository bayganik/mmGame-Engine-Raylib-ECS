using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace mmGameEngine
{

    public class AABB
    {
        public Vector2 min = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        public Vector2 max = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

        #region FromVec3
        public static Vector2 Min(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Min(a.X, b.X), Math.Min(a.Y, b.Y));
        }

        public static Vector2 Max(Vector2 a, Vector2 b)
        {
            return new Vector2(Math.Max(a.X, b.X), Math.Max(a.Y, b.Y));
        }

        public static Vector2 Clamp(Vector2 t, Vector2 a, Vector2 b)
        {
            return Max(a, Min(b, t));
        }
        #endregion

        public AABB()
        {
            // Purposefully Blank.
        }

        public AABB(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }
        public void Resize(Vector2 min, Vector2 max)
        {
            this.min = min;
            this.max = max;
        }
        /// <summary>
        /// Create an AABB from a list of points.
        /// </summary>
        /// <param name="points">List from which to make the AABB.</param>
        public void Fit(List<Vector2> points)
        {
            // Invalidate Min and Max so new values can replace them.
            min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            foreach (Vector2 p in points)
            {
                min = Min(min, p);
                max = Max(max, p);
            }
        }

        /// <summary>
        /// Create an AABB from an array of points.
        /// </summary>
        /// <param name="points">Array from which to make the AABB.</param>
        public void Fit(Vector2[] points)
        {
            // Invalidate Min and Max so new values can replace them.
            min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);

            foreach (Vector2 p in points)
            {
                min = Min(min, p);
                max = Max(max, p);
            }
        }

        // Collision Checks --------------------

        // It's faster to check if they DONT collide since it'll excit the check sooner.
        public bool Overlaps(Vector2 p)
        {
            return !(p.X < min.X || p.Y < min.Y || p.X > max.X || p.Y > max.Y);
        }
        //
        // Collision box check
        //
        public bool Overlaps(AABB otherCollider)
        {
            return !(max.X < otherCollider.min.X || max.Y < otherCollider.min.Y || min.X > otherCollider.max.X || min.Y > otherCollider.max.Y);
        }

        public bool Overlaps(Circle circleCol)
        {
            return true; //placeholder
                         // TODO: Figure out how to make the overlap check for this.
        }

        public Vector2 ClosestPoint(Vector2 p)
        {
            return Clamp(p, min, max);
        }

        // Finding other information from the AABB
        public Vector2 Center()
        {
            return (min + max) * 0.5f;
        }

        public Vector2 Extents()
        {
            return new Vector2(Math.Abs(max.X = min.X) * 0.5f, Math.Abs(max.Y = min.Y) * 0.5f);
        }

        public List<Vector2> Corners()
        {
            List<Vector2> corners = new List<Vector2>();
            corners.Add(min);// = min;
            corners.Add( new Vector2(min.X, max.Y));
            corners.Add(max);
            corners.Add( new Vector2(max.X, min.Y));
            return corners;
        }
    }
}
