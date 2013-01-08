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
        

        //positions
        public Vector2 NewButtonPos;
        public Vector2 LoadButtonPos;
        public Vector2 HelpButtonPos;

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
