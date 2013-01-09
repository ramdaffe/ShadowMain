using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMain
{
    class Menu
    {
        public Texture2D Logo;
        MenuButton NewButton;
        MenuButton LoadButton;
        MenuButton HelpButton;
        public int selButtonID;
        

        //positions
        public Vector2 NewButtonPos;
        public Vector2 LoadButtonPos;
        public Vector2 HelpButtonPos;

        //hostpot
        public Vector2 NewButtonHotPos;
        public Vector2 LoadButtonHotPos;
        public Vector2 HelpButtonHotPos;

        bool MenuState = true;

        public void Initialize(ContentManager content)
        {
            NewButtonPos = new Vector2(750, 150);
            LoadButtonPos = new Vector2(750, 300);
            HelpButtonPos = new Vector2(750, 450);
            NewButton = new MenuButton();
            LoadButton = new MenuButton();
            HelpButton = new MenuButton();
            NewButton.Initialize(content.Load<Texture2D>("Button\\menu_new"), NewButtonPos, "new");
            LoadButton.Initialize(content.Load<Texture2D>("Button\\menu_load"), LoadButtonPos, "load");
            HelpButton.Initialize(content.Load<Texture2D>("Button\\menu_help"), HelpButtonPos, "help");
            NewButtonHotPos = NewButton.Hotspot + NewButtonPos;
            LoadButtonHotPos = LoadButton.Hotspot + LoadButtonPos;
            HelpButtonHotPos = HelpButton.Hotspot + HelpButtonPos;
            selButtonID = 0;
        }

        public void SetSelected(int selectID)
        {
            switch(selectID)
            {
                case 1:
                    NewButton.SetStatus(true);
                    LoadButton.SetStatus(false);
                    HelpButton.SetStatus(false);
                    selButtonID = selectID;
                    break;
                case 2:
                    NewButton.SetStatus(false);
                    LoadButton.SetStatus(true);
                    HelpButton.SetStatus(false);
                    selButtonID = selectID;
                    break;
                case 3:
                    NewButton.SetStatus(false);
                    LoadButton.SetStatus(false);
                    HelpButton.SetStatus(true);
                    selButtonID = selectID;
                    break;
                default:
                    break;
            }
        }

        public int GetSelected()
        {
            return selButtonID;
        }

        public void SetNew(Vector2 newpos)
        {
            NewButtonPos = newpos;
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            NewButton.Draw(spriteBatch);
            LoadButton.Draw(spriteBatch);
            HelpButton.Draw(spriteBatch);
        }
    }
}
