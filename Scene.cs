using System.Collections.Generic;
using System.IO;
using System;
using CsGL.OpenGL;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;

namespace WorldNavigator
{
    enum TextureType
    {
        linear = 1,
        nearest = 2,
    }
    public class Scene
    {
        [DllImport("User32.dll")]
        private static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        #region objects in scene
        public List<Triangle> triangles;
        public List<Quad> quads;
        public List<Bullet> bullets;
        public uint[] textures;
        #endregion

        #region movement settings/variables

        public Vector cameraPosition = new Vector(0F, 0F, 0F);
        public Vector cameraVelocity = new Vector(0F, 0F, 0F);
        bool onGround = true;
        public float lookAngleX = 0F;
        public float lookAngleY = 0F;

        protected bool finished = false;

        public bool InvertUpDown = true;

        public float MinDistanceToObject = 0.8F;
        public float OnGroundDistance = 1.2F;

        public float mouseSensitivity = 0.05F;
        public float moveSpeed = 0.005F;
        public int lastMouseX, lastMouseY;
        TextureType textureType = TextureType.nearest;

        float GRAVITY = 0.00001F;   //m/ms^2??
        float MAX_GRAVITY_SPEED = -0.01F;   //ms/ms??
        float JUMP_UP_SPEED = 0.006F;
        #endregion

        #region update thread and timing settings
        Thread updateThread;
        public double updatesTimeNum = 0;
        public double updatesTimeDenom = 0;
        double smoothingFactor = 0.99;
        int timeBetweenUpdates = 10;    //ms
        #endregion

        HUD hud;        

        #region constructors and helpers
        public Scene()
        {
            triangles = new List<Triangle>();
            quads = new List<Quad>();
            bullets = new List<Bullet>();
        }

        public Scene(string inputFilename)
        {
            triangles = new List<Triangle>();
            quads = new List<Quad>();
            bullets = new List<Bullet>();
            hud = new HUD();

            StreamReader reader = new FileInfo(inputFilename).OpenText();
            string line;

            Dictionary<string, Vector> loadedVertices = new Dictionary<string, Vector>();
            Dictionary<string, int> loadedTextures = new Dictionary<string, int>();
            Dictionary<string, float> values = new Dictionary<string,float>();

            int texturesCount = 0;
            textures = new uint[texturesCount];

            int lineCounter = 0;

            while (true)
            {
                line = reader.ReadLine();
                
                if (line == null)
                    break;

                line = System.Text.RegularExpressions.Regex.Replace(line, "#.*", "", System.Text.RegularExpressions.RegexOptions.Compiled);

                lineCounter++;

                if (line == "")
                    continue;

                if (line.IndexOf("clear vertices") == 0)
                    loadedVertices = new Dictionary<string, Vector>();

                else if (line.IndexOf("start") == 0)
                {
                    #region start
                    string[] parts = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length != 6)
                    {
                        throw new Exception("line.split for start did not make the right length array. line " + lineCounter.ToString());
                    }

                    cameraPosition.x = GetFloat(parts[1], values);
                    cameraPosition.y = GetFloat(parts[2], values);
                    cameraPosition.z = GetFloat(parts[3], values);

                    lookAngleX = GetFloat(parts[4], values);
                    lookAngleY = GetFloat(parts[5], values);
                    #endregion
                }
                else if (line.Contains("vertex"))
                {
                    #region vertex
                    try
                    {
                        string[] parts = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length != 5)
                            throw new Exception("line.Split for Vector did not make the right length array. line " + lineCounter.ToString());

                        string name = parts[1];
                        float x = GetFloat(parts[2], values);
                        float y = GetFloat(parts[3], values);
                        float z = GetFloat(parts[4], values);

                        loadedVertices.Add(name, new Vector(x, y, z));
                    }
                    catch (Exception e)
                    {
                        throw new Exception("error parsing input file. line " + lineCounter.ToString() + " . message: " + e.Message);
                    }
                    #endregion
                }
                else if (line.Contains("quad"))
                {
                    #region quad
                    string[] parts;
                    parts = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 10)
                    {
                        throw new Exception("line.split for quad was not the right length. line: " + lineCounter.ToString());
                    }

                    if (parts[5] == "color")
                    {
                        float red = GetFloat(parts[6], values);
                        float green = GetFloat(parts[7], values);
                        float blue = GetFloat(parts[8], values);

                        MyColor color = new MyColor(red, green, blue);

                        string name = parts[9];

                        Vector v1 = loadedVertices[parts[1]];
                        Vector v2 = loadedVertices[parts[2]];
                        Vector v3 = loadedVertices[parts[3]];
                        Vector v4 = loadedVertices[parts[4]];


                        Quad quad = new Quad(v1, v2, v3, v4, color, name);
                        quads.Add(quad);
                    }
                    else if (parts[5] == "tex")
                    {
                        Vector v1 = loadedVertices[parts[1]];
                        Vector v2 = loadedVertices[parts[2]];
                        Vector v3 = loadedVertices[parts[3]];
                        Vector v4 = loadedVertices[parts[4]];

                        string textureName = parts[6];
                        string name = parts[7];

                        Quad quad = new Quad(v1, v2, v3, v4, name, (uint)loadedTextures[textureName]);
                        quads.Add(quad);
                    }
                    #endregion
                }
                else if (line.Contains("triangle"))
                {
                    #region triangle
                    string[] parts;
                    try
                    {
                        parts = line.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length < 6)
                            throw new Exception("line.split for triangle did not make the right length array. line " + lineCounter.ToString());
                    }
                    catch (Exception e)
                    {
                        throw new Exception("error splitting triangle line in input file. line " + lineCounter.ToString() + " . message: " + e.Message);
                    }

                    try
                    {
                        if (parts[4] == "color")
                        {
                            float red = GetFloat(parts[5], values);
                            float green = GetFloat(parts[6], values);
                            float blue = GetFloat(parts[7], values);

                            MyColor color = new MyColor(red, green, blue);

                            Vector v1 = loadedVertices[parts[1]];
                            Vector v2 = loadedVertices[parts[2]];
                            Vector v3 = loadedVertices[parts[3]];

                            Triangle tri = new Triangle(v1, v2, v3, color);
                            triangles.Add(tri);
                        }
                        else if (parts[4] == "texture")
                        {
                            Vector v1 = loadedVertices[parts[1]];
                            Vector v2 = loadedVertices[parts[2]];
                            Vector v3 = loadedVertices[parts[3]];

                            string textureName = parts[5];

                            Triangle tri = new Triangle(v1, v2, v3, (uint)loadedTextures[textureName]);
                            triangles.Add(tri);
                        }
                    }
                    catch (Exception e)
                    {
                        throw new Exception("error creating triangle in input file. line: " + lineCounter.ToString() + " . message: " + e.Message);
                    }
                    #endregion
                }
                else if (line.Contains("texture"))
                {
                    #region texture
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string file = parts[1];
                    string name = parts[2];

                    texturesCount++;
                    loadedTextures[name] = texturesCount;
                    LoadTexture(file, texturesCount);
                    #endregion
                }
                else if (line.Contains("value"))
                {
                    #region value
                    string[] parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    string name = parts[1];
                    float value = float.Parse(parts[2]);

                    values[name] = value;
                    #endregion
                }
                else if(line.Contains("clearValues"))
                {
                    values = new Dictionary<string,float>();
                }
                else
                {
                    throw new Exception("line " + lineCounter.ToString() + " not understood");
                }

            }

            reader.Close();

            lastMouseX = Cursor.Position.X;
            lastMouseY = Cursor.Position.Y;
        }
        /*
                public static Scene TriangularPrismScene()
                {
                    Scene scene = new Scene();

                    Vector top = new Vector(0.0F, 1.0F, 0.0F);
                    Vector fL = new Vector(1.0F, -1.0F, 1.0F);
                    Vector fR = new Vector(-1.0F, -1.0F, 1.0F);
                    Vector bR = new Vector(-1.0F, -1.0F, -1.0F);
                    Vector bL = new Vector(1.0F, -1.0F, -1.0F);

                    scene.AddTriangle(new Triangle(fL, fR, bR, new MyColor(0.5F, 0.5F, 0.5F)));
                    scene.AddTriangle(new Triangle(fL, bL, bR, new MyColor(0.5F, 0.5F, 0.5F)));
                    scene.AddTriangle(new Triangle(fL, fR, top, new MyColor(1.0F, 0.0F, 0.0F)));
                    scene.AddTriangle(new Triangle(fR, bR, top, new MyColor(0.0F, 1.0F, 0.0F)));
                    scene.AddTriangle(new Triangle(bR, bL, top, new MyColor(0.0F, 0.0F, 1.0F)));
                    scene.AddTriangle(new Triangle(bL, fL, top, new MyColor(1.0F, 1.0F, 1.0F)));

                    return scene;
                }
         * 
         * public void AddTriangle(Triangle triangle)
                {
                    triangles.Add(triangle);
                }
        */
        public float GetFloat(string value, Dictionary<string, float> valueDict)
        {
            float result;
            if (value.Contains("+"))
            {
                string[] values = value.Split('+');
                result = 0;
                foreach (string v in values)
                {
                    result += GetFloat(v, valueDict);
                }
                return result;
            }
            else if (float.TryParse(value, out result))
                return result;

            else
                return valueDict[value];
        }

        public void LoadTexture(string filename, int textureCount)
        {
            uint[] temp = textures;
            textures = new uint[textureCount];
            for (int i = 0; i < textureCount - 1; i++)
                textures[i] = temp[i];

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(filename);

            if (image != null)
            {
                System.Drawing.Imaging.BitmapData bitmapdata = image.LockBits(new System.Drawing.Rectangle(0, 0, image.Width, image.Height),
                    System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                if (textureCount > 1)
                    textures[textureCount - 1] = textures[textureCount - 2] + 1;
                else
                    textures[0] = 1;

                GL.glBindTexture(GL.GL_TEXTURE_2D, textures[textureCount - 1]);
                GL.glTexImage2D(GL.GL_TEXTURE_2D, 0, (int)GL.GL_RGB8, image.Width, image.Height,
                    0, GL.GL_BGR_EXT, GL.GL_UNSIGNED_BYTE, bitmapdata.Scan0);

                if (textureType == TextureType.linear)
                {
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_LINEAR);		// Linear Filtering
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_LINEAR);		// Linear Filtering
                }
                else if (textureType == TextureType.nearest)
                {
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MIN_FILTER, GL.GL_NEAREST);
                    GL.glTexParameteri(GL.GL_TEXTURE_2D, GL.GL_TEXTURE_MAG_FILTER, GL.GL_NEAREST);
                }

                image.UnlockBits(bitmapdata);
                image.Dispose();
            }
            else
            {
                throw new Exception("image is null");
            }

        }

        
        #endregion

        #region processing input
        public bool ProcessInput(float timeSinceLastDraw)
        {
            ProcessMouseInput(timeSinceLastDraw);
            return ProcessKeyboardInput(timeSinceLastDraw); //false if exit
        }

        public void ProcessMouseInput(float timeSinceLastDraw)
        {
            #region position
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;

            lookAngleX -= (lastMouseX - x) * mouseSensitivity;

            if (InvertUpDown)
                lookAngleY += (lastMouseY - y) * mouseSensitivity;
            else
                lookAngleY -= (lastMouseY - y) * mouseSensitivity;

            if (lookAngleY > 30)
                lookAngleY = 30;
            else if (lookAngleY < -30)
                lookAngleY = -30;

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
            #endregion

            #region clicks

            //use mouse events for this?

            #endregion
        }

        public bool ProcessKeyboardInput(float timeSinceLastDraw)
        {
            if (KeyIsDown(Keys.Escape))
            {
                return false;
            }

            #region move around
            if (KeyIsDown(Keys.Up))
            {
                MoveObserverFront(moveSpeed * timeSinceLastDraw);
            }
            if (KeyIsDown(Keys.Down))
            {
                MoveObserverFront(-moveSpeed * timeSinceLastDraw);
            }
            if (KeyIsDown(Keys.Right))
            {
                MoveObserverRight(moveSpeed * timeSinceLastDraw);
            }
            if (KeyIsDown(Keys.Left))
            {
                MoveObserverRight(-moveSpeed * timeSinceLastDraw);
            }

            return true;
            #endregion
        }

        public void ProcessMouseClick(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right) //jump
            {
                if(onGround)
                    cameraVelocity.y += JUMP_UP_SPEED;
            }
            else if (e.Button == MouseButtons.Left)  //shoot
            {
                Bullet bullet = new Bullet(new Vector(cameraPosition.x, cameraPosition.y, cameraPosition.z), Vector.CreateByAngles(lookAngleX, lookAngleY, 0.1F));
                bullets.Add(bullet);
            }
        }
        
        public bool KeyIsDown(System.Windows.Forms.Keys key)
        {
            return 0 != (GetAsyncKeyState(key) & 0x8000);
        }
        
        #endregion
        
        #region movements
        public void MoveObserverFront(float distance)
        {
            cameraPosition.MoveFront(lookAngleX, -distance);

            foreach (Quad quad in quads)
            {
                bool hitWall = MovingTools.AdjustPointToNotGoThroughQuad(MinDistanceToObject, ref cameraPosition, quad);
            }
        }

        public void MoveObserverRight(float distance)
        {
            cameraPosition.MoveRight(lookAngleX, -distance);

            foreach (Quad quad in quads)
            {
                bool hitWall = MovingTools.AdjustPointToNotGoThroughQuad(MinDistanceToObject, ref cameraPosition, quad);
            }
        }

        public void MoveObserver(Vector moveAmounts)
        {
            cameraPosition.Translate(moveAmounts);

            foreach (Quad quad in quads)
            {
                bool hitWall = MovingTools.AdjustPointToNotGoThroughQuad(MinDistanceToObject, ref cameraPosition, quad);
            }
        }

        public void ProcessGravity(float timeSinceLastDraw)
        {
            cameraVelocity.y -= timeSinceLastDraw * GRAVITY;
            if (cameraVelocity.y < MAX_GRAVITY_SPEED)
                cameraVelocity.y = MAX_GRAVITY_SPEED;
        }

        #endregion

        public void SelfUpdateThreadStart()
        {
            updateThread = new Thread(UpdateThread);
            updateThread.Start();
        }

        protected void UpdateThread()
        {
            DateTime lastUpdateTime = DateTime.Now;
            while (!finished)
            {
                int timeSinceLast = (int)(DateTime.Now - lastUpdateTime).TotalMilliseconds;

                if (timeSinceLast < timeBetweenUpdates)
                {
                    Thread.Sleep(timeBetweenUpdates - timeSinceLast);
                }

                float timeGap = (float)(DateTime.Now - lastUpdateTime).TotalMilliseconds;

                updatesTimeNum = updatesTimeNum * smoothingFactor + timeGap;
                updatesTimeDenom = updatesTimeDenom * smoothingFactor + 1;

                lastUpdateTime = DateTime.Now;

                Update(timeGap);
            }
        }
        
        public void Update(float timeSinceLastDraw)
        {
            onGround = MovingTools.DistanceToGround(cameraPosition, quads) <= OnGroundDistance;

            if (!onGround)
                ProcessGravity(timeSinceLastDraw);
            else
            {
                if (cameraVelocity.y < 0)
                    cameraVelocity.y = 0F;
            }

            MoveObserver(cameraVelocity.GetScaledVector(timeSinceLastDraw));

            for (int i = 0; i < bullets.Count; i++)
            {
                bullets[i].Move(timeSinceLastDraw, quads);
            }

            finished = !ProcessInput(timeSinceLastDraw);
        }

        public bool IsFinished()
        {
            return finished;
        }

        public void Draw()
        {
            foreach (Triangle triangle in triangles)
            {
                triangle.Draw();
            }

            foreach (Quad quad in quads)
            {
                quad.Draw();
            }
            foreach (Bullet bullet in bullets)
            {
                bullet.Draw();
            }

            hud.Draw();
            
        }
    }
}