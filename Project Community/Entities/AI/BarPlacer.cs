using Microsoft.Xna.Framework;
using Entities.World;
using Entities.Singletons;
using System.Collections.Generic;
namespace Entities.AI
{
    /// <summary>
    /// Bar Placer Player for cotton catch game world
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class BarPlacer : AIBase
    {
        private int numberOfBars = 6;
        private List<Entity> playerEnts;
        private int index = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        /// <param name="e"></param>
        public BarPlacer(GameWorld _gameWorld, Entity e):base(_gameWorld,e)
        {
            EntityManager em = EntityManager.getEntityManager(gameWorld.game);
            e.color = Color.Blue;
            playerEnts = new List<Entity>();
            playerEnts.Add(new Entity(gameWorld, em.getType("BarPlayer0")));
            playerEnts[0].my_Body = gameWorld.AddBody(new Vector2(-2, -2), em.getType("BarPlayer0"), playerEnts[0]);


            playerEnts.Add(new Entity(gameWorld, em.getType("BarPlayer1")));
            playerEnts[1].my_Body = gameWorld.AddBody(new Vector2(-2, -2), em.getType("BarPlayer1"), playerEnts[1]);

            playerEnts.Add(new Entity(gameWorld, em.getType("BarPlayer2")));
            playerEnts[2].my_Body = gameWorld.AddBody(new Vector2(-2, -2), em.getType("BarPlayer2"), playerEnts[2]);

            playerEnts.Add(new Entity(gameWorld, em.getType("BarPlayer3")));
            playerEnts[3].my_Body = gameWorld.AddBody(new Vector2(-2, -2), em.getType("BarPlayer3"), playerEnts[3]);

            playerEnts.Add(new Entity(gameWorld, em.getType("BarPlayer4")));
            playerEnts[4].my_Body = gameWorld.AddBody(new Vector2(-2, -2), em.getType("BarPlayer4"), playerEnts[3]);

            playerEnts.Add(new Entity(gameWorld, em.getType("BarPlayer5")));
            playerEnts[5].my_Body = gameWorld.AddBody(new Vector2(-2, -2), em.getType("BarPlayer5"), playerEnts[5]);

            foreach (var playerEnt in playerEnts)
            {
                playerEnt.my_Body.SetActive(false);
            }

        }


        /// <summary>
        /// Locks current position, leaving a static version behind
        /// </summary>
        public void placeBar()
        {
            if (numberOfBars > 0)
            {
                EntityManager em = EntityManager.getEntityManager(gameWorld.getGame());
                var newEntity = new Building(gameWorld, em.getType("BarBuilding" + index));
                index++;
                newEntity.color = Color.White;
                newEntity.type.type = TypeOfThing.BUILDING;
                newEntity.animation = "_";
                gameWorld.addEntity(newEntity, null);
                newEntity.my_Body = gameWorld.AddBody(entity.my_Body.Position, newEntity);
                newEntity.my_Body.Rotation = entity.my_Body.Rotation;
                newEntity.isPlayer = true;

                numberOfBars--;
                if (numberOfBars == 0)
                {
                    entity.my_Body.Position = new Vector2(-2, -2);
                    entity.my_Body.SetActive(false);
                }
                else
                {
                    //index = random.Next(0, 5);
                    entity = playerEnts[index];
                    entity.my_Body.Position = new Vector2(newEntity.my_Body.Position.X, newEntity.my_Body.Position.Y - .75f);
                    entity.isPlayer = true;
                    em.player.type = entity.type;
                    em.player.my_Body = entity.my_Body;
                    entity.my_Body.noGravity = true;
                    entity.my_Body.SetActive(true);
                }


        }
        }

        /// <summary>
        /// Update.  Rotates the bar if Q or E are held down.
        /// </summary>
        public override void update()
        {
            if (InputManager.lastState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Q))
                entity.my_Body.Rotation += .02f;
            if (InputManager.lastState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.E))
                entity.my_Body.Rotation -= .02f;
        }
    }
}
