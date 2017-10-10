using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Entities.Singletons;

namespace Entities.World
{
    public class SoccerGameWorld : GameWorld
    {
        /// <summary>
        /// AI for the cotton dropping game.
        /// </summary>
        /// <Owner>Justin Dale</Owner>

        TimeSpan ts = new TimeSpan(0, 5, 30);
        private int lastTR;
        TimeSpan timeElapsed;
        private Random random;
        private DateTime start;
        private bool started = false;
        SpriteBatch myDrawStuff;
        SpriteFont myFont;
        TimeSpan myTemp;
        GameWorld gameworld;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        /// <param name="e"></param>
        public SoccerGameWorld(Game _game, float width, float height, int tileWidth, String _name)
            : base(_game, width, height, tileWidth, _name)
        {

            random = new Random(DateTime.Now.Millisecond);
            myDrawStuff = new SpriteBatch(base.game.GraphicsDevice);
        }

        /// <summary>
        /// Spawn a new cotton ball at a random location.
        /// </summary>
        /*public void spawn()
        {
            Entity newEntity = new Item(gameWorld, EntityManager.getEntityManager(gameWorld.game).getType("CottonBallBlock"), "Cotton");
            newEntity.color = Color.White;
            newEntity.type.type = TypeOfThing.ITEM2;
            newEntity.animation = "Ball";
            gameWorld.addEntity(newEntity, null);
            newEntity.my_Body = gameWorld.AddBody(randomSpawnPosition(), newEntity);
        }*/

        /// <summary>
        /// Returns a random spawn position
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Update
        /// </summary>
        public override void update(GameTime gt)
        {
            if (!started)
            {
                start = DateTime.Now;
            }

            started = true;
            timeElapsed = (DateTime.Now - start);
            EntityManager em = EntityManager.getEntityManager(base.getGame());

            if (timeElapsed > ts)
            {
                em.player.addEvent(
                    new Event(EventList.QuestCompletedReturn, base.getEntities()));
            }
            else
            {
                myTemp = ts - timeElapsed;
                myFont = GUI.FontManager.getFontManager(base.game.Content).getFont("Whatever");

                base.update(gt);

            }
        }

        

        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {


            base.draw(sb);
            sb.Begin();
            Vector2 pos = new Vector2(300, 50);
            if (myTemp.Seconds >= 10)
            {
                sb.DrawString(myFont, ("Time Left: " + myTemp.Minutes + ":" + myTemp.Seconds), pos, Color.Black);
            }
            else
            {
                sb.DrawString(myFont, ("Time Left: " + myTemp.Minutes + ":" +  0 + myTemp.Seconds), pos, Color.Black);
            }

            sb.End();
            //base.draw(sb);
        }

    }
}
