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
    class CollisionClass
    {
        private Bababooy myBababooy;
        private Bullet[] myBullets;
        private Enemy[] myEnemy;
        private Powerup[] myPowerups;
        private Random myRand;
        private int myMaxSpeed;

        public CollisionClass(Bababooy booy, Bullet[] bullets, Enemy[] enemys, Powerup[] powerups)
        {
            myBababooy = booy;
            myBullets = bullets;
            myPowerups = powerups;
            myEnemy = enemys;
            myRand = new Random();
            myMaxSpeed = 4;
        }

        public int MaxSpeed
        {
            get
            {
                return myMaxSpeed;
            }
            set
            {
                myMaxSpeed = value;
            }
        }

        public Boolean playerColideEnemy()
        {
            for (int i = 0; i < myEnemy.Length; i++)
            {
                if (myBababooy.getRect().Intersects(myEnemy[i].getRect()))
                {
                    myBababooy.IsRemoved = true;
                    return true;
                }
            }

            return false;
        }

        public void playerColidePowerUp()
        {
            for (int i = 0; i < myPowerups.Length; i++)
            {
                if (myPowerups[i].X < -10)
                {
                    myPowerups[i].X = myRand.Next(500, 16000);
                    myPowerups[i].Y = myRand.Next(100, 500);
                }

                if (myBababooy.getRect().Intersects(myPowerups[i].BoundingBox))
                {
                    if (myPowerups[i].Type == "Shotgun" && myBababooy.CurrentPowerup == "Shotgun")
                    {
                        myBababooy.Points += 300;
                    }
                    else if (myPowerups[i].Type == "Flack" && myBababooy.CurrentPowerup == "Flack")
                    {
                        myBababooy.Points += 300;
                    }
                    else if (myPowerups[i].Type == "Shotgun")
                    {
                        myBababooy.CurrentPowerup = "Shotgun";     
                    }
                    else if (myPowerups[i].Type == "Flack")
                    {
                        myBababooy.CurrentPowerup = "Flack";
                    }

                    myPowerups[i].X = myRand.Next(500, 16000);
                    myPowerups[i].Y = myRand.Next(100, 500);

                }
            }
        }

        public void Update(int numPoints)
        {
            myBababooy.Update();

            if (numPoints / 1000 > 4)
            {
                myMaxSpeed = numPoints / 1000;
            }
        }

        public void resetEnemyVelocity()
        {
            for (int i = 0; i < myEnemy.Length; i++)
            {
                myEnemy[i].setVelocity(myRand.Next(1, myMaxSpeed));
            }
        }
            
        public Boolean bulletColideEnemy()
        {
            for (int i = 0; i < myEnemy.Length; i++)
            {
                if (myEnemy[i].getRect().X < -10)
                {
                    myEnemy[i].setRectX(myRand.Next(300, 17000));
                }

                for (int j = 0; j < myBullets.Length; j++)
                {
                    if (myBullets[j].BoundingBox.Intersects(myEnemy[i].getRect()))
                    {
                        myEnemy[i].setRectX(myRand.Next(300, 17000));
                        myEnemy[i].setRectY(myRand.Next(100, 500));
                        myEnemy[i].setVelocity(myRand.Next(1, myMaxSpeed));

                        myBababooy.Points += myEnemy[i].PointValue;

                        myBullets[j].TriggerIsPulled = false;
                        myBullets[j].X = -10;
                        myBullets[j].Y = -10;

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
