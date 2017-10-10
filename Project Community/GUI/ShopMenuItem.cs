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
//using Entities;
using System.Threading;

namespace GUI
{
    public class ShopMenuItem : MenuItem
    {
        private int myPrice;
        private int myPauseCounter;
        private string myItemName;
        private Texture2D myLogo;
        private Rectangle myLogoRect;
        private SpriteFont myFont;
        private int myPlayersMoney;//used to calculate color of price text

        public ShopMenuItem(Game game, ContentManager contentManager, string textureName, string logoTextureName, Rectangle itemRect)
            : base(contentManager, textureName, itemRect, Color.White, Color.Black, Color.Brown, "quest")
        {
            base.IsClickable = true;
            myPrice = 0;
            myPauseCounter = 9;
            myLogo = contentManager.Load<Texture2D>(logoTextureName);
            myLogoRect = new Rectangle(base.BoundingRectangle.X,
                                       base.BoundingRectangle.Y + (base.BoundingRectangle.Height / 4),
                                       base.BoundingRectangle.Width / 2, 
                                       base.BoundingRectangle.Height / 2);

            myFont = FontManager.getFontManager(contentManager).getFont("Opium");
        }

        public ShopMenuItem(Game game, ContentManager contentManager, string textureName, string logoTextureName, Rectangle itemRect, int price)
            : base(contentManager, textureName, itemRect, Color.White, Color.Black, Color.Brown, "quest")
        {
            base.IsClickable = true;

            myPrice = price;
            myPauseCounter = 9;
            myLogo = contentManager.Load<Texture2D>(logoTextureName);
            myLogoRect = new Rectangle(base.BoundingRectangle.X,
                                       base.BoundingRectangle.Y + (base.BoundingRectangle.Height / 4),
                                       base.BoundingRectangle.Width / 2,
                                       base.BoundingRectangle.Height / 2);

            myFont = FontManager.getFontManager(contentManager).getFont("Opium");
        }

        public int Price
        {
            get
            {
                return myPrice;
            }
            set
            {
                myPrice = value;
            }
        }

        public string ItemName
        {
            get
            {
                return myItemName;
            }
            set
            {
                myItemName = value;
            }
        }

        public int update(int playersMoney, bool checkInput, GameTime gameTime)
        {
            base.update(checkInput, gameTime);

            int newMoney = playersMoney;
            myPlayersMoney = playersMoney;

            if (isClicked(checkInput))
            {
                myPauseCounter--;

                if (myPauseCounter <= 0)
                {
                    myPauseCounter = 9;

                    if (playersMoney >= myPrice)
                    {
                        newMoney = playersMoney - myPrice;
                        return newMoney;
                    }
                }
            }

            return -1;
        }

        public void draw(SpriteBatch aBatch)
        {
            base.Draw(aBatch);

            aBatch.Begin();

            aBatch.Draw(myLogo, myLogoRect, Color.White);

            if (myPlayersMoney >= myPrice)
            {
                aBatch.DrawString(myFont, "$" + myPrice, new Vector2(myRect.X + 5 + (myRect.Width / 2), myRect.Y + (myRect.Height / 4)), Color.Black);
            }
            else
            {
                aBatch.DrawString(myFont, "$" + myPrice, new Vector2(myRect.X + 5 + (myRect.Width / 2), myRect.Y + (myRect.Height / 4)), Color.Red);
            }

            aBatch.End();
        }
    }

}
