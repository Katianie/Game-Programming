/** XNA_Bababooy_ShootEmUp
* A 2D shooting game where the object is to kill aproaching
* enimies; similar to space invaders.
*
* This was uploaded to Katianie.com, Feel free to use this
* code and share it with others, Just give me credit ^_^.
*
* Eddie O'Hagan
* Copyright © 2009 Katianie.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNA_Bababooy_ShootEmUp
{
    class MenuItem
    {
        private Texture2D myTexture;
        private MouseState myMouseState;
        private Rectangle myRect;
        private SpriteBatch mySpriteBatch;
        private Color myButtonColor;

        public MenuItem(GraphicsDeviceManager graphics, ContentManager cont, String texture, Rectangle rectangle)
        {
            myTexture = cont.Load<Texture2D>(texture);
            myRect = rectangle;
            mySpriteBatch = new SpriteBatch(graphics.GraphicsDevice);
            myMouseState = Mouse.GetState();
            //represents r,g,b
            myButtonColor = new Color(new Vector3(1, 1, 1));
        }

        public Boolean isClicked()
        {
            myMouseState = Mouse.GetState();

            if (myRect.Intersects(new Rectangle(myMouseState.X, myMouseState.Y, 1, 1)))
            {

                if (myMouseState.LeftButton.ToString().Equals("Pressed"))
                {
                    return true;
                }

                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void update()
        {
            myMouseState = Mouse.GetState();

            if (myRect.Intersects(new Rectangle(myMouseState.X, myMouseState.Y, 1, 1)))
            {
                myButtonColor = new Color(new Vector3(0, 0, 1));
            }
            else
            {
                myButtonColor = new Color(new Vector3(1, 1, 1));
            }
        }

        public void draw()
        {
            mySpriteBatch.Begin();
            mySpriteBatch.Draw(myTexture, myRect, myButtonColor);
            mySpriteBatch.End();
        }
    }
}
