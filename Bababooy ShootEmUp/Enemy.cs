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
    class Enemy
    {
        private int myShotsToKill;
        private int myVelocity;
        private int myShotsOnEnemy;
        private int myPointValue;
        private Color myColor;
        private SpriteBatch myEBatch;
        private Texture2D myETexture;
        private Rectangle myERect;
        private Boolean myIsRemoved;

        public Enemy(GraphicsDeviceManager grap, ContentManager cont, int rX, int rY, int vel)
        {
            myShotsToKill = 1;
            myVelocity = vel;
            myShotsOnEnemy = 0;
            myColor = Color.White;
            myEBatch = new SpriteBatch(grap.GraphicsDevice);
            myETexture = cont.Load<Texture2D>("terror1");
            myERect = new Rectangle(rX, rY, 40, 40);
            myIsRemoved = false;
            myPointValue = 100;

        }

        public Enemy(GraphicsDeviceManager grap, ContentManager cont, int rX, int rY, int vel, String fileNameTexture, int pointValue)
        {
            myShotsToKill = 1;
            myVelocity = vel;
            myShotsOnEnemy = 0;
            myColor = Color.White;
            myEBatch = new SpriteBatch(grap.GraphicsDevice);
            myETexture = cont.Load<Texture2D>(fileNameTexture);
            myERect = new Rectangle(rX, rY, 40, 40);
            myIsRemoved = false;
            myPointValue = pointValue;
        }

        public Enemy(GraphicsDeviceManager grap, ContentManager cont, int rX, int rY, int rWidth, int rHeight, int vel, int shotsKill, Color col, String fileNameTexture)
        {
            myShotsToKill = shotsKill;
            myVelocity = vel;
            myShotsOnEnemy = 0;
            myColor = col;
            myEBatch = new SpriteBatch(grap.GraphicsDevice);
            myETexture = cont.Load<Texture2D>(fileNameTexture);
            myERect = new Rectangle(rX, rY, rWidth, rHeight);
            myIsRemoved = false;
            myPointValue = 100;
        }

        public Enemy(GraphicsDeviceManager grap, ContentManager cont, int rX, int rY, int rWidth, int rHeight, int vel, int shotsKill, Color col, String fileNameTexture, int pointValue)
        {
            myShotsToKill = shotsKill;
            myVelocity = vel;
            myShotsOnEnemy = 0;
            myColor = col;
            myEBatch = new SpriteBatch(grap.GraphicsDevice);
            myETexture = cont.Load<Texture2D>(fileNameTexture);
            myERect = new Rectangle(rX, rY, rWidth, rHeight);
            myIsRemoved = false;
            myPointValue = pointValue;
        }

        public void move()
        {
            myERect.X -= myVelocity;
        }

        public void draw()
        {
            if (myIsRemoved == false)
            {
                myEBatch.Begin();
                myEBatch.Draw(myETexture, myERect, myColor);
                myEBatch.End();
                move();
            }
            else 
            {
                Terminate();
            }


        }

        public int PointValue
        {
            get
            {
                return myPointValue;
            }
            set
            {
                myPointValue = value;
            }
        }

        public void Terminate()
        {
            myIsRemoved = true;
        }

        public Rectangle getRect()
        {
            return myERect;
        }

        public int getVelocity()
        {
            return myVelocity;
        }

        public void setRectX(int x)
        {
            myERect.X = x;
        }
        public void setRectY(int y)
        {
            myERect.Y = y;
        }
        public void setVelocity(int vel)
        {
            myVelocity = vel;
        }
    }
}
