using System;
using System.Collections.Generic;
using CsGL.OpenGL;
using System.Windows.Forms;
using System.Drawing;
using System.Runtime.InteropServices;

namespace WorldNavigator
{
    public class View : OpenGLControl
    {
        public Scene scene;

        public bool maximized = true;

        public bool finished;

        DateTime lastDrawTime;

        public View()
            : base()
        {
            finished = false;
            this.MouseDown += new MouseEventHandler(View_MouseDown);
        }

        void View_MouseDown(object sender, MouseEventArgs e)
        {
            if (scene != null)
            {
                scene.ProcessMouseClick(e);
            }
        }

        public override void glDraw()
        {
            float timeSinceLastDraw = (float)(DateTime.Now - lastDrawTime).TotalMilliseconds;
            lastDrawTime = DateTime.Now;

            GL.glClear(GL.GL_COLOR_BUFFER_BIT | GL.GL_DEPTH_BUFFER_BIT);	// Clear the Screen and the Depth Buffer
            GL.glMatrixMode(GL.GL_MODELVIEW);		// Modelview Matrix

            GL.glLoadIdentity();					// reset the current modelview matrix

            GL.glRotatef(scene.lookAngleX, 0.0f, 1.0f, 0.0f); //rotate the viewing angle
            GL.glRotatef(scene.lookAngleY, (float)Math.Cos(scene.lookAngleX * Math.PI / 180.0), 0.0F, (float)Math.Sin(scene.lookAngleX * Math.PI / 180.0));
            
            GL.glTranslatef(-scene.cameraPosition.x, -scene.cameraPosition.y, -scene.cameraPosition.z);

            //scene.Update(timeSinceLastDraw);
            finished = scene.IsFinished();
            scene.Draw();
        }
/*
        public void ProcessInput(float timeSinceLastDraw)
        {
            ProcessKeyboardInput(timeSinceLastDraw);
            ProcessMouseInput(timeSinceLastDraw);
        }

        public void ProcessMouseInput(float timeSinceLastDraw)
        {
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            
                scene.lookAngleX -= (lastMouseX - x) * mouseSensitivity;

            if (InvertUpDown)
                scene.lookAngleY += (lastMouseY - y) * mouseSensitivity;
            else
                scene.lookAngleY -= (lastMouseY - y) * mouseSensitivity;

            if (scene.lookAngleY > 30)
                scene.lookAngleY = 30;
            else if (scene.lookAngleY < -30)
                scene.lookAngleY = -30;

            if (x > Screen.PrimaryScreen.Bounds.Width / 2 + 100
                || x < Screen.PrimaryScreen.Bounds.Width / 2 - 100
                || y > Screen.PrimaryScreen.Bounds.Height / 2 + 100
                || y < Screen.PrimaryScreen.Bounds.Height / 2 - 100)
            {
                lastMouseX = Screen.PrimaryScreen.Bounds.Width / 2;
                lastMouseY = Screen.PrimaryScreen.Bounds.Height / 2;
                Cursor.Position = new Point(Screen.PrimaryScreen.Bounds.Width / 2, Screen.PrimaryScreen.Bounds.Height / 2);
                return;
            }

            lastMouseX = x;
            lastMouseY = y;

        }

        public void ProcessKeyboardInput(float timeSinceLastDraw)
        {
            if (KeyIsDown(Keys.Escape))
            {
                finished = true;
                return;
            }

            #region move around
            if (KeyIsDown(Keys.Up))
            {
                scene.MoveObserverFront(moveSpeed * timeSinceLastDraw);
            }
            if (KeyIsDown(Keys.Down))
            {
                scene.MoveObserverFront(-moveSpeed * timeSinceLastDraw);
            }
            if (KeyIsDown(Keys.Right))
            {
                scene.MoveObserverRight(moveSpeed * timeSinceLastDraw);
            }
            if (KeyIsDown(Keys.Left))
            {
                scene.MoveObserverRight(-moveSpeed * timeSinceLastDraw);
            }
            #endregion
        }

        public bool KeyIsDown(System.Windows.Forms.Keys key)
        {
            return 0 != (GetAsyncKeyState(key) & 0x8000);
        }
*/
        protected override void InitGLContext()
        {
            GL.glEnable(GL.GL_TEXTURE_2D);						// Enable Texture Mapping ( NEW )
            GL.glShadeModel(GL.GL_SMOOTH);						// Enable Smooth Shading
            GL.glClearColor(0.0f, 0.0f, 0.0f, 0.5f);					// Black Background
            GL.glClearDepth(1.0f);							// Depth Buffer Setup
            GL.glEnable(GL.GL_DEPTH_TEST);						// Enables Depth Testing
            GL.glDepthFunc(GL.GL_LEQUAL);							// The Type Of Depth Testing To Do
            GL.glHint(GL.GL_PERSPECTIVE_CORRECTION_HINT, GL.GL_NICEST);			// Really Nice Perspective Calculations

            scene = new Scene("World1.txt");
            lastDrawTime = DateTime.Now;

            scene.SelfUpdateThreadStart();
        }

        protected override void OnSizeChanged(EventArgs e)
        {
            base.OnSizeChanged(e);
            Size size = Size;

            GL.glMatrixMode(GL.GL_PROJECTION);
            GL.glLoadIdentity();
            GL.gluPerspective(45.0f, (double)size.Width / (double)size.Height, 0.1f, 100.0f);
            GL.glMatrixMode(GL.GL_MODELVIEW);
            GL.glLoadIdentity();
        }

        
        
    }
}
