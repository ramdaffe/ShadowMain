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
        public Vector2 Hotspot;
        public bool IsSelected;
        public bool IsClicked;
        public float Scale;
        public int Width
        {
            get { return Texture.Width; }
        }
        public int Height
        { 
            get { return Texture.Height; } 
        }

        public void Initialize(Texture2D texture, Vector2 position, string name)
        {
            Texture = texture;
            Position = position;
            Hotspot = new Vector2(Width * 0.3f, Height * 0.5f);
            Name = name;
            IsSelected = false;
            IsClicked = false;
            Scale = 1.0f;
        }

        public void SetStatus(bool status)
            //for now, selected status only
        {
            IsSelected = status;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}
