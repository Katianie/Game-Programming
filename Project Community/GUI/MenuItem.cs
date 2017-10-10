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
//using Entities;

/** Katianie Game Library - MenuItem.cs
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
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Eddie O'Hagan</Owner>>
    public class MenuItem
    {
        protected Texture2D myTexture;
        protected MouseState myMouseState;
        protected Rectangle myRect;
        protected String myAssetName;
        protected bool myIsClickable;
        protected bool myIsCustomButton;
        protected SpriteFont myFont;

        protected Color myButtonColor;
        protected Color myMouseOverColor;
        protected Color myTextColor;
        private Color myPrevColor;//used when switching mouse over color

        private ContentManager myContentManager;
        private bool myIsQuestLogButton;
        

        public MenuItem(ContentManager cont, 
            String textureName, Rectangle rectangle, 
            Color buttonColor, Color textColor, Color mouseOverColor, string fontName)
        {
            myAssetName = textureName;
            myTexture = cont.Load<Texture2D>(textureName);
            myRect = rectangle;
            myMouseState = Mouse.GetState();
            myButtonColor = buttonColor;
            myTextColor = textColor;
            myMouseOverColor = mouseOverColor;
            myPrevColor = buttonColor;
            myIsClickable = true;
            myIsCustomButton = false;
            myIsQuestLogButton = false;
            myContentManager = cont;

            FontManager fm = FontManager.getFontManager(myContentManager);
            myFont = fm.getFont(fontName);
        }

        public MenuItem(ContentManager cont, 
            string buttonText, Rectangle buttonRect,
            Color buttonColor, Color textColor, Color mouseOverColor, string fontName, bool isQuestLogButton)
        {
            myAssetName = buttonText;
            myTexture = cont.Load<Texture2D>(@"GUITiles\Button");
            myRect = buttonRect;
            myMouseState = Mouse.GetState();
            myButtonColor = buttonColor;
            myTextColor = textColor;
            myMouseOverColor = mouseOverColor;
            myPrevColor = buttonColor;
            myIsClickable = true;
            myIsCustomButton = true;
            myContentManager = cont;
            myIsQuestLogButton = isQuestLogButton;

            FontManager fm = FontManager.getFontManager(myContentManager);
            myFont = fm.getFont(fontName);
        }

        public MenuItem(ContentManager cont,
            string buttonText, int x, int y, int width, int height,
            Color buttonColor, Color textColor, Color mouseOverColor, string fontName, bool isQuestLogButton)
        {
            myAssetName = buttonText;
            myTexture = cont.Load<Texture2D>(@"GUITiles\Button");
            myRect = new Rectangle(x, y, width, height);
            myMouseState = Mouse.GetState();
            myButtonColor = buttonColor;
            myTextColor = textColor;
            myMouseOverColor = mouseOverColor;
            myPrevColor = buttonColor;
            myIsClickable = true;
            myIsCustomButton = true;
            myContentManager = cont;
            myIsQuestLogButton = isQuestLogButton;

            FontManager fm = FontManager.getFontManager(myContentManager);
            myFont = fm.getFont(fontName);
        }

        public MenuItem(Texture2D texture2D, Rectangle itemRect, Color buttonColor, Color textColor, Color mouseOverColor, string fontName, bool isQuestLogButton)
        {
            myAssetName = texture2D.Name;
            myTexture = texture2D;
            myRect = itemRect;
            myMouseState = Mouse.GetState();
            myButtonColor = buttonColor;
            myTextColor = textColor;
            myMouseOverColor = mouseOverColor;
            myPrevColor = buttonColor;
            myIsClickable = true;
            myIsCustomButton = false;
            myContentManager = null;
            myIsQuestLogButton = isQuestLogButton;

            FontManager fm = FontManager.getFontManager(myContentManager);
            myFont = fm.getFont(fontName);
        }

        public bool IsClickable
        {
            get
            {
                return myIsClickable;
            }
            set
            {
                myIsClickable = value;
            }
        }

        public String Name
        {
            get
            {
                return myAssetName;
            }
            set
            {
                myAssetName = value;
            }
        }

        public Texture2D Texture
        {
            get
            {
                return myTexture;
            }
            set
            {
                myTexture = value;
            }
        }

        public SpriteFont Font
        {
            get
            {
                return myFont;
            }
            set
            {
                myFont = value;
            }
        }

        public Rectangle BoundingRectangle
        {
            get
            {
                return myRect;
            }
            set
            {
                myRect = value;
            }
        }

        public Color ButtonColor
        {
            get
            {
                return myButtonColor;
            }
            set
            {
                myButtonColor = value;
            }
        }

        public Color MouseOverColor
        {
            get
            {
                return myMouseOverColor;
            }
            set
            {
                myMouseOverColor = value;
            }
        }

        public Color TextColor
        {
            get
            {
                return myTextColor;
            }
            set
            {
                myTextColor = value;
            }
        }

        public Boolean isClicked(bool checkInput)
        {
            if (checkInput)
            {
                if (myIsClickable)
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

        public virtual void update(bool checkInput, GameTime gameTime)
        {
            if (checkInput)
            {
                if (myIsClickable)
                {
                    myMouseState = Mouse.GetState();

                    if (myRect.Intersects(new Rectangle(myMouseState.X, myMouseState.Y, 1, 1)))
                    {
                        //on mouse over
                        myButtonColor = myMouseOverColor;
                    }
                    else
                    {
                        myButtonColor = myPrevColor;
                    }
                }
            }
        }

        public virtual void Draw(SpriteBatch aBatch)
        {
            aBatch.Begin();

            aBatch.Draw(myTexture, myRect, myButtonColor);

            //draw the button text if it exists
            if (myIsCustomButton && !myIsQuestLogButton)
            {
                string buttonText = myAssetName.Substring(1);

                if (buttonText.Length > 10)
                {
                    char[] chars = buttonText.ToCharArray();
                    string temp = "";

                    for (int i = 0; i < chars.Length; i++)
                    {
                        temp += chars[i];

                        if ((i + 1) % 40 == 0)
                        {
                            temp += "\n";
                        }
                    }

                    buttonText = temp;
                }

                aBatch.DrawString(myFont, buttonText, new Vector2(myRect.X + 15, myRect.Y + 10), myTextColor);
            }
            else if(myIsQuestLogButton)
            {
                aBatch.DrawString(myFont, myAssetName, new Vector2(myRect.X + 15, myRect.Y + 10), myTextColor);
            }

            aBatch.End();
        }

        public bool Equals(MenuItem item)
        {
            if (this.Name == item.Name)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    
    }
}