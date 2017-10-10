using System;
using Microsoft.Xna.Framework;
using Entities.World;
using Entities.Singletons;
namespace Entities.AI
{
    /// <summary>
    /// AI for the cotton shooter game.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class CottonShooter:AIBase
    {
        private int timeRemaining;
        private int lastTR;
        private readonly int gameLength = 45;
        private int interval = 300;
        private readonly Vector2 position;
        private Vector2 velocity;
        private DateTime start;
        private bool started;// = false;
        private bool decInterval = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        /// <param name="e"></param>
        public CottonShooter(GameWorld _gameWorld, Entity e)
            : base(_gameWorld,e)
        {
            position = new Vector2(1f,9.5f);
            velocity = new Vector2(10,-7);          
        }

        /// <summary>
        /// Spawns a cotton ball with random velocity.
        /// </summary>
        public void spawn()
        {
            velocity = new Vector2(random.Next(30,75)/10.0f, random.Next(-100,-50)/10.0f);
            Entity newEntity = new Item(gameWorld, EntityManager.getEntityManager(gameWorld.getGame()).getType("CottonBallBlock"),"Cotton");
            newEntity.color = Color.White;
            newEntity.type.type = TypeOfThing.ITEM2;
            newEntity.animation = "Ball";
            gameWorld.addEntity(newEntity, null);
            newEntity.my_Body = gameWorld.AddBody(position, newEntity);
            newEntity.my_Body.SetLinearVelocity(velocity);
        }

        /// <summary>
        /// Update.  All shooter game logic.
        /// </summary>
        public override void update()
        {
            if (!started)
            {
                start = DateTime.Now;
                started = true;
            }
            timeRemaining = gameLength - (DateTime.Now - start).Seconds;
            EntityManager em = EntityManager.getEntityManager(gameWorld.getGame());

            if (timeRemaining < 0)
            {
                em.player.addEvent(
                    new Event(EventList.GrabIRemainingtems, gameWorld.getEntities()));
            }
            else
            {
                if (timeRemaining > 5 && timeRemaining < gameLength - 5 && random.Next() % interval == 0)
                {
                    spawn();
                }
                if (timeRemaining > gameLength - 5)
                {
                     if (timeRemaining != lastTR)
                    {
                    GUI.GUIManager.getGUIManager(gameWorld.game, gameWorld.game.Content).createFloatingText(timeRemaining - (gameLength - 5) + "!", new Vector2(400, 300), Color.Azure);
                    lastTR = timeRemaining;   
                     }
                }
                else if (timeRemaining <= 5 && decInterval)
                {
                    if (timeRemaining != lastTR)
                    {
                        GUI.GUIManager.getGUIManager(gameWorld.game, gameWorld.game.Content).createFloatingText(timeRemaining + "!", new Vector2(400, 300), Color.Azure);
                        lastTR = timeRemaining;
                    }
                                        }
                if (timeRemaining % 5 == 0 && interval > 25)
                {
                    if (decInterval)
                    {
                        interval -= 10;
                        decInterval = false;
                    }
                }
                else
                    decInterval = true;
                em.clearStringList();
                em.drawString("Time Left: " + timeRemaining);
                //timeRemaining--;
            }
        }
    }
}
