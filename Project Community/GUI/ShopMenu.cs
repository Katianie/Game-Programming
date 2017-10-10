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
    public class ShopMenu : Menu
    {
        private ArrayList myShopMenuItems;
        private int myPlayersNewMoney;
        private string myBoughtItemName;

        public ShopMenu(Game game, ContentManager contentManager, string backgroundImage, Rectangle boundingRectangle, string inventroyName)
            : base(contentManager, backgroundImage, boundingRectangle, null)
        {
            base.IsHidden = true;
            //base.MenuColor = Color.Purple;

            myShopMenuItems = new ArrayList();

            InitializeItems(game);
        }

        //add items to shop
        public void InitializeItems(Game game)
        { 
            float xVal = 1;
            float yVal = 1;
            int width = 100;
            int height = 45;

            string[] itemNames =    { "Vaccine",                    "Axe",                    "Vaccine",                    "Vaccine",                    "Vaccine",                    "Vaccine",                    "Vaccine",                    "Vaccine",                    "Vaccine",                    "Vaccine" };
            int[] itemPrices =      { 10 ,                          100,                           10,                           10,                           10,                           10,                           10,                           10,                           10,                           10};
            string[] itemImgPaths = { @"GUITiles/vaccineInventory", @"GUITiles/axeInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory", @"GUITiles/vaccineInventory" }; 

            for (int i = 0; i < itemNames.Length; i++)
            {
                Rectangle shopItemRect = new Rectangle((int)(base.BoundingRectangle.X  * xVal) + 40 ,
                                                       (int)(base.BoundingRectangle.Y  * yVal) + 35,
                                                       width,
                                                       height);

                if (shopItemRect.Y > (base.BoundingRectangle.Height - height))
                {
                    yVal = 1;
                    xVal += 0.95f;
                }
                else
                {
                    yVal += 0.5f;
                }

                ShopMenuItem shopItem = new ShopMenuItem(game, myContentManager, @"GUITiles/Button", itemImgPaths[i], shopItemRect);

                shopItem.Price = itemPrices[i];
                shopItem.ItemName = itemNames[i];

                myShopMenuItems.Add(shopItem);
            }

        }

        public void AddShopMenuItem(ShopMenuItem item)
        {
            myShopMenuItems.Add(item);
        }

        public string update(int playersMoney, bool checkInput, GameTime gameTime)
        {
            base.update(true, checkInput, gameTime);

            foreach(ShopMenuItem item in myShopMenuItems)
            {
                myPlayersNewMoney = item.update(playersMoney, checkInput, gameTime);

                if (myPlayersNewMoney != -1)
                {
                    myBoughtItemName = item.ItemName;
                    return myPlayersNewMoney + "_" + myBoughtItemName;
                }
            }

            return "NOPE";
        }

        public void draw(SpriteBatch aBatch)
        {
            base.Draw(aBatch);

            foreach (ShopMenuItem item in myShopMenuItems)
            {
                item.draw(aBatch);
            }
        }
    }
}
