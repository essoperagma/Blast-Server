using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game_Core.Utilities
{
    public struct Vector3
    {
        public float x, y, z;

        public static readonly Vector3 zero = new Vector3(0,0,0);

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Vector3 operator +(Vector3 lhs, Vector3 other)
        {
            return new Vector3(lhs.x + other.x, lhs.y + other.y, lhs.z + other.z);
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 other)
        {
            return new Vector3(lhs.x - other.x, lhs.y - other.y, lhs.z - other.z);
        }

        public static Vector3 operator *(Vector3 lhs, float other)
        {
            return new Vector3(lhs.x*other, lhs.y*other, lhs.z*other);
        }

        public static Vector3 operator *(float multip, Vector3 rhs)
        {
            return new Vector3(rhs.x * multip, rhs.y * multip, rhs.z * multip);
        }

        public static bool operator ==(Vector3 lhs, Vector3 rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y && lhs.z == rhs.z;
        }

        public static bool operator !=(Vector3 lhs, Vector3 rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector3 Scale(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.x*rhs.x,lhs.y*rhs.y,lhs.z*rhs.z);
        }

        public float magnitude
        {
            get{return (float) Math.Sqrt(x*x + y*y + z*z); }
        }

        public Vector3 normalized
        {
            get
            {
                float mag = magnitude;
                if (mag < 0.001)
                    return zero;
                return this*(1/mag);
            }
        }
    }
}
