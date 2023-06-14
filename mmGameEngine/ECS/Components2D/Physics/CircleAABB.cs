using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace mmGameEngine
{
    public class CircleAABB
    {
        public Vector2 Center;
        public float Radius;
        public Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
        public Vector2 max = new Vector2(float.MinValue, float.MinValue);

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
        public float Magnitude(Vector2 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
        }
        public float MagnitudeSqr(Vector2 v)
        {
            return v.X * v.X + v.Y * v.Y;
        }
        public float Distance(Vector2 original, Vector2 compareTo)
        {
            float diffX = original.X - compareTo.X;
            float diffY = original.Y - compareTo.Y;
            return (float)Math.Sqrt(diffX * diffX + diffY * diffY);
        }
        public Vector2 GetNormalized(Vector2 v)
        {
            return (v / Magnitude(v));
        }
        public float DotProduct(Vector2 lhs, Vector2 rhs)
        {
            return lhs.X * rhs.X + lhs.Y * rhs.Y;
        }
        #endregion

        public CircleAABB()
        {
            // Purposefully Blank.
        }

        public CircleAABB(Vector2 p, float r)
        {
            Center = p;
            Radius = r;
        }

        public void Fit(Vector2[] points)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            for (int i = 0; i < points.Length; i++)
            {
                min = Vector2.Min(min, points[i]);
                max = Vector2.Max(max, points[i]);
            }

            Center = (min + max) * 0.5f;
            Radius = Distance(Center, max);
        }

        public void Fit(List<Vector2> points)
        {
            //Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            //Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (Vector2 p in points)
            {
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }

            Center = (min + max) * 0.5f;
            Radius = Distance(Center, max);
        }

        // TODO: Another method of fitting a Sphere to a collection of points is to first find the average 
        //       position within the collection and set it to the Sphere’s Center, then set the Radius to the 
        //       distance between the Center and the point farthest from the Center.This method requires looping 
        //       through the points multiple times.
        //
        // Try and implement this second method yourself.

        // Collision Checks --------------------
        public bool Overlaps(Vector2 p)
        {
            Vector2 toPoint = p - Center;
            return MagnitudeSqr(toPoint) <= (Radius * Radius);
        }

        public bool Overlaps(CircleAABB otherCollider)
        {
            Vector2 diff = otherCollider.Center - Center;
            float r = Radius + otherCollider.Radius;
            return MagnitudeSqr(diff) <= (r * r);
        }
        //
        // Circle check with collision box 
        //
        public bool Overlaps(BoxAABB aabb)
        {
            Vector2 diff = aabb.ClosestPoint(Center) - Center;
            return DotProduct(diff, diff) <= (Radius * Radius);
        }

        Vector2 ClosestPoint(Vector2 p)
        {
            // Distance from the Center.
            Vector2 toPoint = p - Center;

            // If outside the Radius, bring it back to the Radius.
            if (MagnitudeSqr(toPoint) > (Radius * Radius))
            {
                toPoint = GetNormalized(toPoint) * Radius;
            }
            return Center + toPoint;
        }
    }
}
