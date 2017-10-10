using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.World
{
    /// <summary>
    /// Used when an AI is needed but there is no physical entity to attach it to.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class NonEntity : Entity
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="et"></param>
        public NonEntity(GameWorld gw, EntityType et = null) :base(gw,et)
        {

        }

        /// <summary>
        /// Doesn't collide 
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            throw new Exception("This Should Never be called.");
        }

        /// <summary>
        /// Not drawn either
        /// </summary>
        /// <param name="sb"></param>
        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            //No
        }

        /// <summary>
        /// Never collides with anything
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public override bool shouldCollide(Entity ent)
        {
            return false;
        }


        /// <summary>
        /// Never animates
        /// </summary>
        public override void animate()
        {
            //Just say no
        }

        public override bool AABBQueryCallback(Box2D.XNA.Fixture fixture)
        {
            throw new Exception("SHould not be called.");
        }
    }
}
