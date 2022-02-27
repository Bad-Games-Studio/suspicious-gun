using System;
using UnityEngine;

namespace Util
{
    public static class CoolMath
    {
        public static float Degrees(double radians)
        {
            var result = radians * Mathf.Rad2Deg;
            return (float) result;
        }
        
        public static float Radians(double degrees)
        {
            var result = degrees * Mathf.Deg2Rad;
            return (float) result;
        }
    }
}