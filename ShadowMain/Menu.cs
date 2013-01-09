using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMain
{
    class Menu
    {
        public Texture2D Logo;
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

        public void Update()
        {
            switch(selButtonID){
                case 1:
                    NewButton.Position = NewButton.HoverPosition;
                    LoadButton.Position = InitLoadButtonPos;
                    HelpButton.Position = InitHelpButtonPos;
                    break;
                case 2:
                    LoadButton.Position = LoadButton.HoverPosition;
                    NewButton.Position = InitNewButtonPos;
                    HelpButton.Position = InitHelpButtonPos;
                    break;
                case 3:
                    HelpButton.Position = HelpButton.HoverPosition;
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
            NewButton.Draw(spriteBatch);
            LoadButton.Draw(spriteBatch);
            HelpButton.Draw(spriteBatch);
        }
    }
}
