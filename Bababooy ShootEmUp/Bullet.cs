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
    class Bullet
    {
        private Boolean myTrigger;
        private Texture2D myBullText;
        private Rectangle myBullRect;
        private SpriteBatch myBullBatch;
        private Vector2 myInitialPosition;

        public Bullet(GraphicsDeviceManager grap, ContentManager cont, int x, int y, int width, int height)
        {
            myTrigger = false;
            myBullRect = new Rectangle(x, y, width, height);
            myBullText = cont.Load<Texture2D>("bullet");
            myBullBatch = new SpriteBatch(grap.GraphicsDevice);
        }

        public void Shoot(int Xi, int Yi)
        {
            myInitialPosition = new Vector2(Xi, Yi);

            if (myTrigger == false)
            {
                myBullRect.X = (int)myInitialPosition.X;
                myBullRect.Y = (int)myInitialPosition.Y;

                myTrigger = true;
            }
        }

        public void Update()
        {
            if (myTrigger == true)
            {
                myBullRect.X += 12;
            }
            if (myBullRect.X > 700 || myBullRect.Y > 550)
            {
                myTrigger = false;
            }
        }

        public void Update(int x, int y)
        {
            if (myTrigger == true)
            {
                myBullRect.X += x;
                myBullRect.Y += y;
            }
            if (myBullRect.X > 700 || myBullRect.Y > 550)
            {
                myTrigger = false;
            }
        }

        public void Draw()
        {
            {
                myBullBatch.Begin();
                myBullBatch.Draw(myBullText, myBullRect, Color.White);
                myBullBatch.End();
            }
        }
		
        public int X
        {
            get
            {
                return myBullRect.X;
            }
            set
            {
                myBullRect.X = value;
            }
        }

        public int Y
        {
            get
            {
                return myBullRect.Y;
            }
            set
            {
                myBullRect.Y = value;
            }
        }

        public Boolean TriggerIsPulled
        {
            get
            {
                return myTrigger;
            }
            set
            {
                myTrigger = value;
            }
        }

        public Rectangle BoundingBox
        {
            get
            {
                return myBullRect;
            }
            set
            {
                myBullRect = value;
            }
        }
    }
}
