using Microsoft.Xna.Framework;
using Entities.World;
using Entities.Singletons;
namespace Entities.AI
{
    /// <summary>
    /// Works for the 2nd maze.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class MazeTraverserAI2:AIBase
    {
        private Entity block;
        private Entity block2;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld">Gameworld reference</param>
        /// <param name="e">Entity attached to.</param>
        public MazeTraverserAI2(GameWorld _gameWorld, Entity e)
            : base(_gameWorld,e)
        {
                Entity newEntity = new Building(gameWorld, EntityManager.getEntityManager(gameWorld.game).getType("TreeBuilding"));
                newEntity.type.type = TypeOfThing.BUILDING;
                newEntity.animation = "_";
                newEntity.my_Body= gameWorld.AddBody(new Vector2(35.5f, 108f), newEntity);
                gameWorld.addEntity(newEntity,null);
                newEntity.color = Color.Red;
                block = newEntity;

                newEntity = new Building(gameWorld, EntityManager.getEntityManager(gameWorld.game).getType("TreeBuilding"));
                newEntity.type.type = TypeOfThing.BUILDING;
                newEntity.animation = "_";
                newEntity.my_Body = gameWorld.AddBody(new Vector2(37.5f, 107f), newEntity);
                gameWorld.addEntity(newEntity, null);
                newEntity.color = Color.Red;
                block2 = newEntity;   
        }
        
        /// <summary>
        /// Update. Checks to see if the objects have been grabbed and the exit can be opened.
        /// </summary>
        public override void update()
        {
            Player.Player p = (entity as Player.Player);
            if (p.inventory["Wire"] >= 3 || p.inventory["LED"] >= 6 || p.inventory["PowerSupply"] >= 1)
            {
                if (block != null)
                {
                
                gameWorld.removeEntity(block);
                block = null;
            }
                if (block2 != null)
                {

                    gameWorld.removeEntity(block2);
                    block2 = null;
                }
        }
        }
    }
}
