using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Entities.Singletons;
namespace Entities.World
{
    /// <summary>
    /// Represents an in game building
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class Building:Entity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="et"></param>
        public Building(GameWorld gw, EntityType et)
            : base(gw, et)
        {


        }

        /// <summary>
        /// Collide method
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            if (otherThing.isPlayer && InputManager.lastState.IsKeyDown(Keys.T))
            {
                if(ai != null)
                    ai.playCurrentConversation();
            }
            if (otherThing.isPlayer && base.gameWorld.name.Contains("Cannon Game"))
            {
                if (this.type.type == TypeOfThing.BUILDING)
                {
                    if (EntityManager.getEntityManager(base.game).player.myHasLaunched)
                    {
                        int currHighScore = EntityManager.getEntityManager(base.game).player.myCannonHighScore;
                        EntityManager.getEntityManager(base.game).player.myHasLaunched = false;
                        EntityManager.getEntityManager(base.game).player.LastScore = (EntityManager.getEntityManager(base.game).player.my_Body.GetPosition().X) -
                                        (EntityManager.getEntityManager(base.game).player.PlayerCannon.my_Body.GetPosition().X);

                        my_Body.SetLinearVelocity(Vector2.Zero);

                        if ((int)EntityManager.getEntityManager(base.game).player.LastScore > currHighScore)
                        {
                            currHighScore = (int)EntityManager.getEntityManager(base.game).player.LastScore;
                            EntityManager.getEntityManager(base.game).player.myCannonHighScore = currHighScore;

                        }
                        
                        EntityManager.getEntityManager(base.game).player.addEvent(new Event(EventList.ResetWorld,"1" + "," + "3"));
                    }
                }
            }
            //base.collide(otherThing, touching);
        }

        /// <summary>
        /// Handle Events
        /// </summary>
        public override void handleEvents()
        {
            while (eventList.Count > 0)
            {
                Event e = eventList[0];
                eventList.RemoveAt(0);
                if (e.type.Equals("Set_Building_Color"))
                {
                    if ((e._value as String).Equals("Blue"))
                        color = Color.Blue;
                    if ((e._value as String).Equals("Red"))
                        color = Color.Red;
                    if ((e._value as String).Equals("Green"))
                        color = Color.Green;
                }
                if (e.type.Equals("Repair_Building"))
                {
                    int cost = int.Parse(e._value as String);
                        if (animation == "_")
                            animation = "1";
                        else if (animation == "1")
                            animation = "2";
                        else if (animation == "2")
                            animation = "3";
                }
                if (e.type.Equals(EventList.QuestCompletedReturn))
                {
                    EntityManager em = EntityManager.getEntityManager(game);
                    Player.Player p = em.player;
                    String worldName = em.getCurrentGameWorld().name;
                    String worldToReturnTo = e._value as String;
                    if (p.activeQuest != null)
                    {
                        if (p.activeQuest.completedCallback != null)
                            p.activeQuest.completedCallback(null);
                    }
                    if (worldName != worldToReturnTo)
                    {
                        p.switchBody(worldToReturnTo);
                        em.reloadGameWorld(worldName);
                    }
                }
            }
        }
    }
}
