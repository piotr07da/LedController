using System;

namespace LedController3Client.Mathematics
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

        public Vector ClosestPointOnSegment(Vector s0, Vector s1, out float ratio)
        {
            var sdx = s1.X - s0.X;
            var sdy = s1.Y - s0.Y;

            if ((sdx == 0f) && (sdy == 0f))
            {
                ratio = 0f;
                return s0;
            }

            ratio = ((X - s0.X) * sdx + (Y - s0.Y) * sdy) / (sdx * sdx + sdy * sdy);

            if (ratio < 0f)
            {
                ratio = 0f;
                return s0;
            }
            else if (ratio > 1f)
            {
                ratio = 1f;
                return s1;
            }
            return new Vector((float)Math.Round(s0.X + ratio * sdx), (float)Math.Round(s0.Y + ratio * sdy));
        }

        public static Vector operator -(Vector lhs, Vector rhs)
        {
            return new Vector(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector operator *(float lhs, Vector Vector)
        {
            return Vector * lhs;
        }

        public static Vector operator *(Vector lhs, float rhs)
        {
            return new Vector(lhs.X * rhs, lhs.Y * rhs);
        }
    }
}

