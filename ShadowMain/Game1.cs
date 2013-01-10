using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Xml.Serialization;
//using System.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Kinect;
//using Microsoft.Speech; -> need x86 version, not x64

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
        Texture2D StaticBG;

        // Record layer
        Texture2D recordFrameTexture;
        float recOpacity = 0.0f;

        // Player layer
        Player player;

        // GameState
        // 0 = main menu
        // 1 = recording stage
        int currentstate = 0;


        //Save IO
        // Savedata structure - IN PROGRESS
        public struct SaveData
        {
            public string bgID;
            public string skinID;
            public string foreID;
        }
        enum SavingState
        {
            NotSaving,
            ReadyToSelectStorageDevice,
            SelectingStorageDevice,
            ReadyToOpenStorageContainer,
            OpeningStorageContainer,
            ReadyToSave
        }
        StorageDevice storageDevice;
        SavingState savingState = SavingState.NotSaving;
        IAsyncResult asyncResult;
        PlayerIndex playerIndex = PlayerIndex.One;
        StorageContainer storageContainer;
        string filename = "savegame.sav";
        SaveData saveGameData = new SaveData()
        {
            bgID = "x",
            skinID = "y",
            foreID = "z"
        };

        // Debugging
        string debugmsg = "ready";
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
            player = new Player();
            player.Initialize(graphics);

            //Finally, base
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // create a new spritebatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load main UI asset
            font = Content.Load<SpriteFont>("gameFont");
            recordFrameTexture = Content.Load<Texture2D>("Misc\\recframe");
            cursorTexture = Content.Load<Texture2D>("Misc\\cursor");
            // Init static bg
            StaticBG = Content.Load<Texture2D>("Background\\bg1");

            // Load Menu
            mainmenu.Initialize(Content);
            stage.Initialize(Content);

            // Load Kinect joint marker
            player.LoadContent(Content);
            
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            // Save previous keyboard state into temporary values and read the current new one
            keyboard.previousKeyboardState = keyboard.currentKeyboardState;
            keyboard.currentKeyboardState = Keyboard.GetState();
            
            //Update UI
            mouseState = Mouse.GetState();
            if (player.ready)
            {
                player.Update();
                cursorPos.X = player.pointerPosX;
                cursorPos.Y = player.pointerPosY;            
            }


            //Update Stage
            stage.Update();

            //Menu Update
            mainmenu.Update(gameTime,elapsedTime);

            //Detect Hover
            DetectHover();
            CheckLevel();

            //Counter
            eT++;
            
            if (rec.isRecording)
            {
                debugmsg = "recording";
            }
            if (rec.isDecompressing)
            {
                debugmsg = "decompressing" + rec.GetDecProg().ToString();
            }
            if (rec.isEncoding)
            {
                debugmsg = "encoding" + rec.GetEncProg().ToString();
            }
            RecordTrigger(gameTime);    
            //TimeTrigger(eT);

            //debugmsg
            //debugmsg = player.status;

            //save
            UpdateSaveKey(Microsoft.Xna.Framework.Input.Keys.F1);
            UpdateSaving();

            base.Update(gameTime);
        }

        public void RecordTrigger(GameTime g)
        {
            
            if (keyboard.IsToggled(Microsoft.Xna.Framework.Input.Keys.Space) && !rec.isRecording && !rec.isDecompressing)
            {
                rec.isRecording = true;
                rec.Record();
            } 
            if (keyboard.isPressed(Microsoft.Xna.Framework.Input.Keys.Space) && rec.isRecording)
            {
                rec.isRecording = false;
                rec.StopAndDecompress();
                rec.isDecompressing = true;
            }
            if (keyboard.IsToggled(Microsoft.Xna.Framework.Input.Keys.Enter) && rec.isDecompressing)
            {
                rec.isDecompressing = false;
                rec.Encode();
                rec.isEncoding = true;
            } 

            /*
            if (keyboard.IsToggled(Microsoft.Xna.Framework.Input.Keys.P) && !rec.isRecording && !rec.isDecompressing)
            {
                rec.isRecording = true;
                rec.Record();
            } 

            if(rec.GetEncProg() == 1d)
            {
                rec.isEncoding = false;
                rec.isFileReady = true;
            } 
            if (rec.isFileReady)
            {
                debugmsg = "file ready";
                rec.isEncoding = false;
                rec.isDecompressing = false;
                rec.isRecording = false;
                rec.isFileReady = false;
            }*/
                
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
        public void CheckLevel()
        {
            if (mainmenu.NewButton.IsSelected)
            {
                currentstate = 1;
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

        

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            spriteBatch.Draw(StaticBG, Vector2.Zero, Color.White);

            // Draw stage
            if (currentstate == 1)
            {
                stage.Draw(spriteBatch);
                player.Draw(spriteBatch);
            }
            else
            {
                mainmenu.Draw(spriteBatch);
            }

            // Debug text
            spriteBatch.DrawString(font, mainmenu.NewButton.IsSelected.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);
            spriteBatch.DrawString(font, mainmenu.selButtonID.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y + 200), Color.White);
            spriteBatch.DrawString(font, player.pointerPosX.ToString() + player.pointerPosY.ToString(), new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 100, GraphicsDevice.Viewport.TitleSafeArea.Y + 300), Color.White);
            spriteBatch.DrawString(font, debugmsg, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X + 300, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

            // Draw menu and button
             

             // Draw UI
             spriteBatch.Draw(cursorTexture, cursorPos, Color.White * 0.5f);
             if (rec.isRecording)
             {
                 spriteBatch.Draw(recordFrameTexture, Vector2.Zero, Color.White);
             }

            //Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);

        }


        /// <summary>
        /// Helper methods : Save/Load
        /// </summary>
        public void UpdateSaveKey(Microsoft.Xna.Framework.Input.Keys key)
        {
            if (keyboard.isPressed(key))
            {
                if (savingState == SavingState.NotSaving)
                {
                    savingState = SavingState.ReadyToOpenStorageContainer;
                    Debug.WriteLine("Updatesavekey");
                }
            }
        }

        public void UpdateSaving()
        {
            switch (savingState)
            {
                case SavingState.ReadyToSelectStorageDevice:
                    {
                        asyncResult = StorageDevice.BeginShowSelector(playerIndex, null, null);
                        savingState = SavingState.SelectingStorageDevice;
                    }
                    break;

                case SavingState.SelectingStorageDevice:
                    if (asyncResult.IsCompleted)
                    {
                        storageDevice = StorageDevice.EndShowSelector(asyncResult);
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    break;

                case SavingState.ReadyToOpenStorageContainer:
                    if (storageDevice == null || !storageDevice.IsConnected)
                    {
                        savingState = SavingState.ReadyToSelectStorageDevice;
                    }
                    else
                    {
                        asyncResult = storageDevice.BeginOpenContainer("Game1StorageContainer", null, null);
                        savingState = SavingState.OpeningStorageContainer;
                    }
                    break;

                case SavingState.OpeningStorageContainer:
                    if (asyncResult.IsCompleted)
                    {
                        storageContainer = storageDevice.EndOpenContainer(asyncResult);
                        savingState = SavingState.ReadyToSave;
                    }
                    break;

                case SavingState.ReadyToSave:
                    if (storageContainer == null)
                    {
                        savingState = SavingState.ReadyToOpenStorageContainer;
                    }
                    else
                    {
                        try
                        {
                            DeleteExisting();
                            Save();
                        }
                        catch (IOException e)
                        {
                            // Replace with in game dialog notifying user of error
                            Debug.WriteLine(e.Message);
                        }
                        finally
                        {
                            storageContainer.Dispose();
                            storageContainer = null;
                            savingState = SavingState.NotSaving;
                        }
                    }
                    break;
            }
        }

        private void DeleteExisting()
        {
            if (storageContainer.FileExists(filename))
            {
                storageContainer.DeleteFile(filename);
            }
        }

        private void Save()
        {
            using (Stream stream = storageContainer.CreateFile(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(stream, saveGameData);
            }
        }
    }
}
