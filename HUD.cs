using System;
using System.Collections.Generic;
using CsGL.OpenGL;

namespace WorldNavigator
{
    public class HUD
    {
        public double verticalResolution = 1280;
        public double horizontalResolution = 800;
        public float CrossHairRadius = 20F;

        public HUD()
        {
        }

        public void Draw()
        {
            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glPushMatrix();
            GL.glLoadIdentity();
            GL.glOrtho(0, verticalResolution, 0, horizontalResolution, -1, 1);
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glPushMatrix();
            GL.glLoadIdentity();

            //draw stuff
            GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_BLEND);

            GL.glDisable(GL.GL_DEPTH_TEST);
            GL.glDepthMask((byte)GL.GL_FALSE);

            GL.glLineWidth(5.0F);
            GL.glColor3f(0F, 0F, 0F);

            GL.glBegin(GL.GL_LINE_LOOP);
            
            //draw circle
            for (int i = 0; i < 360; i += 20)
            {
                GL.glVertex2f((float)(verticalResolution / 2 + CrossHairRadius * Math.Sin(i * Math.PI / 180.0)),(float)( horizontalResolution / 2 + CrossHairRadius * Math.Cos(i * Math.PI / 180.0)));
            }
            GL.glEnd();

            GL.glEnable(GL.GL_DEPTH_TEST);
            GL.glDepthMask((byte)GL.GL_TRUE);

            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glPopMatrix();
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glPopMatrix();
        }
    }
}
