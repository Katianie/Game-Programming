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

/** Katianie Game Library - Menu.cs
* 
* This class represents a Menu containing an array of 
* MenuItem's along with other functions.
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
    public class Menu
    {
        protected Texture2D myTexture;
        protected ContentManager myContentManager;
        protected Rectangle myRect;
        protected ArrayList myItems;
        protected ArrayList myListOfEvents;
        protected string myName; //image asset name
        protected bool myIsPauseMenu; //True if the game should be paused when this menu is up
        protected Color myMenuColor;  //Changes the "tint" of the background
        protected bool myIsHidden;
        protected TextArea myTextArea;
        private int myPauseCounter;
        private bool myIsGameStarted;

        public Menu(ContentManager cont, String aTexture, Rectangle aRect, TextArea aTextArea)
        {
            myContentManager = cont;
            myName = aTexture;
            myTexture = cont.Load<Texture2D>(aTexture);
            myRect = aRect;
            myItems = new ArrayList();
            myListOfEvents = new ArrayList();
            myIsPauseMenu = false;
            myMenuColor = Color.White;
            myIsHidden = false;
            myTextArea = aTextArea;
            myPauseCounter = 9;//remember to change this in update if u change here
            myIsGameStarted = false;
        }

        public bool IsGameStarted
        {
            get
            {
                return myIsGameStarted;
            }
            set
            {
                myIsGameStarted = value;
            }
        }


        public Color MenuColor
        {
            get
            {
                return myMenuColor;
            }
            set
            {
                myMenuColor = value;
            }
        }

        public bool IsHidden
        {
            get
            {
                return myIsHidden;
            }
            set
            {
                myIsHidden = value;

            }
        }

        public bool IsPauseMenu
        {
            get
            {
                return myIsPauseMenu;
            }
            set
            {
                myIsPauseMenu = value;
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

        public string Name
        {
            get
            {
                return myName;
            }
            set
            {
                myName = value;
            }
        }

        public int NumItems
        {
            get
            {
                return myItems.Count;
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

        public MenuItem getItemByName(String name)
        {
            foreach(MenuItem item in myItems)
            {
                if (item.Name.Equals(name))
                {
                    return item;
                }
            }

            return null;
        }

        public void addItem(string imagePath, int x, int y, int width, int height, Color buttonColor, Color mouseOverColor)
        {
            MenuItem item = new MenuItem(myContentManager,
                imagePath, new Rectangle(x, y, width, height),
                buttonColor, Color.White, mouseOverColor, "quest");

            myItems.Add(item);
        }

        public void addItem(string imagePath, Rectangle aRectangle, Color buttonColor, Color mouseOverColor)
        {
            MenuItem item = new MenuItem(myContentManager, 
                imagePath, aRectangle,
                buttonColor, Color.White, mouseOverColor, "Incantation");

            myItems.Add(item);
        }

        public void addCustomItem(string itemText, int x, int y, int width, int height,
                                  Color buttonColor, Color textColor, Color mouseOverColor)
        {
            MenuItem item = new MenuItem(myContentManager,
                itemText, new Rectangle(x, y, width, height),
                buttonColor, textColor, mouseOverColor, "Incantation", false);


            myItems.Add(item);

            myListOfEvents.Add(itemText);
        }

        public void addCustomItem(string itemText, Rectangle aRectangle,
                                  Color buttonColor, Color textColor, Color mouseOverColor)
        {
            MenuItem item = new MenuItem(myContentManager,
                itemText, aRectangle,
                buttonColor, textColor, mouseOverColor, "quest", false);

            myItems.Add(item);

            myListOfEvents.Add(itemText);
        }


        //returns the index of what should be the current menu
        public string processClickedButton(Menu currentMenu, MenuItem item)
        {
            if (item.Name == "pPlay")
            {
                return "playerGUI";
            }
            else if (item.Name == "cControls")
            {
                return "ControlsMenuScreen";
            }
            else if (item.Name == "cCredits")
            {
                return "AboutMenuScreen";
            }
            else if(item.Name == "aAbout")
            {
                return "StoryMenuScreen";
            }
            else if (item.Name == "lLoad")
            {
                return "loadGameEvent";
            }
            else if (item.Name == "sSave")
            {
                return "saveGameEvent";
            }
            else if(item.Name == @"GUITiles\inventoryButton")
            {
                return "showInventoryEvent";
            }
            else if (item.Name == @"GUITiles\questLogButton")
            {
                return "showQuestLogEvent";
            }
            else if (item.Name == @"GUITiles\settingsButton")
            {
                return "pauseScreen";
            }
            else if (item.Name == @"GUITiles\quitButton")
            {
                return "splash";
            }
            else if (item.Name == "qQuit" && currentMenu.Name == "splash")
            {
                return "quitGameEvent";
            }
            else if (item.Name == "bBack" && currentMenu.Name == "pauseScreen")
            {
                return "playerGUI";
            }
            else if (item.Name == "bBack" && myIsGameStarted)
            {
                return "pauseScreen";
            }
            else if (item.Name == "bBack" && currentMenu.Name.Contains("MenuScreen"))
            {
                //menus that are from the main menu all have MenuScreen in their name
                return "splash";
            }
            else if (item.Name == "qQuit")
            {
                myIsGameStarted = false;
                return "splash";
            }

            foreach (string aEvent in myListOfEvents)
            {
                if (item.Name == aEvent)
                {
                    return aEvent + "Responce";
                }
            }

            return "";
        }

        //Returns what should be the current menu
        public string update(bool gameStarted, bool checkInput, GameTime gameTime)
        {
            myIsGameStarted = gameStarted;

            if (!myIsHidden && checkInput)
            {
                if (myItems != null)
                {
                    if (myTextArea != null)
                    {
                        myTextArea.update(checkInput, gameTime);

                        //only allow the player to respond after they have read everything
                        if (myTextArea.IsOnLastPage)
                        {
                            foreach (MenuItem item in myItems)
                            {
                                item.update(checkInput, gameTime);

                                if (item.isClicked(checkInput))
                                {
                                    myPauseCounter--;

                                    if (myPauseCounter <= 0)
                                    {
                                        myPauseCounter = 9;
                                        return processClickedButton(this, item);
                                    }
                                }
                            }
                        }
                    }
                    else 
                    {
                        foreach (MenuItem item in myItems)
                        {
                            item.update(checkInput, gameTime);

                            if (item.isClicked(checkInput))
                            {
                                myPauseCounter--;

                                if (myPauseCounter <= 0)
                                {
                                    myPauseCounter = 9;
                                    return processClickedButton(this, item);
                                }
                            }
                        }
                    }


                }
            }

            return "";
        }

        public void Draw(SpriteBatch aBatch)
        {
            if (!myIsHidden)
            {
                aBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);
                
                if (myName == "playerGUI")
                {
                    myMenuColor.A = (byte)(255 / 1.2);
                }

                aBatch.Draw(myTexture, myRect, myMenuColor);
                aBatch.End();

                if (myTextArea != null)
                {
                    myTextArea.Draw(aBatch);
                }

                if (myItems != null)
                {
                    foreach (MenuItem item in myItems)
                    {
                        //if the player is not on the last page of text then
                        //we want to gray out the buttons to indicate they
                        //cannot continue.
                        if (myTextArea != null && !myTextArea.IsOnLastPage)
                        {
                            item.ButtonColor = Color.Gray;
                        }

                        item.Draw(aBatch);
                    }
                }
            }
            
        }

        public bool Equals(Menu aMenu)
        {
            return this.Name == aMenu.Name;
        }

    }
}
