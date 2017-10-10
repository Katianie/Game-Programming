using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GUI
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Eddie O'Hagan</Owner>>
    public class GUIManager
    {
        //This is a pointer to a function. It is used in the AI classes to know 
        //how to respond to a particular statement.
        public delegate void ResponseMethod(int i);
        private ResponseMethod justinsMethod;

        //used for singleton
        private static GUIManager myGUIManager;

        private ContentManager myContentManager;

        private ArrayList myMenus;
        private Menu myCurrentMenu;
        private Menu myPlayerGUI;
        private Menu myConvoMenu;

        private ArrayList myFloatingTextList;
        private ArrayList myInformationTextList;

        private Game myGame;

        private InventoryMenu myInventoryMenu;
        private QuestLogMenu myQuestLogMenu;
        private ShopMenu myShopMenu;

        private Rectangle myQuestLogRectangle;
        private Rectangle myQuestLogTextAreaRectangle;

        private FontManager myFontManager;

        private string myCurrentIndex;//index in the array where the current menu is
        private string myUpdateReturnVal;//didn't want to declare this inside the update method
        private string myPlayerReturnVal;
        private string myConvoReturnVal;
        private string myShopReturnVal;

        private string myGameWorldName;

        private bool myGameStarted;
        private bool myInventoryShowing;

        private int myMoney;
        private int myNewMoney;//new amount of money after making a purchase
        private int myRep;

        private Dictionary<String, int> myPlayerInventory;

        public static GUIManager getGUIManager(Game game, ContentManager cont)
        {
            if (myGUIManager == null)
            {
                myGUIManager = new GUIManager(game, cont);
            }

            return myGUIManager;
        }

        private GUIManager(Game game, ContentManager cont)
        {
            myMenus = new ArrayList();
            myGame = game;
            myContentManager = cont;
            myGameStarted = false;
            myInventoryShowing = false;

            myFontManager = FontManager.getFontManager(cont);

            //Construct the initial main menu
            myCurrentMenu = new Menu(cont,
                                     "splash",
                                     new Rectangle(0, 0, 800, 600),
                                     null);

            myCurrentMenu.addCustomItem("pPlay", 165, 150, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("cControls", 165, 250, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("lLoad", 165, 350, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("cCredits", 450, 150, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("aAbout", 450, 250, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("qQuit", 450, 350, 150, 50, Color.White, Color.Black, Color.Brown);


            myMenus.Add(myCurrentMenu);

            //Construct the player GUI
            //this will not be in the menu list but in its own object
            //this is because the player GUI needs to be displayed simultaneously
            //with other in game menus
            myPlayerGUI = new Menu(cont,
                                   "playerGUI",
                                   new Rectangle(0, 0, 800, 600),
                                   null);

            myPlayerGUI.addItem(@"GUITiles\inventoryButton", 30, 500, 80, 75, Color.White, Color.Green);
            myPlayerGUI.addItem(@"GUITiles\questLogButton", 110, 500, 80, 75, Color.White, Color.Orange);
            myPlayerGUI.addItem(@"GUITiles\settingsButton", 190, 500, 80, 75, Color.White, Color.Red);
            myPlayerGUI.addItem(@"GUITiles\quitButton", 270, 500, 80, 75, Color.White, Color.Purple);

            //Construct controls screen
            myCurrentMenu = new Menu(cont,
                                     "ControlsMenuScreen",
                                     new Rectangle(0, 0, 800, 600),
                                     null);

            myCurrentMenu.addCustomItem("bBack", 700, 500, 150, 50, Color.White, Color.Black, Color.Brown);

            myMenus.Add(myCurrentMenu);

            //Construct about screen
            myCurrentMenu = new Menu(cont,
                                     "AboutMenuScreen",
                                     new Rectangle(0, 0, 800, 600),
                                     null);

            myCurrentMenu.addCustomItem("bBack", 700, 500, 150, 50, Color.White, Color.Black, Color.Brown);

            myMenus.Add(myCurrentMenu);

            //Construct controls screen
            myCurrentMenu = new Menu(cont,
                                     "StoryMenuScreen",
                                     new Rectangle(0, 0, 800, 600),
                                     null);

            myCurrentMenu.addCustomItem("bBack", 700, 500, 150, 50, Color.White, Color.Black, Color.Brown);

            myMenus.Add(myCurrentMenu);

            //Create in game pause menu.
            myCurrentMenu = new Menu(cont,
                                     "pauseScreen",
                                     new Rectangle(0, 0, 800, 600),
                                     null);

            myCurrentMenu.addCustomItem("aAbout", 350, 60, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("bBack", 350, 110, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("cControls", 350, 160, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("cCredits", 350, 210, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("lLoad", 350, 260, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("sSave", 350, 310, 150, 50, Color.White, Color.Black, Color.Brown);
            myCurrentMenu.addCustomItem("qQuit", 350, 460, 150, 50, Color.White, Color.Black, Color.Brown);


            myCurrentMenu.IsPauseMenu = true;

            myMenus.Add(myCurrentMenu);

            //set the current menu to the main menu which is index 0
            myCurrentIndex = "splash";

            myFloatingTextList = new ArrayList();
            myInformationTextList = new ArrayList();

            foreach (Menu aMenu in myMenus)
            {
                if (aMenu.Name == myCurrentIndex)
                {
                    myCurrentMenu = aMenu;
                }
            }


            myQuestLogRectangle = new Rectangle(200, 100, 500, 400);
            myQuestLogTextAreaRectangle = new Rectangle(myQuestLogRectangle.X + (myQuestLogRectangle.Width / 2),
                                                        myQuestLogRectangle.Y,
                                                        250,
                                                        400);
            //myQuestLogTextArea = new TextArea(cont, myQuestLogTextAreaRectangle, Color.White, Color.Black, "quest");
            myQuestLogMenu = new QuestLogMenu(cont, @"GUITiles/questLogLayout", myQuestLogRectangle, null);
            myInventoryMenu = new InventoryMenu(cont, @"GUITiles/layout", new Rectangle(200, 100, 500, 350), "Player");
            myShopMenu = new ShopMenu(game, cont, @"GUITiles/layout", new Rectangle(120, 100, 500, 350), "Shop");
        }

        public Menu PlayersGUI
        {
            get
            {
                return myPlayerGUI;
            }
            set
            {
                myPlayerGUI = value;
            }
        }

        public string GameWorldName
        {
            get
            {
                return myGameWorldName;
            }
            set
            {
                myGameWorldName = value;
            }
        }

        public int Money
        {
            get
            {
                return myMoney;
            }
            set
            {
                myMoney = value;
            }
        }

        public int Rep
        {
            get
            {
                return myRep;
            }
            set
            {
                myRep = value;
            }
        }

        public bool IsInventoryShowing
        {
            get
            {
                return myInventoryShowing;
            }
        }

        public QuestLogMenu QuestLogMenu
        {
            get
            {
                return myQuestLogMenu;
            }
        }

        //adds a quest to the quest log for visual
        public void addQuestLogItem(string questName, string questDetails, Rectangle itemRect)
        {
            QuestLogItem temp = new QuestLogItem(myContentManager, questName, itemRect, questDetails);
            myQuestLogMenu.addQuestLogItem(temp);
        }


        //since we have a list of InventoryMenus, we will take in a parameter 
        //specifying what inventory in particular u want to set the data. 
        //ex: the players inventory
        public void setInventoryData(Dictionary<String, int> inventory,
            Dictionary<String, Texture2D> textures, String inventoryName)
        {
            myInventoryMenu.sendInventoryData(inventory, textures);
        }

        public InformationText getParticularInfoText(int index)
        {
            int count = 0;
            foreach (InformationText infoText in myInformationTextList)
            {
                if (count == index)
                {
                    return infoText;
                }

                count++;
            }

            return null;
        }

        public InformationText getParticularFloatingText(int index)
        {
            int count = 0;
            foreach (InformationText floatText in myFloatingTextList)
            {
                if (count == index)
                {
                    return floatText;
                }

                count++;
            }

            return null;
        }

        public InformationText getParticularInfoText(string displayText)
        {
            foreach (InformationText infoText in myInformationTextList)
            {
                if (infoText.DisplayText == displayText)
                {
                    return infoText;
                }
            }

            return null;
        }

        public InformationText getParticularFloatingText(string displayText)
        {
            foreach (InformationText floatText in myFloatingTextList)
            {
                if (floatText.DisplayText == displayText)
                {
                    return floatText;
                }
            }

            return null;
        }

        public void createFloatingText(string text, Vector2 position, Color color)
        {
            FloatingText tempFT = new FloatingText(myContentManager, text, position, color);
            tempFT.IsAlive = true;

            myFloatingTextList.Add(tempFT);
        }

        public void createInformationText(string text, Vector2 position, Color color)
        {
            InformationText tempIT = new InformationText(myContentManager, text, position, color);
            tempIT.IsAlive = true;
            if (myInformationTextList.Count > 0)
                myInformationTextList.RemoveAt(0);
            myInformationTextList.Add(tempIT);
        }

        public bool GameStarted
        {
            get
            {
                return myGameStarted;
            }
        }

        public Menu CurrentMenu
        {
            get
            {
                return myCurrentMenu;
            }
        }

        public string CurrentIndex
        {
            get
            {
                return myCurrentIndex;
            }
            set
            {
                myCurrentIndex = value;

                //change the current menu to the corresponding index
                foreach (Menu aMenu in myMenus)
                {
                    if (aMenu.Name == myCurrentIndex)
                    {
                        myCurrentMenu = aMenu;
                    }
                }
            }
        }

        public bool isShopMenuShowing()
        {
            if (myShopMenu.IsHidden)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        public void displayShopMenu()
        {
            if (myShopMenu.IsHidden)
            {
                myShopMenu.IsHidden = false;
            }
            else
            {
                myShopMenu.IsHidden = true;
            }

        }

        public void playConversation(string question, string[] responses, ResponseMethod callback, string fontName)
        {
            justinsMethod = callback;

            string id = question;
            Rectangle convoMenuRectangle = new Rectangle(100, 0, 600, 510);
            Rectangle convoTextAreaRectangle = new Rectangle(convoMenuRectangle.X + 100,
                                                             convoMenuRectangle.Y + 30,
                                                             400,
                                                             190);

            TextArea questTextArea = new TextArea(myContentManager, convoTextAreaRectangle, Color.White, Color.Black, fontName);
            myConvoMenu = new Menu(myContentManager, @"GUITiles\layout", convoMenuRectangle, questTextArea);

            questTextArea.addConversation(question);
            myConvoMenu.Name = id;

            for (int i = 0; i < responses.Length; i++)
            {
                Rectangle itemRect = new Rectangle(convoMenuRectangle.X + 100,
                                                   65 * i + 240,
                                                   400,
                                                   45);

                myConvoMenu.addCustomItem(i + responses[i], itemRect, Color.White, Color.Black, Color.Brown);
            }

            CurrentIndex = id;


        }

        public void Update(bool checkInput, GameTime gameTime)
        {
            //returns the index of what should be the current menu
            if (myCurrentMenu != null)
            {
                myUpdateReturnVal = myCurrentMenu.update(myGameStarted, checkInput, gameTime);
            }

            if (myPlayerGUI != null)
            {
                myPlayerReturnVal = myPlayerGUI.update(myGameStarted, checkInput, gameTime);
            }

            if (myConvoMenu != null)
            {
                myConvoReturnVal = myConvoMenu.update(myGameStarted, checkInput, gameTime);
            }

            if (myGameWorldName != null)
            {
                if (myGameWorldName.Equals("Pit Fall"))
                {
                    myPlayerGUI.IsHidden = true;
                }
                else
                {
                    myPlayerGUI.IsHidden = false;
                }
            }


            //If there were no problems with the menus update method
            if (myUpdateReturnVal != "")
            {
                if (myUpdateReturnVal == "quitGameEvent")
                {
                    myGame.Exit();
                }

                //if the screen should be the player GUI then the game has started
                if (myUpdateReturnVal == "playerGUI" && myCurrentMenu.Name == "splash")
                {
                    myGameStarted = true;
                    myCurrentMenu.IsHidden = true;

                    //switch the current menu to reflect what should be the current menu
                    //using the member reflects the changes properly
                    CurrentIndex = myUpdateReturnVal;
                }
                else if (myUpdateReturnVal == "playerGUI" && myCurrentMenu.IsPauseMenu == true)
                {
                    myCurrentMenu.IsHidden = true;
                    myCurrentMenu.IsPauseMenu = false;

                    //change game state to currently playing

                    //switch the current menu to reflect what should be the current menu
                    //using the member reflects the changes properly
                    CurrentIndex = myUpdateReturnVal;
                }
                else if (myUpdateReturnVal == "splash" && myGameStarted == true)
                {
                    myGameStarted = false;

                    //switch the current menu to reflect what should be the current menu
                    //using the member reflects the changes properly
                    CurrentIndex = myUpdateReturnVal;

                    myCurrentMenu.IsPauseMenu = false;
                }
                else
                {
                    //switch the current menu to reflect what should be the current menu
                    //using the member reflects the changes properly
                    CurrentIndex = myUpdateReturnVal;
                }

            }

            if (myGameStarted)
            {
                if (myPlayerReturnVal != "")//check UI button presses here
                {
                    if (myPlayerReturnVal == "pauseScreen" && myCurrentMenu.IsPauseMenu == false)
                    {
                        myCurrentMenu.IsHidden = false;
                        myCurrentMenu.IsPauseMenu = true;
                        //set game state to paused
                    }
                    else if (myPlayerReturnVal == "showInventoryEvent")
                    {
                        toggleDisplayInventory();
                    }
                    else if (myPlayerReturnVal == "showQuestLogEvent")
                    {
                        displayQuestLog();
                    }

                    CurrentIndex = myPlayerReturnVal;
                }

                foreach (FloatingText floatText in myFloatingTextList)
                {
                    if (floatText != null && floatText.IsAlive)
                    {
                        floatText.update(true, gameTime);
                    }
                }

                foreach (InformationText infoText in myInformationTextList)
                {
                    if (infoText != null && infoText.IsAlive)
                    {
                        infoText.update(true, gameTime);
                    }
                }

                if (!myInventoryMenu.IsHidden)
                {
                    myInventoryMenu.Update(true);
                }


                if (!myQuestLogMenu.IsHidden)
                {
                    myQuestLogMenu.Update(true);
                }

                if (myConvoReturnVal != null && myConvoReturnVal != "")
                {
                    //the first char in the string is a num corresponding to the
                    //reply so the AI knows what to respond with
                    justinsMethod(int.Parse(myConvoReturnVal.Substring(0, 1)));
                    myConvoReturnVal = null;
                }

                myShopReturnVal = myShopMenu.update(myMoney, true, gameTime);

                if (myShopReturnVal != "NOPE")
                {
                    string itemName;

                    myNewMoney = int.Parse(myShopReturnVal.Substring(0, myShopReturnVal.IndexOf('_')));
                    itemName = myShopReturnVal.Substring(myShopReturnVal.IndexOf('_') + 1);

                    createFloatingText("-" + (myMoney - myNewMoney), new Vector2(400, 200), Color.Red);
                    myMoney = myNewMoney;
                    
                    //give item
                    myPlayerInventory[itemName]++;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!myPlayerGUI.IsHidden)
            {
                myPlayerGUI.Draw(spriteBatch);

                spriteBatch.Begin();

                //Reputation Display
                spriteBatch.DrawString(myFontManager.getFont("money"),
                                       myRep.ToString(),
                                       new Vector2(100, 34),
                                       Color.Red);
                //Money Display
                spriteBatch.DrawString(myFontManager.getFont("money"),
                                       myMoney.ToString(),
                                       new Vector2(100, 120),
                                       Color.Yellow);

                spriteBatch.End();
            }

            if (!myShopMenu.IsHidden)
            {
                myShopMenu.draw(spriteBatch);
            }

            foreach (FloatingText floatText in myFloatingTextList)
            {
                if (floatText != null && floatText.IsAlive)
                {
                    floatText.Draw(spriteBatch);
                }
            }

            foreach (InformationText infoText in myInformationTextList)
            {
                if (infoText != null && infoText.IsAlive)
                {
                    infoText.Draw(spriteBatch);
                }
            }


            if (!myInventoryMenu.IsHidden)
            {
                myInventoryMenu.Draw(spriteBatch);
            }

            if (!myQuestLogMenu.IsHidden)
            {
                myQuestLogMenu.Draw(spriteBatch);
            }

            if (myConvoMenu != null)
            {
                myConvoMenu.Draw(spriteBatch);
            }

            if (myCurrentMenu.Name != myPlayerGUI.Name)
            {
                myCurrentMenu.Draw(spriteBatch);
            }

        }


        public void closeConversationMenu()
        {
            myConvoMenu = null;
            justinsMethod = null;
        }

        public void toggleDisplayInventory()
        {
            if (myInventoryMenu.IsHidden)
            {
                myInventoryMenu.IsHidden = false;
            }
            else
            {
                myInventoryMenu.IsHidden = true;
            }
        }

        public void displayQuestLog()
        {
            if (myQuestLogMenu.IsHidden)
            {
                myQuestLogMenu.IsHidden = false;
            }
            else
            {
                myQuestLogMenu.IsHidden = true;
            }
        }

        public Dictionary<String, int> Inventory
        {
            get
            {
                return myPlayerInventory;
            }
            set
            {
                myPlayerInventory = value;
            }
        }
    }
}
