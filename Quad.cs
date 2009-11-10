using CsGL.OpenGL;

namespace WorldNavigator
{
    public class Quad
    {
        public Vector[] vertices;
        public MyColor color;

        public bool HasTexture = false;
        public uint texture;

        public string name;

        public Quad(Vector a, Vector b, Vector c, Vector d, MyColor col)
        {
            vertices = new Vector[4];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            vertices[3] = d;

            color = col;
            name = "";
        }

        public Quad(Vector a, Vector b, Vector c, Vector d, MyColor col, string n)
        {
            vertices = new Vector[4];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            vertices[3] = d;

            color = col;
            name = n;
        }

        public Quad(Vector a, Vector b, Vector c, Vector d, string n, uint t)
        {
            vertices = new Vector[4];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            vertices[3] = d;

            name = n;
            texture = t;
            HasTexture = true;
        }

        public void Draw()
        {  
            if (HasTexture)
            {
                GL.glBindTexture(GL.GL_TEXTURE_2D, texture);
                GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_DECAL);
                GL.glBegin(GL.GL_QUADS);				// start drawing a quad

                GL.glTexCoord2f(0F, 0F);
                GL.glVertex3f(vertices[0].x, vertices[0].y, vertices[0].z);
                GL.glTexCoord2f(1F, 0F);
                GL.glVertex3f(vertices[1].x, vertices[1].y, vertices[1].z);
                GL.glTexCoord2f(1F, 1F);
                GL.glVertex3f(vertices[2].x, vertices[2].y, vertices[2].z);
                GL.glTexCoord2f(0F, 1F);
                GL.glVertex3f(vertices[3].x, vertices[3].y, vertices[3].z);
                GL.glEnd();
            }
            else
            {
                GL.glColor3f(color.red, color.green, color.blue);
                GL.glTexEnvf(GL.GL_TEXTURE_ENV, GL.GL_TEXTURE_ENV_MODE, GL.GL_BLEND);
                GL.glBegin(GL.GL_QUADS);				// start drawing a quad
                foreach (Vector Vector in vertices)
                {
                    GL.glVertex3f(Vector.x, Vector.y, Vector.z);
                }
                GL.glEnd();
            }
        }
    }
}
