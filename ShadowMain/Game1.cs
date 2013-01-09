using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Kinect;
using ScapLIB;
using AForge.Video;
using AForge.Video.FFMPEG;

namespace ShadowMain
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        // XNA Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // XNA Input: mouse states
        MouseState mouseState;

        // Keyboard Input
        KeyControl keyboard;

        //Time counter
        float elapsedTime = 0;

        // Menu layer
        Menu mainmenu;

        // Stage layer
        Stage stage;

        // Recorder module
        Recorder rec;

        // Cursor & GUI layer
        SpriteFont font;
        Texture2D cursorTexture;
        Vector2 cursorPos;
        int SelectedID = 3;

        // Record layer
        Texture2D recordFrameTexture;
        float recOpacity = 0.0f;

        //Kinect
        KinectSensor kinect;
        Texture2D colorVideo, depthVideo, jointTexture;
        Skeleton[] skeletonData;
        Skeleton skeleton;


        // Debugging
        bool debugging = true;
        string debugmsg = "";
        bool toggled = false;

        // Counter
        int eT = 0;

        public Game1()
        {
            // Start graphic
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferHeight = Global.ScreenHeight;
            graphics.PreferredBackBufferWidth = Global.ScreenWidth;

            // Point content
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Init form
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);

            //Initialize cursor
            cursorPos = Vector2.Zero;

            //Initialize stage
            stage = new Stage();

            //Initialize menu layer
            mainmenu = new Menu();

            //Init ScapLIB
            rec = new Recorder();
            rec.Initialize(0, 0);

            //Init input handling
            keyboard = new KeyControl();

            //Init Kinect
            try
            {
                kinect = KinectSensor.KinectSensors[0];
                //kinect.ColorStream.Enable(ColorImageFormat.RgbResolution640x480Fps30);
                kinect.DepthStream.Enable(DepthImageFormat.Resolution320x240Fps30);
                kinect.SkeletonStream.Enable();
                kinect.AllFramesReady += new EventHandler<AllFramesReadyEventArgs>(kinect_AllFramesReady);

                kinect.Start();
                colorVideo = new Texture2D(graphics.GraphicsDevice, kinect.ColorStream.FrameWidth, kinect.ColorStream.FrameHeight);
                depthVideo = new Texture2D(graphics.GraphicsDevice, kinect.DepthStream.FrameWidth, kinect.DepthStream.FrameHeight);
                Debug.WriteLineIf(debugging, kinect.Status);
            }
            catch (Exception e)
            {
                debugmsg = "error";
                Debug.WriteLine(e.ToString());
            }

            //Finally, base
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create a new spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load main UI asset
            font = Content.Load<SpriteFont>("DINFont");
            recordFrameTexture = Content.Load<Texture2D>("Misc\\recframe");
            cursorTexture = Content.Load<Texture2D>("Misc\\cursor");

            // Load Menu
            mainmenu.Initialize(Content);
            stage.Initialize(Content);

            // Load Kinect joint marker
            jointTexture = Content.Load<Texture2D>("Kinect\\joint");
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Save previous keyboard state into temporary values and read the current new one
            keyboard.Update();
            
            //Update UI
            mouseState = Mouse.GetState();
            cursorPos.X = mouseState.X;
            cursorPos.Y = mouseState.Y;

            //Update Stage
            stage.Update();

            //Menu Update
            mainmenu.Update();

            //Detect Hover
            DetectHover();

            //Counter
            eT++;
            KeyTrigger();
            //TimeTrigger(eT);

            base.Update(gameTime);
        }
        public void KeyTrigger()
        {
            if (keyboard.IsToggled(Microsoft.Xna.Framework.Input.Keys.Space) && !toggled)
            {
                toggled = true;
                debugmsg = "toggled";
            }
            if (toggled && keyboard.currentKeyboardState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                debugmsg = "";
                toggled = false;
            }
                
        }
        public void TimeTrigger(int time)
        {
            if (time == 60)
            {
                rec.Record();
                debugmsg = "start recording";
            }

            if (time == 240)
            {
                rec.StopAndDecompress();
                debugmsg = "decompressing";
            }

            if (time == 500)
            {
                rec.Encode();
                debugmsg = "encoding";
            }
            if (rec.IsEncodeFinished())
            {
                debugmsg = "finished";
            }
        }


        public void DetectHover()
        {
            if (Vector2.Distance(cursorPos, mainmenu.NewButton.Hotspot) < Global.HoverTolerance)
            {
                mainmenu.selButtonID = 1;
            }
            if (Vector2.Distance(cursorPos, mainmenu.LoadButton.Hotspot) < Global.HoverTolerance)
            {
                mainmenu.selButtonID = 2;
            }
             if (Vector2.Distance(cursorPos, mainmenu.HelpButton.Hotspot) < Global.HoverTolerance)
            {
                mainmenu.selButtonID = 3;
            }
        }

        private Vector2 SmoothMove(Vector2 initPos, Vector2 endPos, int animDuration, GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += dt;
            if (elapsedTime > 1)
                elapsedTime = 1;

            float param = elapsedTime / animDuration;
            return Vector2.Lerp(initPos, endPos, (float)Math.Pow(param / 2.0, 0.5));
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            // Draw stage
            stage.Draw(spriteBatch);

            // Draw Skeleton
            DrawSkeleton(spriteBatch, new Vector2(graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), jointTexture);
            
            // Draw UI
            spriteBatch.Draw(cursorTexture, cursorPos, Color.White * 0.5f);
            //spriteBatch.Draw(recordFrameTexture, CenterScreen, Color.White * recOpacity);

            // Debug text
            spriteBatch.DrawString(font, mainmenu.NewButton.Hotspot.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(font, mainmenu.selButtonID.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y + 200), Color.White);
            spriteBatch.DrawString(font, cursorPos.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y + 300), Color.White);
            spriteBatch.DrawString(font, debugmsg,new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 300, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

            // Draw menu and button
             //mainmenu.Draw(spriteBatch);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);

        }

        void kinect_AllFramesReady(object sender, AllFramesReadyEventArgs imageFrames)
        {
            //
            // Color Frame 
            //
            /*
            //Get raw image
            ColorImageFrame colorVideoFrame = imageFrames.OpenColorImageFrame();

            if (colorVideoFrame != null)
            {
                //Create array for pixel data and copy it from the image frame
                Byte[] pixelData = new Byte[colorVideoFrame.PixelDataLength];
                colorVideoFrame.CopyPixelDataTo(pixelData);

                //Convert RGBA to BGRA
                Byte[] bgraPixelData = new Byte[colorVideoFrame.PixelDataLength];
                for (int i = 0; i < pixelData.Length; i += 4)
                {
                    bgraPixelData[i] = pixelData[i + 2];
                    bgraPixelData[i + 1] = pixelData[i + 1];
                    bgraPixelData[i + 2] = pixelData[i];
                    bgraPixelData[i + 3] = (Byte)255; //The video comes with 0 alpha so it is transparent
                }

                // Create a texture and assign the realigned pixels
                colorVideo = new Texture2D(graphics.GraphicsDevice, colorVideoFrame.Width, colorVideoFrame.Height);
                colorVideo.SetData(bgraPixelData);
            }*/

            //
            // Depth Frame
            //
            DepthImageFrame depthVideoFrame = imageFrames.OpenDepthImageFrame();

            if (depthVideoFrame != null)
            {
                Debug.WriteLineIf(debugging, "Frame");
                //Create array for pixel data and copy it from the image frame
                short[] pixelData = new short[depthVideoFrame.PixelDataLength];
                depthVideoFrame.CopyPixelDataTo(pixelData);

                for (int i = 0; i < 10; i++)
                { Debug.WriteLineIf(debugging, pixelData[i]); }

                // Convert the Depth Frame
                // Create a texture and assign the realigned pixels
                //
                depthVideo = new Texture2D(graphics.GraphicsDevice, depthVideoFrame.Width, depthVideoFrame.Height);
                depthVideo.SetData(ConvertDepthFrame(pixelData, kinect.DepthStream));

            }

            //
            // Skeleton Frame
            //
            using (SkeletonFrame skeletonFrame = imageFrames.OpenSkeletonFrame())
            {
                if (skeletonFrame != null)
                {
                    if ((skeletonData == null) || (this.skeletonData.Length != skeletonFrame.SkeletonArrayLength))
                    {
                        this.skeletonData = new Skeleton[skeletonFrame.SkeletonArrayLength];
                    }

                    //Copy the skeleton data to our array
                    skeletonFrame.CopySkeletonDataTo(this.skeletonData);
                }
            }

            if (skeletonData != null)
            {
                foreach (Skeleton skel in skeletonData)
                {
                    if (skel.TrackingState == SkeletonTrackingState.Tracked)
                    {
                        skeleton = skel;
                    }
                }
            }

        }

        private void DrawSkeleton(SpriteBatch spriteBatch, Vector2 resolution, Texture2D img)
        {
            if (skeleton != null)
            {
                foreach (Joint joint in skeleton.Joints)
                {
                    Vector2 position = new Vector2((((0.5f * joint.Position.X) + 0.5f) * (resolution.X)), (((-0.5f * joint.Position.Y) + 0.5f) * (resolution.Y)));
                    spriteBatch.Draw(img, new Rectangle(Convert.ToInt32(position.X), Convert.ToInt32(position.Y), 10, 10), Color.Red);
                }
            }
        }

        private byte[] ConvertDepthFrame(short[] depthFrame, DepthImageStream depthStream)
        {
            int RedIndex = 0, GreenIndex = 1, BlueIndex = 2, AlphaIndex = 3;

            byte[] depthFrame32 = new byte[depthStream.FrameWidth * depthStream.FrameHeight * 4];

            for (int i16 = 0, i32 = 0; i16 < depthFrame.Length && i32 < depthFrame32.Length; i16++, i32 += 4)
            {
                int player = depthFrame[i16] & DepthImageFrame.PlayerIndexBitmask;
                int realDepth = depthFrame[i16] >> DepthImageFrame.PlayerIndexBitmaskWidth;

                // transform 13-bit depth information into an 8-bit intensity appropriate
                // for display (we disregard information in most significant bit)
                byte intensity = (byte)(~(realDepth >> 4));

                depthFrame32[i32 + RedIndex] = (byte)(intensity);
                depthFrame32[i32 + GreenIndex] = (byte)(intensity);
                depthFrame32[i32 + BlueIndex] = (byte)(intensity);
                depthFrame32[i32 + AlphaIndex] = 255;
            }

            return depthFrame32;
        }
    }
}
