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
    public class InformationText : FloatingText
    {
        private int myMaxSpacePerLine;
        private TimeSpan myTimeSpan;
        private TimeSpan myTimeElapsed;
        private DateTime myStart;
        private bool myStarted;

        public InformationText(ContentManager contentManager, string text, Vector2 position, Color color) :
            base(contentManager, text, position, color)
        {
            
            this.IsMoving = false;
            this.FadeIncrement = 5;

            myMaxSpacePerLine = 5;
            myTimeSpan = new TimeSpan(0, 0, 20);

            //break up text into different lines
            string temp = " ";

            if (text != null)
            {
                if (text.Length >= 10)
                {
                    char[] chars = text.ToCharArray();
                    int maxSpaceCount = 0;
                    int spaceCount = 0;

                    for (int i = 0; i < chars.Length; i++)
                    {
                        if (chars[i] == ' ')
                        {
                            if ((spaceCount + 1) % myMaxSpacePerLine == 0)
                            {
                                temp += "\n";
                            }

                            spaceCount++;
                            maxSpaceCount++;
                        }

                        temp += chars[i];

                    }

                }

                this.DisplayText = temp;
            }

        }

        public override void update(bool checkInput, GameTime gameTime)
        {
            if (!myStarted)
            {
                myStart = DateTime.Now;
            }

            myStarted = true;
            myTimeElapsed = (DateTime.Now - myStart);

            if (myTimeElapsed > myTimeSpan)
            {
                myAlphaValue -= myFadeIncrement;
            }

            if (myIsMoving)
            {
                myRect.Y -= 1;

                if (myRect.Y < 0)
                {
                    IsAlive = false;
                }
            }

        }
    }
}
