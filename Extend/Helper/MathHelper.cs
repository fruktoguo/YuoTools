using System;

namespace YuoTools.Extend.Helper
{
    public static class MathHelper
    {
        public static float RadToDeg(float radians)
        {
            return (float) (radians * 180 / Math.PI);
        }

        public static float DegToRad(float degrees)
        {
            return (float) (degrees * Math.PI / 180);
        }
    }
}