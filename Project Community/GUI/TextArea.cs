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

/** Katianie Game Library - TextArea.cs
* 
* This class represents a single item in a Menu. This
* Class is used by the Menu Class. 
* 
* This was uploaded to Katianie.com, Feel free to use this
* code and share it with others. Special thanks to all the
* guys and girls on the XNA forums along with other tutorials
* found via http://forums.xna.com/forums/Default.aspx
*
* This is an open source library so if you make any changes
* please SEND IT TO ME! That way I can upload the new version
* for everyone to use!
* 
* Eddie O'Hagan
* eddieohagan@optonline.net 
* Copyright © 2012 Katianie.com
*/

namespace GUI
{
    public class TextArea : MenuItem
    {
        private int myCurrentIndex;
        private string myCurrentString; //current text displayed
        private ArrayList myStrings; //entire conversation broken up into displayable sections
        private string[] myStringsArray;
        private bool myIsOnLastPage; //if true then the player can respond

        private int myMaxSpaceCount; //number of spaces before spliting
        private int myMaxSpacePerLine; //number of spaces per line
        private int myClickDelay;

        private MenuItem myNextButton;
        private MenuItem myPrevButton;

        bool nextonce = true;
        bool prevonce = true;

        public TextArea(ContentManager cont, Rectangle rectangle,
                        Color buttonColor, Color textColor, string fontName)
            : base(cont, @"GUITiles\layout", rectangle, buttonColor, textColor, buttonColor, fontName)
        {
            Rectangle nextButtonRect = new Rectangle(rectangle.Right, rectangle.Bottom - 50, 50, 50);
            Rectangle prevButtonRect = new Rectangle(rectangle.Left - 50, rectangle.Bottom - 50, 50, 50);

            //This is a text area and not a button
            myCurrentString = "";
            base.IsClickable = false;

            myMaxSpaceCount = 25;
            myMaxSpacePerLine = 4;
            myClickDelay = 9;
            myStrings = new ArrayList();
            myCurrentIndex = 0;

            myNextButton = new MenuItem(cont, @"GUITiles\nextbutton", nextButtonRect, Color.White, Color.White, Color.Red, fontName);
            myPrevButton = new MenuItem(cont, @"GUITiles\prevbutton", prevButtonRect, Color.White, Color.White, Color.Red, fontName);

            myIsOnLastPage = false;
        }

        public TextArea(ContentManager cont, Rectangle rectangle,
                        int maxSpacePerPage, int maxSpacePerLine,
                        Color buttonColor, Color textColor, Color mouseOverColor, string fontName)
            : base(cont, "questTextArea", rectangle, buttonColor, textColor, mouseOverColor, fontName)
        {
            Rectangle nextButtonRect = new Rectangle(rectangle.Right, rectangle.Bottom - 50, 50, 50);
            Rectangle prevButtonRect = new Rectangle(rectangle.Left - 50, rectangle.Bottom - 50, 50, 50);

            //This is a text area and not a button
            myCurrentString = "";
            base.IsClickable = false;

            myMaxSpaceCount = maxSpacePerPage;
            myMaxSpacePerLine = maxSpacePerLine;
            myStrings = new ArrayList();
            myCurrentIndex = 0;

            myNextButton = new MenuItem(cont, @"GUITiles\nextbutton", nextButtonRect, Color.White, Color.White, Color.Red, fontName);
            myPrevButton = new MenuItem(cont, @"GUITiles\prevbutton", prevButtonRect, Color.White, Color.White, Color.Red, fontName);

            myIsOnLastPage = false;
        }

        public bool IsOnLastPage
        {
            get
            {
                return myIsOnLastPage;
            }
            set
            {
                myIsOnLastPage = value;
            }
        }

        public void addConversation(string conversation)
        {
            if (conversation != null)
            {
                if (conversation.Length >= 10)
                {
                    char[] chars = conversation.ToCharArray();
                    String temp = " ";

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

                        if (maxSpaceCount >= myMaxSpaceCount)
                        {
                            myStrings.Add(temp);

                            temp = " ";
                            maxSpaceCount = 0;
                            spaceCount = 0;
                        }
                        
                    }

                    if (temp.Length > 0)
                    {
                        myStrings.Add(temp);
                    }
                }
                else
                {
                    myStrings.Add(conversation);
                }

                myStringsArray = (string[])myStrings.ToArray(typeof(string));
            }
        }


        public override void update(bool checkInput, GameTime gameTime)
        {
            if (checkInput)
            {
                base.update(checkInput, gameTime);

                myNextButton.update(checkInput, gameTime);
                myPrevButton.update(checkInput, gameTime);

                if (myNextButton.isClicked(checkInput) && nextonce)
                {
                    if (myCurrentIndex >= 0 && myCurrentIndex < myStringsArray.Length - 1)
                    {
                        myClickDelay--;

                        if (myClickDelay <= 0)
                        {
                            myClickDelay = 8;
                            myCurrentIndex++;
                            nextonce = false;
                            prevonce = false;
                        }    
                    }
                }
                else if (myPrevButton.isClicked(checkInput) && prevonce)
                {
                    if (myCurrentIndex > 0 && myCurrentIndex < myStringsArray.Length)
                    {
                        myClickDelay--;

                        if (myClickDelay <= 0)
                        {
                            myClickDelay = 8;
                            myCurrentIndex--;
                            nextonce = false;
                            prevonce = false;

                        }
                    }
                }
                else
                {
                    if (myStringsArray != null)
                    {
                        myCurrentString = myStringsArray[myCurrentIndex];
                        nextonce = true;
                        prevonce = true;
                    }
                }

                if (myCurrentIndex == myStringsArray.Length - 1)
                {
                    myIsOnLastPage = true;
                }
                else
                {
                    myIsOnLastPage = false;
                }
            }
        }

        public override void Draw(SpriteBatch aBatch)
        {
            base.Draw(aBatch);

            if (!myIsOnLastPage)
            {
                myNextButton.ButtonColor = Color.Green;
            }
            else
            {
                myNextButton.ButtonColor = Color.White;
            }

            myNextButton.Draw(aBatch);
            myPrevButton.Draw(aBatch);

            aBatch.Begin();
            aBatch.DrawString(base.Font, myCurrentString, new Vector2(base.BoundingRectangle.X + 35, base.BoundingRectangle.Y + 10), Color.Black);
            aBatch.End();

        }
    }

}
