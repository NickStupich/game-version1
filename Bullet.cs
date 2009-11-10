using CsGL.OpenGL;
using System.Collections.Generic;
using System;

namespace WorldNavigator
{
    public class Bullet
    {
        public Vector position;
        public Vector velocity;

        public bool HitSomething = false;

        public Bullet(Vector pos, Vector vel)
        {
            position = pos;
            velocity = vel;
        }

        public void Move(float timeSpan, List<Quad> quads)
        {
            if (HitSomething)   //hit bullets don't move...
                return;

            float closestCollisionDistance = float.MaxValue;
            int closestCollisionIndex = -1;

            for (int i = 0; i < quads.Count; i++)
            {
                Vector ab = quads[i].vertices[0].Difference(quads[i].vertices[1]);
                Vector ac = quads[i].vertices[0].Difference(quads[i].vertices[2]);

                Vector N = ab.CrossProduct(ac);
                float d = -N.DotProduct(quads[i].vertices[0]);
                float t = -(d + position.DotProduct(N)) / (N.DotProduct(velocity));
                if (t > 0)
                    t *= -1F;
                float distance = (float)Math.Abs(t * velocity.Length());

                if (distance > closestCollisionDistance)
                    continue;

                Vector projection = position.Difference(velocity.GetScaledVector(t));

                if (!MovingTools.PointIsInQuad(projection, quads[i]))
                    continue;

                closestCollisionDistance = distance;
                closestCollisionIndex = i;
            }

            if (closestCollisionIndex >= 0 && closestCollisionDistance < velocity.GetScaledVector(timeSpan).Length())
            {
                //bullet collided with something, take the appropriate action
                    HitSomething = true;

                //move to the surface of the object
                    position.Translate(velocity.GetScaledVector(timeSpan * closestCollisionDistance / velocity.GetScaledVector(timeSpan).Length()));
            }
            else
            {
               position.Translate(velocity.GetScaledVector(timeSpan));
            }
        }

        public void Draw()
        {
            GL.glTranslatef(position.x, position.y, position.z);
            GL.glColor3f(0F, 1F, 0F);
            GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_BLEND);
                
            GLUquadric quadratic = GL.gluNewQuadric();
            GL.gluQuadricNormals(quadratic, GL.GLU_SMOOTH);
            GL.gluQuadricTexture(quadratic, (byte)GL.GL_TRUE);

            GL.gluSphere(quadratic, 0.05, 4, 5);
            GL.glTranslatef(-position.x, -position.y, -position.z);
        }
    }
}
