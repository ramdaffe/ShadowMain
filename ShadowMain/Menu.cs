using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMain
{
    class Menu
    {
        public Texture2D Logo;
        public Texture2D Spotlight;
        public MenuButton NewButton;
        public MenuButton LoadButton;
        public MenuButton HelpButton;
        public int selButtonID = 0;
        public bool ready = false;
        //Initial value
        public Vector2 InitNewButtonPos = new Vector2(750, 150);
        public Vector2 InitLoadButtonPos = new Vector2(750, 300);
        public Vector2 InitHelpButtonPos = new Vector2(750, 450);

        public void Initialize(ContentManager content)
        {
            NewButton = new MenuButton(content.Load<Texture2D>("Button\\menu_new"), InitNewButtonPos,"new");
            LoadButton = new MenuButton(content.Load<Texture2D>("Button\\menu_load"), InitLoadButtonPos, "load");
            HelpButton = new MenuButton(content.Load<Texture2D>("Button\\menu_help"), InitHelpButtonPos, "help");
            Logo = content.Load<Texture2D>("MenuRes\\logo");
            Spotlight = content.Load<Texture2D>("MenuRes\\spotlight");
            NewButton.HoverPosition = Vector2.Add(NewButton.Position, new Vector2(-30, 0));
            LoadButton.HoverPosition = Vector2.Add(LoadButton.Position, new Vector2(-30, 0));
            HelpButton.HoverPosition = Vector2.Add(HelpButton.Position, new Vector2(-30, 0));
        }

        public Vector2 Hover(Vector2 pos)
        {
            return pos = Vector2.Add(pos, new Vector2(-30, 0));
        }

        public void ResetAllPos()
        {
            NewButton.Position = InitNewButtonPos;
            LoadButton.Position = InitLoadButtonPos;
            HelpButton.Position = InitHelpButtonPos;
        }

        public Vector2 SmoothMove(Vector2 initPos, Vector2 endPos, int animDuration, GameTime gameTime, float elapsedTime )
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            elapsedTime += dt;
            if (elapsedTime > 1)
                elapsedTime = 1;

            float param = elapsedTime / animDuration;
            return Vector2.Lerp(initPos, endPos, (float)Math.Pow(param / 2.0, 0.5));
        }

        public void Update(GameTime gameTime, float elapsedTime)
        {
            switch(selButtonID){
                case 1:
                    NewButton.SetSelected();
                    NewButton.Position = SmoothMove(NewButton.Position,NewButton.HoverPosition,2,gameTime,elapsedTime);
                    LoadButton.Position = InitLoadButtonPos;
                    HelpButton.Position = InitHelpButtonPos;
                    break;
                case 2:
                    LoadButton.SetSelected();
                    LoadButton.Position = SmoothMove(LoadButton.Position, LoadButton.HoverPosition, 2, gameTime, elapsedTime);
                    NewButton.Position = InitNewButtonPos;
                    HelpButton.Position = InitHelpButtonPos;
                    break;
                case 3:
                    HelpButton.SetSelected();
                    HelpButton.Position = SmoothMove(HelpButton.Position, HelpButton.HoverPosition, 2, gameTime, elapsedTime);
                    NewButton.Position = InitNewButtonPos;
                    LoadButton.Position = InitLoadButtonPos;
                    break;
                default:
                    ResetAllPos();
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Spotlight, new Vector2(200, 200), Color.White * 0.5f);
            spriteBatch.Draw(Logo, new Vector2(200, 200), Color.White);
            NewButton.Draw(spriteBatch);
            LoadButton.Draw(spriteBatch);
            HelpButton.Draw(spriteBatch);
        }
    }
}
