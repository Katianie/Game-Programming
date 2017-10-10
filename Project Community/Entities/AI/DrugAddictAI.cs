using Microsoft.Xna.Framework;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// Class for drug addict enemy ai.  Randomly walks back and forth in 2D sidescroller.
    /// </summary>
    class DrugAddictAI:AIBase
    {
        float speed = 3;
        Vector2 velocity = new Vector2(3,0);
        private readonly int interval = 50;
        bool pause = false;
        int time = 50;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld">Gameworld reference</param>
        /// <param name="e">Entity attached to.</param>
        public DrugAddictAI(GameWorld _gameWorld, Entity e):base(_gameWorld,e)
        {
        }

        /// <summary>
        /// Collide method.  Removes this guy if he collides with the bottom edge.
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="isTouching"></param>
        public override void collide(Entity otherThing, bool isTouching)
        {
            if (isTouching && otherThing.type.type == TypeOfThing.WALL)
            {
                if((otherThing as WorldEdge).side == SIDE.BOTTOM)
                     gameWorld.removeEntity(entity);
            }
            base.collide(otherThing, isTouching);
        }

        /// <summary>
        /// Update.  Random back and forth movement with random pausing.
        /// </summary>
        public override void update()
        {
            if (pause)
            {
                if (random.Next(0, 100) == 0)
                    pause = false;
                entity.animation = "Walking Down";
                return;
            }
            if (time <= 0)
            {
                speed *= -1;
                time = interval;
                pause = true;

            }
            else
                time--;

            velocity = entity.my_Body.GetLinearVelocity();
            velocity.X = speed;
            entity.my_Body.SetLinearVelocity(velocity);
        }

    }
}
