using System;
using UnityEngine;

namespace YuoTools.Extend.YuoMathf
{
    [Serializable]
    public struct YuoVector2Int
    {
        public bool Equals(YuoVector2Int other)
        {
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            return obj is YuoVector2Int other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public int x;
        public int y;

        public YuoVector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        [Newtonsoft.Json.JsonIgnore] public float Magnitude => Mathf.Sqrt(x * x + y * y);

        public static float Distance(YuoVector2Int a, YuoVector2Int b)
        {
            return (a - b).Magnitude;
        }

        public static bool operator !=(YuoVector2Int a, YuoVector2Int b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static bool operator ==(YuoVector2Int a, YuoVector2Int b)
        {
            return a.x == b.x && a.y == b.y;
        }


        public static YuoVector2Int operator +(YuoVector2Int a, YuoVector2Int b)
        {
            return new YuoVector2Int(a.x + b.x, a.y + b.y);
        }

        public static YuoVector2Int operator -(YuoVector2Int a, YuoVector2Int b)
        {
            return new YuoVector2Int(a.x - b.x, a.y - b.y);
        }

        public static YuoVector2Int operator *(YuoVector2Int a, int b)
        {
            return new YuoVector2Int(a.x * b, a.y * b);
        }

        public static YuoVector2Int operator /(YuoVector2Int a, int b)
        {
            return new YuoVector2Int(a.x / b, a.y / b);
        }
    }

    public struct YuoVector2
    {
        public bool Equals(YuoVector2 other)
        {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override bool Equals(object obj)
        {
            return obj is YuoVector2 other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }

        public float x;
        public float y;

        public YuoVector2(float x, float y)
        {
            this.x = x;
            this.y = y;
        }

        public float Magnitude => Mathf.Sqrt(x * x + y * y);

        public static float Distance(YuoVector2 a, YuoVector2 b)
        {
            return (a - b).Magnitude;
        }

        public static bool operator !=(YuoVector2 a, YuoVector2 b)
        {
            return !a.x.ApEqual(b.x, 0.00001f) || !a.y.ApEqual(b.y, 0.00001f);
        }

        public static bool operator ==(YuoVector2 a, YuoVector2 b)
        {
            return a.x.ApEqual(b.x, 0.00001f) && a.y.ApEqual(b.y, 0.00001f);
        }

        public static YuoVector2 operator +(YuoVector2 a, YuoVector2 b)
        {
            return new YuoVector2(a.x + b.x, a.y + b.y);
        }

        public static YuoVector2 operator -(YuoVector2 a, YuoVector2 b)
        {
            return new YuoVector2(a.x - b.x, a.y - b.y);
        }

        public static YuoVector2 operator *(YuoVector2 a, float b)
        {
            return new YuoVector2(a.x * b, a.y * b);
        }

        public static YuoVector2 operator /(YuoVector2 a, float b)
        {
            return new YuoVector2(a.x / b, a.y / b);
        }

        public static implicit operator YuoVector2Int(YuoVector2 a)
        {
            return new YuoVector2Int((int)a.x, (int)a.y);
        }
    }

    public struct YuoVector3
    {
        public float x;
        public float y;
        public float z;

        public YuoVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        public static float Distance(YuoVector3 a, YuoVector3 b)
        {
            return (a - b).Magnitude;
        }

        public static YuoVector3 operator +(YuoVector3 a, YuoVector3 b)
        {
            return new YuoVector3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static YuoVector3 operator -(YuoVector3 a, YuoVector3 b)
        {
            return new YuoVector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static YuoVector3 operator *(YuoVector3 a, float b)
        {
            return new YuoVector3(a.x * b, a.y * b, a.z * b);
        }

        public static YuoVector3 operator /(YuoVector3 a, float b)
        {
            return new YuoVector3(a.x / b, a.y / b, a.z / b);
        }
    }

    public struct YuoVector3Int
    {
        public int x;
        public int y;
        public int z;

        public YuoVector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float Magnitude => Mathf.Sqrt(x * x + y * y + z * z);

        public static float Distance(YuoVector3Int a, YuoVector3Int b)
        {
            return (a - b).Magnitude;
        }

        public static YuoVector3Int operator +(YuoVector3Int a, YuoVector3Int b)
        {
            return new YuoVector3Int(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static YuoVector3Int operator -(YuoVector3Int a, YuoVector3Int b)
        {
            return new YuoVector3Int(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static YuoVector3Int operator *(YuoVector3Int a, int b)
        {
            return new YuoVector3Int(a.x * b, a.y * b, a.z * b);
        }

        public static YuoVector3Int operator /(YuoVector3Int a, int b)
        {
            return new YuoVector3Int(a.x / b, a.y / b, a.z / b);
        }
    }
}