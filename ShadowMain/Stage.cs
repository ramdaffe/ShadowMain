using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace ShadowMain
{
    class Stage
    {
        Texture2D StaticBG;
        Foreground ForeLayer1;
        Foreground ForeLayer2;
        Vector2 Position;
        
        public void Initialize(ContentManager content)
        {
            // Initialize Background
            StaticBG = content.Load<Texture2D>("Background\\bg1");

            // Initialize Foreground
            ForeLayer1 = new Foreground();
            ForeLayer2 = new Foreground();

            ForeLayer1.Initialize(content, "Foreground\\fg1", new Vector2(0, 0));
            ForeLayer2.Initialize(content, "Foreground\\fg2", new Vector2(0, 0));

            // Initialize Skeleton

            //Position = position;
        }
        public void Update()
        {   
            /*
            ForeLayer1.Update();
            ForeLayer2.Update();*/
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(StaticBG, Vector2.Zero, Color.White);
            ForeLayer1.Draw(spriteBatch);
            ForeLayer2.Draw(spriteBatch);
            
        }
    }
}
