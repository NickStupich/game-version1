/* Lesson 05 of NeHe Productions in C#
   created by Sabine Felsinger*/

using System;
using System.Windows.Forms;
using System.Threading;

namespace WorldNavigator
{
	public class MainForm : System.Windows.Forms.Form	
	{
		private WorldNavigator.View view;
        
		public MainForm()
		{
            this.TopMost = true;
			this.view = new WorldNavigator.View();
            if (view.maximized)
            {
                this.WindowState = FormWindowState.Maximized;
                //this.FormBorderStyle = FormBorderStyle.None;
                Cursor.Hide();
            }
            else
            {
                this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
                this.ClientSize = new System.Drawing.Size(640, 480);
                Cursor.Hide();
            }
            
			this.view.Parent = this;
			this.view.Dock = DockStyle.Fill; // Will fill whole form
            
         	this.Show();
		}

		static void Main() 
		{
			MainForm form = new MainForm();

            DateTime lastDrawTime = DateTime.Now;
            double fpsNum = 0, fpsDenom = 0, smoothingFactor = 0.99;
            //int timeBetweenFrames = 25;
            form.Focus();

			while ((!form.view.finished) && (!form.IsDisposed))		// refreshing the window
			{
                //int timeSinceLastDraw = (DateTime.Now - lastDrawTime).Milliseconds;
                //if (timeSinceLastDraw < timeBetweenFrames)
                //    Thread.Sleep(timeBetweenFrames - timeSinceLastDraw);
                
                    double fps = 1000.0 / (DateTime.Now - lastDrawTime).TotalMilliseconds;
                    fpsNum = fpsNum * smoothingFactor + fps;
                    fpsDenom = fpsDenom * smoothingFactor + 1;

                    form.Text = "( " + ((int)(form.view.scene.cameraPosition.x)).ToString() + " - " + ((int)(form.view.scene.cameraPosition.z)).ToString() + " - " + ((int)(form.view.scene.cameraPosition.y)).ToString()
                        + " ) - ( " + ((int)(form.view.scene.lookAngleX)).ToString() + " - " + ((int)(form.view.scene.lookAngleY)).ToString() + " )  FPS: " + ((int)(fpsNum / fpsDenom)).ToString()
                        + "  -  Update time: " + ((int)(form.view.scene.updatesTimeNum / form.view.scene.updatesTimeDenom)).ToString();
                
                lastDrawTime = DateTime.Now;

                form.view.glDraw();
				form.Refresh();
				Application.DoEvents();
			}
            form.Dispose();
		}
	}
}