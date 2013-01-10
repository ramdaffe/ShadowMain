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
        public Vector2 HoverPosition;
        public Vector2 Hotspot;
        public bool IsSelected;
        public bool IsClicked;
        public float Scale;
        public int hovertime;
        public int Width
        {
            get { return Texture.Width; }
        }
        public int Height
        {
            get { return Texture.Height; }
        }
        public MenuButton(Texture2D texture, Vector2 position,string name)
        {
            Texture = texture;
            Position = position;
            Hotspot = Vector2.Add(new Vector2(Width * 0.3f, Height * 0.5f),position);
            Name = name;
            IsSelected = false;
            IsClicked = false;
            Scale = 1.0f;
            HoverPosition = position;
            hovertime = 0;
        }


        public void SetSelected()
            //for now, selected status only
        {
            IsSelected = true;
        }

        public void SetPos(Vector2 pos)
        {
            Position = pos;
        }



        public void Update(GameTime g)
        {
            if (IsSelected)
            {
                hovertime++;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }

    }
}
