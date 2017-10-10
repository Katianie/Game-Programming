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
    class Bababooy
    {
        private Rectangle myRect;
        private Texture2D myTex;
        private SpriteBatch myBaBaBatch;
        private GraphicsDeviceManager myGrap;
        private ContentManager myCont;
        private Boolean myIsRemoved;
        private String myCurrentPowerup;
        private Bullet[] myBullets;
        private KeyboardState myKeyState;
        private KeyboardState myPrevKeyState;
        private int myScorePoints;

        public int rectX;
        public int rectY;
        public int rectWidth;
        public int rectHeight;

        int flackOuterAngle = 10;
        int flackInnerAngle = 5;

        public Bababooy(GraphicsDeviceManager grap, ContentManager cont)
        {
            myGrap = grap;
            myCont = cont;
            rectX = 0;
            rectY = 0;
            rectWidth = 40;
            rectHeight = 40;
            myRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
            myBaBaBatch = new SpriteBatch(grap.GraphicsDevice);
            myTex = myCont.Load<Texture2D>("babapng");
            myIsRemoved = false;
            myCurrentPowerup = "None";
            myBullets = new Bullet[4];
            myKeyState = Keyboard.GetState();
            myPrevKeyState = myKeyState;

            for (int i = 0; i < 4; i++)
            {
                myBullets[i] = new Bullet(myGrap, myCont,-100,-100,15,10);

            }
        }

        /**
         * The shoot method will create a rectangle and if that rectangle's x cordinates are equal to the
         * enimys x cordinates then the enimy will be removed and you will gain points
         */
        public void shoot()
        {
            myKeyState = Keyboard.GetState();

            if (myKeyState.IsKeyDown(Keys.Space) && CanShoot(myBullets))
            {
                myBullets[0].Shoot((myRect.X + 20), (myRect.Y + 25));

            }
        }

        //shuld only alow you to shoot when all triggers are false
        public void shootShotgun()
        {
            myKeyState = Keyboard.GetState();

            if (myKeyState.IsKeyDown(Keys.Space) && CanShoot(myBullets))
            {
                myBullets[0].Shoot((myRect.X + 20), (myRect.Y + 10));
                myBullets[1].Shoot((myRect.X + 20), (myRect.Y + 20));
                myBullets[2].Shoot((myRect.X + 20), (myRect.Y + 30));
                myBullets[3].Shoot((myRect.X + 20), (myRect.Y + 40));

            }
        }

        public void shootFlack()
        {
            myKeyState = Keyboard.GetState();

            if (myKeyState.IsKeyDown(Keys.Space) && CanShoot(myBullets))
            {
                myBullets[0].Shoot((myRect.X + 20), (myRect.Y + 10));
                myBullets[1].Shoot((myRect.X + 20), (myRect.Y + 20));
                myBullets[2].Shoot((myRect.X + 20), (myRect.Y + 30));
                myBullets[3].Shoot((myRect.X + 20), (myRect.Y + 40));

            }
        }

        public Boolean CanShoot(Bullet[] bullets)
        {
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i].TriggerIsPulled == true)
                {
                    return false;
                }
            }

            return true;
        }

        /**
         * This method is called in the Game1 class in its update method. when the user presses a key
         * the sprites y cordinates will change accordingly. the x cordinate will never change.
         */
        public void move()
        {
            if (myKeyState.IsKeyDown(Keys.Up))
            {
                if (myRect.Y > 0)
                {
                    myRect.Y -= 5;
                }

            }

            if (myKeyState.IsKeyDown(Keys.Down))
            {
                if (myRect.Y < 510)
                {
                    myRect.Y += 5;
                }
            }
        }

        public void Update()
        {
            if (myCurrentPowerup.Equals("None"))
            {
                shoot();

                for (int i = 0; i < myBullets.Length; i++)
                {
                    myBullets[i].Update();
                }
				
            }
            else if (myCurrentPowerup.Equals("Shotgun"))
            {
                shootShotgun();

                for (int i = 0; i < myBullets.Length; i++)
                {
                    myBullets[i].Update();
                }
				
            }
            else if (myCurrentPowerup.Equals("Flack"))
            {
                shootFlack();

                myBullets[0].Update((int)(12 * Math.Cos(MathHelper.ToRadians(flackOuterAngle))), (int)(12 * -Math.Sin(MathHelper.ToRadians(flackOuterAngle))));// /
                myBullets[1].Update((int)(12 * Math.Cos(MathHelper.ToRadians(flackInnerAngle))), (int)(12 * -Math.Sin(MathHelper.ToRadians(flackInnerAngle))));//  -
                myBullets[2].Update((int)(12 * Math.Cos(MathHelper.ToRadians(flackInnerAngle))), (int)(12 * Math.Sin(MathHelper.ToRadians(flackInnerAngle))));//   -
                myBullets[3].Update((int)(12 * Math.Cos(MathHelper.ToRadians(flackOuterAngle))), (int)(12 * Math.Sin(MathHelper.ToRadians(flackOuterAngle))));//  \

            }

            move();
        }

        public void Draw()
        {
            for (int i = 0; i < myBullets.Length; i++)
            {
                myBullets[i].Draw();
            }

            if (myIsRemoved == false)
            {
                myBaBaBatch.Begin();
                myBaBaBatch.Draw(myTex, myRect, Color.White);
                myBaBaBatch.End();
            }
            else
            {
                myIsRemoved = true;
            }
        }

        public int Points
        {
            get
            {
                return myScorePoints;
            }
            set
            {
                myScorePoints = value;
            }
        }

        public bool IsRemoved
        {
            get
            {
                return myIsRemoved;
            }
            set
            {
                myIsRemoved = value;
            }
        }

        public String CurrentPowerup
        {
            get
            {
                return myCurrentPowerup;
            }
            set
            {
                myCurrentPowerup = value;
            }
        }

        public Rectangle getRect()
        {
            return myRect;
        }

        public int getRectX()
        {
            return rectX;
        }

        public int getRectY()
        {
            return rectY;
        }

        public int getRectWidth()
        {
            return rectWidth;
        }

        public int getRectHeight()
        {
            return rectHeight;
        }

        public Bullet[] getBullets()
        {
            return myBullets;
        }

        public GraphicsDeviceManager getGrap()
        {
            return myGrap;
        }

        public ContentManager getCon()
        {
            return myCont;
        }
    }
}
