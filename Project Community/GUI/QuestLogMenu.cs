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
    public class QuestLogMenu : Menu
    {
        private ArrayList myQuestLogItems;
        private MenuItem myNextPageButton;
        private MenuItem myPrevPageButton;
        private SpriteFont myFont;

        public QuestLogMenu(ContentManager contentManager, string backgroundImage, Rectangle menuRectangle, TextArea informationArea) 
            : base(contentManager, backgroundImage, menuRectangle, informationArea)
        {
            base.IsHidden = true;
            myQuestLogItems = new ArrayList();
            myNextPageButton = new MenuItem(contentManager, @"GUITiles\nextButton", menuRectangle, Color.White, Color.Red, Color.Brown, "quest");
            myPrevPageButton = new MenuItem(contentManager, @"GUITiles\prevButton", menuRectangle, Color.White, Color.Red, Color.Brown, "quest");

            myFont = GUI.FontManager.getFontManager(myContentManager).getFont("Whatever");
        }

        public void addQuestLogItem(QuestLogItem questLogItem)
        {
            if (!myQuestLogItems.Contains(questLogItem))
            {
                myQuestLogItems.Add(questLogItem);
            }
        }

        public void Update(bool checkInput)
        {
            if (checkInput)
            {
                foreach (QuestLogItem item in myQuestLogItems)
                {
                    item.Update(checkInput);
                }

            }

            base.update(true, checkInput, null);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (QuestLogItem item in myQuestLogItems)
            {
                item.Draw(spriteBatch, myFont);
            }

            if (myTextArea != null)
            {
                myTextArea.Draw(spriteBatch);
            }
        }

    }
}
