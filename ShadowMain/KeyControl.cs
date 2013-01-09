using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace ShadowMain
{
    class KeyControl
    {
        private Keys key = Keys.None;
        private bool isEnabled = false;
        public KeyboardState currentKeyboardState;
        public KeyboardState previousKeyboardState;
        //private List<KeyControl> keyToggleList = new List<KeyControl>();

        public KeyControl()
        {
            //this.key = key;
        }
        /*
        public static bool IsEnabled()
        {
            if (previousKeyboardState.IsKeyDown(key) && currentKeyboardState.IsKeyDown(key))
                isEnabled = !isEnabled;

            return isEnabled;
        }
        public bool IsToggled(Keys key)
        {
            if (ReferenceEquals(keyToggleList.Find(x => x.key == key), null))
                keyToggleList.Add(new KeyControl(key));

            return keyToggleList.Find(x => x.key == key).IsEnabled();
        }*/

        public bool IsToggled(Keys key)
        {
            if (currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Update()
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }
        
    }
}
