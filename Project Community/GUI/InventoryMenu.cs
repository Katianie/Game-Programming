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
using System.Collections.Generic;

namespace GUI
{
    public class InventoryMenu : Menu
    {
        private string myName;
        private MenuItem myNextPageButton;
        private MenuItem myPrevPageButton;
        private Rectangle myNextButtonRect;
        private Rectangle myPrevButtonRect;

        private ArrayList myInventoryItems;
        private SpriteFont myFont;
        private ContentManager myContentManager;
        private Dictionary<String, int> myInventoryData;

        public InventoryMenu(ContentManager contentManager, string backgroundImage, Rectangle boundingRectangle, string inventroyName)
            : base(contentManager, backgroundImage, boundingRectangle, null)
        {

            myName = inventroyName;

            myNextButtonRect = new Rectangle(0, 0, 100, 100);
            myPrevButtonRect = new Rectangle(0, 100, 100, 100);

            myNextPageButton = new MenuItem(contentManager, @"GUITiles/nextbutton", myNextButtonRect, Color.White, Color.Red, Color.Brown, "quest");
            myNextPageButton = new MenuItem(contentManager, @"GUITiles/prevbutton", myPrevButtonRect, Color.White, Color.Red, Color.Brown, "quest");

            myIsHidden = true;
            myIsPauseMenu = false;

            //load tiles and place them properly
            myInventoryItems = new ArrayList();

            myContentManager = contentManager;

        }

        public Dictionary<String, int> InventoryData
        {
            get
            {
                return myInventoryData;
            }
            set
            {
                myInventoryData = value;
            }
        }

        //receives the actual inventory data from the GUIManager. This
        //represents an individual inventory.
        public void sendInventoryData(Dictionary<String, int> inventory, Dictionary<String, Texture2D> textures)
        {
            int yRow = this.BoundingRectangle.Y + 50;
            int xCol = this.BoundingRectangle.X + 65;
            int itemWidth = 40;
            int itemHeight = 40;
            int itemsPerRow = 4;

            myInventoryData = inventory;

            myFont = GUI.FontManager.getFontManager(myContentManager).getFont("Whatever");

            int carriage = 0;//the thing that resets back to the left for the next line on a typewriter
            int i = 0;
            foreach(String imgStr in textures.Keys)
            {
                InventoryItem temp = new InventoryItem(textures[imgStr], new Rectangle(((this.BoundingRectangle.X / 2) * carriage) + xCol, yRow, itemWidth, itemHeight));

                if ((i + 1) % itemsPerRow == 0)
                {
                    yRow += (itemHeight + 10);
                    carriage = 0;
                }
                else
                {
                    carriage++;
                }

                temp.Quantity = inventory[imgStr];

                myInventoryItems.Add(temp);

                i++;
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


        public void Update(bool checkInput)
        {
            if (checkInput)
            {
                foreach (InventoryItem item in myInventoryItems)
                {
                    item.Update(checkInput);
                }
            }

            base.update(true, checkInput, null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            //make semi transparent
            myMenuColor.A = (byte)(255 / 1.5f);

            base.Draw(spriteBatch);

            foreach (InventoryItem item in myInventoryItems)
            {
                item.Draw(spriteBatch, myFont);
            }
        }
    }
}
