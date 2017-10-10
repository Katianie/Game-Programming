using Microsoft.Xna.Framework;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// Rice grabbing player AI.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class RiceGrabber:AIBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        /// <param name="e"></param>
        public RiceGrabber(GameWorld _gameWorld, Entity e):base(_gameWorld,e)
        {

        }

        /// <summary>
        /// Collide method. If item grab it.
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="isTouching"></param>
        public override void collide(Entity otherThing, bool isTouching)
        {
            if (otherThing.type.type == TypeOfThing.ITEM)
            {
                grabRice(otherThing as Item);
            }
            //base.collide(otherThing, isTouching);
        }

        /// <summary>
        /// Connects the item to the player with a distance joint.
        /// </summary>
        /// <param name="itemToGrab">Item to grab.</param>
        private void grabRice(Item itemToGrab)
        {
            if (!entity.areJoined(itemToGrab.my_Body))
            {
                Vector2 center = entity.my_Body.GetWorldCenter();
                center.X += .5f;
                center.Y += .5f;
                if (entity.my_Body.GetJointList() == null)
                {
                    center.X += .2f;
                    center.Y += .2f;
                }
                itemToGrab.my_Body.Position = center;
                gameWorld.joinBodies_Distance(entity.my_Body, itemToGrab.my_Body);
            }
        }
    }
}
