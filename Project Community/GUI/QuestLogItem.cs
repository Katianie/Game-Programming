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
    public class QuestLogItem : MenuItem
    {
        private string myDetailedQuestInfo;

        public QuestLogItem(ContentManager contentManager, string questName, Rectangle buttonRect)
            : base(contentManager, questName, buttonRect,
            Color.White, Color.Black, Color.Yellow, "Whatever", true)
        {
            myDetailedQuestInfo = "";
        }

        public QuestLogItem(ContentManager contentManager, string questName, Rectangle buttonRect, string details)
            : base(contentManager, questName, buttonRect,
            Color.White, Color.Black, Color.Yellow,"Whatever" ,true)
        {
            myDetailedQuestInfo = details;
        }

        public string DetailedQuestInfo
        {
            get
            {
                return myDetailedQuestInfo;
            }
            set
            {
                myDetailedQuestInfo = value;
            }
        }

        public void Update(bool checkInput)
        {
            base.update(checkInput, null);
        }

        public void Draw(SpriteBatch spritebatch, SpriteFont font)
        {
            base.Draw(spritebatch);

            //spritebatch.Begin();

            //spritebatch.DrawString(font, myAssetName, new Vector2(myRect.X, myRect.Y), Color.Black);

            //spritebatch.End();

        }

        //this is a perfect example on how to override the .Equals method
        //that way when we use .contains on an array list of quest log items,
        //it will use this equals method to compare them.
        public override bool Equals(object obj)
        {
            QuestLogItem item = obj as QuestLogItem;

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
