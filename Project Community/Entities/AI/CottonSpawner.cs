using System;
using Microsoft.Xna.Framework;
using Entities.World;
using Entities.Singletons;
namespace Entities.AI
{
    /// <summary>
    /// AI for the cotton dropping game.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class CottonSpawner : AIBase
    {
        private readonly int gameLength = 59;
        private int lastTR;
        private int timeRemaining;
        private int interval = 250;
        private DateTime start;
        private bool started;// = false;
        private bool decInterval = true;



        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        /// <param name="e"></param>
        public CottonSpawner(GameWorld _gameWorld, Entity e):base(_gameWorld,e)
        {
        }

        /// <summary>
        /// Spawn a new cotton ball at a random location.
        /// </summary>
        public void spawn()
        {
            Entity newEntity = new Item(gameWorld,EntityManager.getEntityManager(gameWorld.game).getType("CottonBallBlock"),"Cotton");
            newEntity.color = Color.White;
            newEntity.type.type = TypeOfThing.ITEM2;
            newEntity.animation = "Ball";
            gameWorld.addEntity(newEntity,null);
            newEntity.my_Body = gameWorld.AddBody(randomSpawnPosition(), newEntity);
        }

        /// <summary>
        /// Returns a random spawn position
        /// </summary>
        /// <returns></returns>
        public Vector2 randomSpawnPosition()
        {
            return new Vector2(random.Next(1,750)/100.0f,random.Next(150,450)/100.0f);
        }


        /// <summary>
        /// Update
        /// </summary>
        public override void update()
        {
            if (!started)
                start = DateTime.Now;
            started = true;
            timeRemaining = gameLength - (DateTime.Now - start).Seconds;
            EntityManager em =  EntityManager.getEntityManager(gameWorld.getGame());

            if (timeRemaining <= 0)
            {
                em.player.addEvent(
                    new Event(EventList.GrabIRemainingtems, gameWorld.getEntities()));
            }
            else
            {
                if (timeRemaining > 5 && timeRemaining < gameLength - 10 && interval <= 0)
                {
                    spawn();
                    interval = random.Next(50, 150);
                }
                else
                    interval--;
                if (timeRemaining > gameLength - 10)
                {
                    if (timeRemaining != lastTR)
                    {
                        GUI.GUIManager.getGUIManager(gameWorld.game, gameWorld.game.Content).createFloatingText(timeRemaining - (gameLength - 10) + "!", new Vector2(400, 300), Color.Azure);
                        lastTR = timeRemaining;
                    }
               }
                em.clearStringList();
                em.drawString("Time Left: " + timeRemaining);
                timeRemaining--;
            }
        }

     

    }
}
