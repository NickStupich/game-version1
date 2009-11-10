using System;
using System.Collections.Generic;
using System.Text;

namespace WorldNavigator
{
    public class MovingTools
    {
        public static bool AdjustPointToNotGoThroughQuad(float MinDistance, ref Vector observer, Quad quad)
        {
            Vector ab = quad.vertices[0].Difference(quad.vertices[1]);
            Vector ad = quad.vertices[0].Difference(quad.vertices[3]);
            Vector N = ab.CrossProduct(ad);

            float d = -N.DotProduct(quad.vertices[0]);
            float t = -(d + observer.DotProduct(N)) / (N.DotProduct(N));
            float Distance = (float)Math.Abs(t * N.Length());

            if (Distance > MinDistance)
                return false;   //too far away, regardless of where you are relative to the wall

            Vector projectionOntoTriangle = new Vector(observer.x + t * N.x, observer.y + t * N.y, observer.z + t * N.z);

            if (!PointIsInQuad(projectionOntoTriangle, quad))
                return false;   //outside the quad

            //the point needs to be moved
            float NormalMultiplier = MinDistance / N.Length();
            if (N.x / (observer.x - projectionOntoTriangle.x) < 0
                || N.y / (observer.y - projectionOntoTriangle.y) < 0
                || N.z / (observer.z - projectionOntoTriangle.z) < 0)
                NormalMultiplier *= -1.0F;

            observer = new Vector(projectionOntoTriangle.x + N.x * NormalMultiplier, projectionOntoTriangle.y + N.y * NormalMultiplier, projectionOntoTriangle.z + N.z * NormalMultiplier);
            return true;
        }

        public static bool PointIsInQuad(Vector projection, Quad quad)
        {
            float Error = 0.1F;

            if (projection.x > quad.vertices[0].x + Error && projection.x > quad.vertices[1].x + Error && projection.x > quad.vertices[2].x + Error)
                return false;
            if (projection.y > quad.vertices[0].y + Error && projection.y > quad.vertices[1].y + Error && projection.y > quad.vertices[2].y + Error)
                return false;
            if (projection.z > quad.vertices[0].z + Error && projection.z > quad.vertices[1].z+ Error && projection.z > quad.vertices[2].z + Error)
                return false;

            if (projection.x < quad.vertices[0].x - Error && projection.x < quad.vertices[1].x - Error && projection.x < quad.vertices[2].x - Error)
                return false;
            if (projection.y < quad.vertices[0].y - Error && projection.y < quad.vertices[1].y - Error && projection.y < quad.vertices[2].y - Error)
                return false;
            if (projection.z < quad.vertices[0].z - Error && projection.z < quad.vertices[1].z - Error && projection.z < quad.vertices[2].z - Error)
                return false;

            return true;
        }

        public static float DistanceToGround(Vector observer, List<Quad> quads)
        {
            float closestGround = float.MaxValue;
            Vector down = new Vector(0F, -1F, 0F);

            foreach (Quad quad in quads)
            {
                Vector ab = quad.vertices[0].Difference(quad.vertices[1]);
                Vector ac = quad.vertices[0].Difference(quad.vertices[2]);
                Vector N = ab.CrossProduct(ac);
                float d = -N.DotProduct(quad.vertices[0]);
                float t = -(d + observer.DotProduct(N)) / (N.DotProduct(down));
                if (t > 0)
                    t *= -1F;
                float distance = (float)Math.Abs(t * down.Length());

                Vector projection = observer.Difference(down.GetScaledVector(t));

                if (!PointIsInQuad(projection, quad))
                    continue;

                if (distance < closestGround)
                    closestGround = distance;
            }

            return closestGround;
        }

    }
}
