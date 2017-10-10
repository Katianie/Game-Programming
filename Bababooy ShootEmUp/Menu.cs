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
    class Menu
    {
        private Texture2D myTexture;
        private SpriteBatch myBatch;
        private Rectangle myRect;
        private MenuItem[] myItems;

        public Menu(GraphicsDeviceManager graphics, ContentManager cont, String aTexture, Rectangle aRect, MenuItem[] menuItemsArray)
        {
            myTexture = cont.Load<Texture2D>(aTexture);
            myBatch = new SpriteBatch(graphics.GraphicsDevice);
            myRect = aRect;
            myItems = menuItemsArray;
        }

        public Texture2D getTexture()
        {
            return myTexture;
        }

        public Rectangle getPos()
        {
            return myRect;
        }

        public MenuItem[] getItemsArray()
        {
            return myItems;
        }

        public void update()
        {
            for (int i = 0; i < myItems.Length; i++)
            {
                myItems[i].update();
            }
        }

        public void draw()
        {
            myBatch.Begin();
            myBatch.Draw(myTexture, myRect, Color.White);
            myBatch.End();

            for (int i = 0; i < myItems.Length; i++)
            {
                myItems[i].draw();
            }
        }
    }
}
