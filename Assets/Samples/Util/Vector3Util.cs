using System;
using UnityEngine;

namespace Pico.Platform.Samples
{
    [Serializable]
    public class SerializedVector3
    {
        public float x;
        public float y;
        public float z;

        public SerializedVector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public SerializedVector3(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }
    }

    public static class V3
    {
        public static Vector3 ToVector3(this SerializedVector3 serializedVector3)
        {
            return new Vector3(serializedVector3.x, serializedVector3.y, serializedVector3.z);
        }

        public static SerializedVector3 FromVector3(this Vector3 vector3)
        {
            return new SerializedVector3(vector3);
        }
    }
}