using System;
using System.Collections;
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
using System.Threading;

namespace GUI
{
    public class FloatingText : MenuItem
    {
        protected double myAlphaValue;
        protected double myFadeIncrement;
        protected double myFadeDelay;
        protected bool myIsAlive;
        protected string myDisplayText;
        protected Color myDrawColor;//used to modify alpha value
        protected bool myIsMoving;

        public FloatingText(ContentManager contentManager, string displayText, Vector2 position, Color textColor) :
            base(contentManager, displayText, new Rectangle((int)position.X, (int)position.Y, 100, 100), Color.Black, textColor, textColor, "floating", false)
        {
            myDrawColor = textColor;
            myDisplayText = displayText;
            myAlphaValue = 255.0;
            myFadeIncrement = 5.0;
            myFadeDelay = 0.035;

            myIsAlive = false;
            myIsMoving = true;
        }

        public bool IsMoving
        {
            get
            {
                return myIsMoving;
            }
            set
            {
                myIsMoving = value;
            }
        }

        public double FadeIncrement
        {
            get
            {
                return myFadeIncrement;
            }
            set
            {
                myFadeIncrement = value;
            }
        }

        public double AlphaValue
        {
            get
            {
                return myAlphaValue;
            }
            set
            {
                myAlphaValue = value;
            }
        }

        public string DisplayText
        {
            get
            {
                return myDisplayText;
            }
            set
            {
                myDisplayText = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return myDrawColor;
            }
            set
            {
                myDrawColor = value;
            }
        }

        public bool IsAlive
        {
            get
            {
                return myIsAlive;
            }
            set
            {
                myIsAlive = value;
                myAlphaValue = 255;
            }
        }

        public override void update(bool checkInput, GameTime gameTime)
        {
            //Decrement the delay by the number of seconds that have elapsed since
            //the last time that the Update method was called
            myFadeDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            //If the Fade delays has dropped below zero, then it is time to 
            //fade in/fade out the image a little bit more.
            if (myFadeDelay <= 0)
            {
                //Reset the Fade delay
                myFadeDelay = 0.035;

                //If the AlphaValue is equal or above the max Alpha value or
                //has dropped below or equal to the min Alpha value, then 
                //reverse the fade
                if (myAlphaValue <= 0)
                {
                    myAlphaValue = 0;
                }
                else
                {
                    //Increment/Decrement the fade value for the image
                    myAlphaValue -= myFadeIncrement;
                }
            }


            if (myIsMoving)
            {
                myRect.Y -= 1;

                if (myRect.Y < 0)
                {
                    IsAlive = false;
                }
            }

            base.update(checkInput, gameTime);

        }

        public override void Draw(SpriteBatch aBatch)
        {
            if (myIsAlive)
            {
                aBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
                
                aBatch.DrawString(myFont, myDisplayText, new Vector2(myRect.X, myRect.Y), myDrawColor);

                myDrawColor.A = (byte)(MathHelper.Clamp((int)myAlphaValue, 0, 255));
                
                aBatch.End();
            }
            
        }
    }
}
