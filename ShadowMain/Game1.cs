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

        // XNA Graphics
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        // XNA Input: mouse states and keyboard states
        MouseState mouseState;
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

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
        int SelectedID;

        // Record layer
        Texture2D recordFrameTexture;
        float recOpacity = 0.0f;

        // Debugging
        string debugmsg = "";

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
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Save previous keyboard state into temporary values and read the current new one
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            
            //Update UI
            mouseState = Mouse.GetState();
            cursorPos.X = mouseState.X;
            cursorPos.Y = mouseState.Y;

            //Update Stage
            stage.Update();

            //Menu Update
            mainmenu.Update();

            //Detect Hover
            DetectHover(gameTime);

            //Counter
            eT++;
            TimeTrigger(eT);

            base.Update(gameTime);
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
            
            // Draw UI
            spriteBatch.Draw(cursorTexture, cursorPos, Color.White * 0.5f);
            //spriteBatch.Draw(recordFrameTexture, CenterScreen, Color.White * recOpacity);

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
