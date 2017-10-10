using Microsoft.Xna.Framework;
using Entities.World;
using Entities.Singletons;
namespace Entities.AI
{
    /// <summary>
    /// Player AI will count the solar panels and open the exit to the maze.
    /// Possibly, rearrange the maze all together which would be wicked awesome.
    /// Works for the first maze.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class MazeTraverserAI:AIBase
    {
        private Entity block;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld">Gameworld reference</param>
        /// <param name="e">Entity attached to.</param>
        public MazeTraverserAI(GameWorld _gameWorld, Entity e)
            : base(_gameWorld,e)
        {
                Entity newEntity = new Building(gameWorld, EntityManager.getEntityManager(gameWorld.game).getType("TreeBuilding"));
                newEntity.type.type = TypeOfThing.BUILDING;
                newEntity.animation = "_";
                newEntity.my_Body= gameWorld.AddBody(new Vector2(61f, 5f), newEntity);
                gameWorld.addEntity(newEntity,null);
                newEntity.color = Color.Red;
            block = newEntity;
               // navStuff.addRectangle(new Rectangle((int)(xPos * 64), (int)(yPos * 64), (int)newEntity.type.size.X, (int)newEntity.type.size.Y));
            
        }
        
        /// <summary>
        /// Update. Checks to see if the objects have been grabbed and the exit can be opened.
        /// </summary>
        public override void update()
        {
            Player.Player p = (entity as Player.Player);
            if (p.inventory["Solar"] >= 10)
            {
                if(block != null)
                    gameWorld.removeEntity(block);
                block = null;
            }
        }
    }
}
