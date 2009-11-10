using System;
using System.Collections.Generic;
using System.Text;

namespace WorldNavigator
{
    public class Vector
    {
        public float x, y, z;

        public Vector(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public static Vector CreateByAngles(float rotationAngleX, float rotationAngleY, float vectorLength)
        {
            float x = (float)(Math.Sin(rotationAngleX * Math.PI / 180.0) * Math.Cos(rotationAngleY * Math.PI / 180.0));
            float y = -(float)Math.Sin(rotationAngleY * Math.PI / 180.0);
            float z = -(float)(Math.Cos(rotationAngleX * Math.PI / 180.0) *  Math.Cos(rotationAngleY * Math.PI / 180.0));

            Vector result = new Vector(x, y, z);

            float scalingFactor = vectorLength / result.Length();
            result.x *= scalingFactor;
            result.y *= scalingFactor;
            result.z *= scalingFactor;

            return result;
        }

        public Vector CrossProduct(Vector v2)
        {
            Vector result = new Vector(this.y * v2.z - this.z * v2.y, this.z * v2.x - this.x * v2.z, this.x * v2.y - this.y * v2.x);
            return result;
        }

        public Vector Difference(Vector v2)
        {
            return new Vector(this.x - v2.x, this.y - v2.y, this.z - v2.z);
        }

        public float DotProduct(Vector v2)
        {
            float result = this.x * v2.x + this.y * v2.y + this.z * v2.z;
            return result;
        }

        public float Length()
        {
            return (float)Math.Sqrt(x * x + y * y + z * z);
        }

        public void MoveFront(float rotation, float distance)
        {
            x -= (float)Math.Sin(rotation / 180 * Math.PI) * distance;
            z += (float)Math.Cos(-rotation / 180 * Math.PI) * distance;
        }

        public void MoveRight(float rotation, float distance)
        {
            x -= (float)Math.Cos(rotation / 180 * Math.PI) * distance;
            z -= (float)Math.Sin(rotation / 180 * Math.PI) * distance;
        }

        public void Translate(float dx, float dy, float dz)
        {
            x += dx;
            y += dy;
            z += dz;
        }

        public void Translate(Vector v2)
        {
            x += v2.x;
            y += v2.y;
            z += v2.z;
        }

        public Vector GetScaledVector(float scalingFactor)
        {
            return new Vector(x * scalingFactor, y * scalingFactor, z * scalingFactor);
        }
    }
}
