using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMain
{
    class Menu
    {
        public Texture2D Background;
        public Texture2D Logo;
        public Texture2D NewButton;
        public Texture2D LoadButton;
        public Texture2D HelpButton;
        bool MenuState = true;

        public void Initialize(Texture2D bg,Texture2D logo, Texture2D newButton,Texture2D loadButton, Texture2D helpButton, Vector2 position)
        {
            Background = bg;
            Logo = logo;
            NewButton = newButton;
            LoadButton = loadButton;
            HelpButton = helpButton;
        }


        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);
            spriteBatch.Draw(Background, Vector2.Zero, Color.White);
        }
    }
}
