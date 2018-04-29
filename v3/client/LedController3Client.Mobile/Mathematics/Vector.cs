using System;

namespace LedController3Client.Mobile.Mathematics
{
    public class Vector
    {
        public Vector(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float X { get; set; }
        public float Y { get; set; }

        public void Normalize()
        {
            var mag = Magnitude();
            X /= mag;
            Y /= mag;
        }

        public float Magnitude()
        {
            return (float)Math.Sqrt(X * X + Y * Y);
        }

        public float AngleFrom(Vector relativeVector)
        {
            double sin = relativeVector.X * Y - relativeVector.Y * X;
            double cos = relativeVector.X * X + relativeVector.Y * Y;

            return (float)Math.Atan2(sin, cos);
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }
    }
}

