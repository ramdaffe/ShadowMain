using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ShadowMain
{
    class Foreground
    {
        Texture2D Texture;
        Vector2 Position;
        public void Initialize(ContentManager content, String texturePath, Vector2 position)
        {
            // Load foreground texture
            Texture = content.Load<Texture2D>(texturePath);
            Position = position;
        }
        public void Update()
        {
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Position, Color.White);
        }
    }
}
