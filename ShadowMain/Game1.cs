using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using ScapLIB;
using AForge.Video;
using AForge.Video.FFMPEG;

namespace ShadowMain
{

    public class Game1 : Microsoft.Xna.Framework.Game
    {

        //screen direction variable
        Vector2 CenterScreen, TopScreen, BottomScreen;

        // XNA Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // XNA Input: mouse states and keyboard states
        MouseState mouseState;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        //Time counter
        float elapsedTime = 0;

        //Main layers
        Texture2D staticBG;
        Foreground foreLayer1;
        Foreground foreLayer2;

        // Menu layer
        Menu mainmenu;

        // Cursor & GUI layer
        SpriteFont font;
        Texture2D cursorTexture;
        Vector2 cursorPos;
        int SelectedID;

        // Record layer
        Texture2D recordFrameTexture;
        bool isRecording = false;
        float recOpacity = 0.0f;

        // TEST
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(0, 600);
        string debugmsg = "";

        //TEST Screen Capture
        ScapCapture Cap;
        bool isCapture = false;
        int eT = 0;
        int Xlocation = 0;
        int Ylocation = 0;

        public Game1()
        {
            //Form
            //var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            //form.Location = new System.Drawing.Point(0, 0);
            //Control control = Control.FromHandle(this.Window.Handle);
            //control 
            //Graphics
            graphics = new GraphicsDeviceManager(this);
            // Set screen resolution to HD
            graphics.PreferredBackBufferHeight = Global.ScreenHeight;
            graphics.PreferredBackBufferWidth = Global.ScreenWidth;
            
            CenterScreen = new Vector2(Global.ScreenWidth / 2, Global.ScreenHeight / 2);
            TopScreen = new Vector2(Global.ScreenWidth / 2, 0);
            BottomScreen = new Vector2(Global.ScreenWidth / 2, Global.ScreenHeight);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            //Init form
            var form = (System.Windows.Forms.Form)System.Windows.Forms.Form.FromHandle(this.Window.Handle);
            form.Location = new System.Drawing.Point(0, 0);
            Xlocation = form.Location.X;
            Ylocation = form.Location.Y;
            Cap = new ScapCapture(false, 5, 10, ScapVideoFormats.MPEG4, ScapImageFormats.Jpeg, Xlocation, Ylocation, 1280, 720);

            //Initialize cursor
            cursorPos = Vector2.Zero;

            //Initialize foreground layer
            foreLayer1 = new Foreground();
            foreLayer2 = new Foreground();

            //Initialize menu layer
            mainmenu = new Menu();

            //Init ScapLIB
            ScapBackendConfig.ScapBackendSetup(Cap);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create a new spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load background layer content
            staticBG = Content.Load<Texture2D>("Background\\bg1");

            // Load foreground layer content
            foreLayer1.Initialize(Content, "Foreground\\fg1", new Vector2(0,0));
            foreLayer2.Initialize(Content, "Foreground\\fg2", new Vector2(0,0));

            // Load Record layer content
            font = Content.Load<SpriteFont>("DINFont");
            recordFrameTexture = Content.Load<Texture2D>("Misc\\recframe");
            cursorTexture = Content.Load<Texture2D>("Misc\\cursor");

            // Load Menu
            mainmenu.Initialize(Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Save previous keyboard state into temporary values and read the current new one
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            
            //Update foreground
            foreLayer1.Update();
            foreLayer2.Update();

            //Update UI
            mouseState = Mouse.GetState();
            cursorPos.X = mouseState.X;
            cursorPos.Y = mouseState.Y;

            //Detect Hover
            DetectHover(gameTime);

            //Test Capture
            eT++;
            TimeTrigger(eT);


            base.Update(gameTime);
        }

        public void TimeTrigger(int time)
        {
            if (time == 60)
            {
                ScapCore.StartCapture();
                debugmsg = "start recording";
            }

            if (time == 240) 
            {
                debugmsg = "stop recording";
                ScapCore.StopCapture();
                ScapCore.DecompressCapture(false);
                debugmsg = "start decompressing";
            }

            if (time == 500)
            {
                ScapCore.EncodeCapture(false);
                debugmsg = "start encoding";
            }

            if (ScapCore.GetEncodeProgress() == 1d)
            {
                debugmsg = "finished";
            
            }

        }
      

        public void DetectHover(GameTime gameTime)
        {
            //TEST HOVER
            //new button
            SelectedID = 0;
            if (Vector2.Distance(cursorPos, mainmenu.NewButtonHotPos) < Global.HoverTolerance)
            {
                //mainmenu.SetSelected(1);
                SelectedID = 1;
                mainmenu.SetNew(Vector2.Zero);
            }
            
            if (Vector2.Distance(cursorPos, mainmenu.LoadButtonHotPos) < Global.HoverTolerance)
            {
                //mainmenu.SetSelected(2);
                SelectedID = 2;
            }
             if (Vector2.Distance(cursorPos, mainmenu.HelpButtonHotPos) < Global.HoverTolerance)
            {
                //mainmenu.SetSelected(3);
                SelectedID = 3;
            }
            //SelectedID = mainmenu.GetSelected();
        }

        private void AnimManager(GameTime gameTime)
        { }

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

            // Draw background
            spriteBatch.Draw(staticBG, Vector2.Zero, Color.White);
            
            // Draw foreground
            foreLayer1.Draw(spriteBatch);
            foreLayer2.Draw(spriteBatch);

            // Draw UI
            spriteBatch.Draw(cursorTexture, cursorPos, Color.White * 0.5f);
            spriteBatch.Draw(recordFrameTexture, CenterScreen, Color.White * recOpacity);

            // Debug text
             spriteBatch.DrawString(font, eT.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
             spriteBatch.DrawString(font, debugmsg, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 300, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

            // Draw menu and button
             mainmenu.Draw(spriteBatch);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);

        }
    }
}
