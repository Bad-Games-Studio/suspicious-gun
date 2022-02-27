using System;
using UnityEngine;

namespace Util
{
    public static class Vector3Extensions
    {
        public static float HorizontalDistance(Vector3 from, Vector3 to)
        {
            var a = new Vector2(from.x, from.z);
            var b = new Vector2(to.x,   to.z);
            return Vector2.Distance(a, b);
        }

        public static float VerticalDistance(Vector3 from, Vector3 to)
        {
            return Math.Abs(from.y - to.y);
        }
    }
}