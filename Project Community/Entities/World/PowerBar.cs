using System;
using System.Collections.Generic;
using System.Linq;
using Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Entities.World
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PowerBar : Entity
    {
        private GameWorld myGameWorld;
        private SpriteBatch spriteBatch;
        private int myCurrentPower;
        private int myMaxPower;
        private int myMinPower;
        private bool myIsIncreasing;
        private bool myIsPaused;

        public PowerBar(GameWorld gw, EntityType type) : base(gw, type)
        {
            myGameWorld = gw;
            myMaxPower = 300;
            myMinPower = 2;
            myIsIncreasing = true;
            myIsPaused = false;

            
        }

        public bool IsPaused
        {
            get
            {
                return myIsPaused;
            }
            set
            {
                myIsPaused = value;
            }
        }

        public int MaxPower
        {
            get
            {
                return myMaxPower;
            }
            set
            {
                myMaxPower = value;
            }
        }

        public int CurrentPower
        {
            get
            {
                return myCurrentPower;
            }
        }

        public override void  animate()
        {
            KeyboardState mKeys = Keyboard.GetState();

            if (!myIsPaused)
            {
                if (myIsIncreasing)
                {
                    myCurrentPower += (myMaxPower / 75);
                }
                else
                {
                    myCurrentPower -= (myMaxPower / 75);
                }
            }

            if (myCurrentPower <= myMinPower)
            {
                myIsIncreasing = true;
            }
            else if (myCurrentPower >= myMaxPower)
            {
                myIsIncreasing = false;
            }

            //Force the power to remain between 0 and myMaxPower
            myCurrentPower = (int)MathHelper.Clamp(myCurrentPower, myMinPower, myMaxPower);

            base.Size = new Vector2(myCurrentPower, base.Height);

            base.animate();
        }
    }
}
