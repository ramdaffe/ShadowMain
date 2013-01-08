using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShadowMain
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        //static var
        Vector2 CenterScreen, TopScreen, BottomScreen;


        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        MouseState mouseState;

        // Time
        float elapsedTime = 0;

        // Background
        Texture2D staticBG;
        Foreground foreLayer1;
        Foreground foreLayer2;

        // Represents the player
        Player player;

        // UI
        SpriteFont font;
        Texture2D recordFrameTexture;
        bool isRecording = false;
        float recOpacity = 0.0f;
        Texture2D cursorTexture;
        Vector2 cursorPos;

        // XUI
        private GameInput GameInput;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // A movement speed for the player
        float playerMoveSpeed;
        Vector2 burst = new Vector2(500,0);

        // TEST
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(0, 600);

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            // Set screen resolution to HD
            graphics.PreferredBackBufferHeight = Global.ScreenHeight;
            graphics.PreferredBackBufferWidth = Global.ScreenWidth;
            CenterScreen = new Vector2(640,360);
            TopScreen = new Vector2(640, 0);
            BottomScreen = new Vector2(640, 720);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            //Initialize the player class
            player = new Player();
            cursorPos = Vector2.Zero;
            playerMoveSpeed = 8.0f;

            //Initialize foreground
            foreLayer1 = new Foreground();
            foreLayer2 = new Foreground();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load static background
            staticBG = Content.Load<Texture2D>("Background\\bg1");

            // Load foreground
            foreLayer1.Initialize(Content, "Foreground\\fg1", new Vector2(0,0));
            foreLayer2.Initialize(Content, "Foreground\\fg2", new Vector2(0,0));

            // Load UI
            font = Content.Load<SpriteFont>("DINFont");
            recordFrameTexture = Content.Load<Texture2D>("Misc\\recframe");
            cursorTexture = Content.Load<Texture2D>("Misc\\cursor");

            // Load XUI
            GameInput = new GameInput((int)E_UiButton.Count, (int)E_UiAxis.Count);
            _UI.SetupControls(GameInput);
            _UI.Startup(this, GameInput);
            _UI.Screen.AddScreen(new UI.ScreenTest());

            // Load the player resources            
            Vector2 playerPosition = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
            player.Initialize(Content.Load<Texture2D>("player"), playerPosition);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // XUI
            _UI.Shutdown();
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Save the previous state of the keyboard and game pad so we can determine single key/button presses
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and gamepad and store it
            currentKeyboardState = Keyboard.GetState();

            //Update the player
            UpdatePlayer(gameTime);
            
            //Update foreground
            foreLayer1.Update();
            foreLayer2.Update();

            //Update UI
            mouseState = Mouse.GetState();
            cursorPos.X = mouseState.X;
            cursorPos.Y = mouseState.Y;

            //Update XUI
            float frameTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GameInput.Update(frameTime);
            _UI.Sprite.BeginUpdate();
            _UI.Screen.Update(frameTime);

            //TEST
            player.Position = SmoothMove(a, b, 5, gameTime);

            base.Update(gameTime);
        }

        private void UpdatePlayer(GameTime gameTime)
        {
            KeyPressed();
            player.Position.X = MathHelper.Clamp(player.Position.X, 0, GraphicsDevice.Viewport.Width - player.Width);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, 0, GraphicsDevice.Viewport.Height - player.Height);
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

        private void KeyPressed()
        {
            // Basic Movement
            if (currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.Position.X -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.Position.X += playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Up))
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if (currentKeyboardState.IsKeyDown(Keys.Down))
            {
                player.Position.Y += playerMoveSpeed;
            }
            // Rec
            if (currentKeyboardState.IsKeyDown(Keys.Space))
            {
                if (isRecording == false)
                {
                    isRecording = true;
                    recOpacity = 1.0f;
                }
                else
                {
                    isRecording = false;
                    recOpacity = 0.0f;
                }
            }



        }


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            // Draw background
            spriteBatch.Draw(staticBG, Vector2.Zero, Color.White);

            // Draw Player
            player.Draw(spriteBatch);
            
            // Draw foreground
            foreLayer1.Draw(spriteBatch);
            foreLayer2.Draw(spriteBatch);

            // Draw UI
            spriteBatch.Draw(cursorTexture, cursorPos, Color.White * 0.5f);
            spriteBatch.Draw(recordFrameTexture, CenterScreen, Color.White * recOpacity);


            // Debug text
             spriteBatch.DrawString(font, "ramda", new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

            // Draw XUI
             _UI.Sprite.Render(0);

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
