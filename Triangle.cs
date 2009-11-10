using CsGL.OpenGL;

namespace WorldNavigator
{

    public class Triangle
    {
        public Vector[] vertices;
        public MyColor color;
        public bool HasTexture;
        public uint texture;

        public Triangle(Vector a, Vector b, Vector c)
        {
            vertices = new Vector[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            color = new MyColor(0.5F, 0.5F, 0.5F);
            HasTexture = false;
        }

        public Triangle(Vector a, Vector b, Vector c, MyColor col)
        {
            vertices = new Vector[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            color = col;
            HasTexture = false;
        }
        public Triangle(Vector a, Vector b, Vector c, uint t)
        {
            vertices = new Vector[3];
            vertices[0] = a;
            vertices[1] = b;
            vertices[2] = c;
            texture = t;
            HasTexture = true;
        }

        public void Draw()
        {
            GL.glBegin(GL.GL_TRIANGLES);			// start drawing a triangle, always counterclockside (top-left-right)
            
            if (HasTexture)
            {
                GL.glBindTexture(GL.GL_TEXTURE_2D, texture);
            }
            else
            {
                GL.glColor3f(color.red, color.green, color.blue);
            }
            
            foreach (Vector Vector in vertices)
            {
                GL.glVertex3f(Vector.x, Vector.y, Vector.z);
            }
            GL.glEnd();
        }
    }

}
