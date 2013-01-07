using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace ShadowMain
{
    class Player
    {

        public Texture2D Cursor;

        public Vector2 Position;

        // State of the player
        public bool RecordState;

        // Get the width of the cursor
        public int Width
        {
            get { return Cursor.Width; }
        }

        // Get the height of the cursor
        public int Height
        {
            get { return Cursor.Height; }
        }


        public void Initialize(Texture2D texture, Vector2 position)
        {
            Cursor = texture;
            Position = position;
            RecordState = false;
        }


        public void Update()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Cursor, Position, null, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }

    }
}
