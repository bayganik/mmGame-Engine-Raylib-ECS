using System;
using System.Collections.Generic;
using System.Text;
using System.Numerics;

namespace mmGameEngine
{
    public class Circle
    {
        Vector2 center;
        float radius;

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

        public Circle()
        {
            // Purposefully Blank.
        }

        public Circle(Vector2 p, float r)
        {
            center = p;
            radius = r;
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

            center = (min + max) * 0.5f;
            radius = Distance(center, max);
        }

        public void Fit(List<Vector2> points)
        {
            Vector2 min = new Vector2(float.MaxValue, float.MaxValue);
            Vector2 max = new Vector2(float.MinValue, float.MinValue);

            foreach (Vector2 p in points)
            {
                min = Vector2.Min(min, p);
                max = Vector2.Max(max, p);
            }

            center = (min + max) * 0.5f;
            radius = Distance(center, max);
        }

        // TODO: Another method of fitting a Sphere to a collection of points is to first find the average 
        //       position within the collection and set it to the Sphere’s center, then set the radius to the 
        //       distance between the center and the point farthest from the center.This method requires looping 
        //       through the points multiple times.
        //
        // Try and implement this second method yourself.

        // Collision Checks --------------------
        public bool Overlaps(Vector2 p)
        {
            Vector2 toPoint = p - center;
            return MagnitudeSqr(toPoint) <= (radius * radius);
        }

        public bool Overlaps(Circle otherCollider)
        {
            Vector2 diff = otherCollider.center - center;
            float r = radius + otherCollider.radius;
            return MagnitudeSqr(diff) <= (r * r);
        }
        //
        // Circle check with collision box 
        //
        public bool Overlaps(AABB aabb)
        {
            Vector2 diff = aabb.ClosestPoint(center) - center;
            return DotProduct(diff, diff) <= (radius * radius);
        }

        Vector2 ClosestPoint(Vector2 p)
        {
            // Distance from the center.
            Vector2 toPoint = p - center;

            // If outside the radius, bring it back to the radius.
            if (MagnitudeSqr(toPoint) > (radius * radius))
            {
                toPoint = GetNormalized(toPoint) * radius;
            }
            return center + toPoint;
        }
    }
}
