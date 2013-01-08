using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ShadowMain
{
    class MenuButton
    {
        Texture2D Texture;
        String Name;
        public Vector2 Position;
        public bool IsSelected;
        public bool IsClicked;
        public float Scale;
        int Width
        {
            get { return Texture.Width; }
        }
        int Height
        { 
            get { return Texture.Height; } 
        }

        public void Initialize(Texture2D texture, Vector2 position, string name)
        {
            Texture = texture;
            Position = position;
            Name = name;
            IsSelected = true;
            IsClicked = false;
            Scale = 1.0f;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}
