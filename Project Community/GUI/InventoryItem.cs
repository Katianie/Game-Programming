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
    public class InventoryItem : MenuItem
    {
        private int myQuantity;
        private ContentManager myContentManager;
        private Texture2D texture2D;
        private Rectangle rectangle;

        public InventoryItem(ContentManager contentManager, string textureName, Rectangle itemRect)
            : base(contentManager, textureName, itemRect, Color.Gray, Color.Black, Color.LightGray, "quest")
        {
            myQuantity = 0;
        }

        public InventoryItem(ContentManager contentManager, string textureName, Rectangle itemRect, int quantity)
            : base(contentManager, textureName, itemRect, Color.White, Color.Black, Color.Yellow, "quest")
        {
            myQuantity = quantity;
        }

        public InventoryItem(Texture2D texture2D, Rectangle itemRect)
            : base(texture2D, itemRect, Color.Gray, Color.Black, Color.LightGray, "quest", false)
        {
            myQuantity = 0;
        }

        public InventoryItem(Texture2D texture2D, Rectangle itemRect, int quantity)
            : base(texture2D, itemRect, Color.White, Color.Black, Color.Yellow, "quest", false)
        {
            myQuantity = quantity;
        }


        public int Quantity
        {
            get
            {
                return myQuantity;
            }
            set
            {
                myQuantity = value;
            }
        }

        public void Update(bool checkInput)
        {
            base.update(checkInput, null);
        }

        public void Draw(SpriteBatch spritebatch, SpriteFont font)
        {
            spritebatch.Begin();

            if(myQuantity > 0)
            {
                spritebatch.Draw(base.Texture, base.BoundingRectangle, Color.White);

                spritebatch.DrawString(font, myQuantity.ToString(), new Vector2(base.BoundingRectangle.X + (base.BoundingRectangle.Width * 0.5f),
                                                                                base.BoundingRectangle.Y + (base.BoundingRectangle.Height * 0.5f)), Color.SandyBrown);
            }
            else
            {
                spritebatch.Draw(base.Texture, base.BoundingRectangle, Color.DarkGray);

                spritebatch.DrawString(font, myQuantity.ToString(), new Vector2(base.BoundingRectangle.X + (base.BoundingRectangle.Width * 0.5f),
                                                                                base.BoundingRectangle.Y + (base.BoundingRectangle.Height * 0.5f)), Color.DarkGray);
            }

            spritebatch.End();
            
        }
    }
}
