using System;

namespace WorldNavigator
{
    public class Vertex
    {
        public float x, y, z;
        public Vertex(float X, float Y, float Z)
        {
            x = X;
            y = Y;
            z = Z;
        }

        public void MoveFront(float rotation, float distance)
        {
            x -= (float)Math.Sin(rotation / 180 * Math.PI) * distance;
            z += (float)Math.Cos( - rotation / 180 * Math.PI) * distance;
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
    }

}
