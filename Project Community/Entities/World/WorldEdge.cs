using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities.World
{
    public enum SIDE {LEFT,RIGHT,TOP,BOTTOM}

    /// <summary>
    /// Representation of a world edge (edge of map) as an entity so collision with the edges can be meaningful.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class WorldEdge : Entity
    {
        public SIDE side = SIDE.BOTTOM;
        public Event collideEvent;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="_side"></param>
        public WorldEdge(GameWorld gw,SIDE _side)
            : base(gw,null)
        {
            side = _side;
        }

        /// <summary>
        /// Never does anything
        /// </summary>
        public override void animate()
        {
            
        }
        /// <summary>
        /// Never does anything
        /// </summary>
        public override void draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            //base.draw(sb);
        }

        /// <summary>
        /// Never does anything
        /// </summary>
        public override void handleEvents()
        {
            //base.handleEvents();
        }

        /// <summary>
        /// Passes its collide Event to the thing it is colliding with.
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            if (touching && collideEvent != null)
            {
                if (otherThing.isPlayer)
                    otherThing.addEvent(collideEvent);
                else
                {
                    if (collideEvent.type != EventList.QuestCompletedReturn)
                        otherThing.addEvent(collideEvent);
                }
            }
            base.collide(otherThing, touching);
        }
    }
}
