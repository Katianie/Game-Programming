using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Entities.Singletons;
using Microsoft.Xna.Framework.Graphics;
//testing commit

namespace Entities.World
{
    /// <summary>
    /// Represents a chop able tree
    /// </summary>
    /// <Owner>Edward Francis Katianie O'Hagan</Owner>
    public class Explosive : Entity
    {
        private Player.Player myPlayer;
        private GameWorld myGameWorld;
        private Vector2 myVelocity;
        private Random myRandom;

        bool once = false;
        int buffer = 7;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="et"></param>
        public Explosive(GameWorld gw, EntityType et)
            : base(gw, et)
        {
            myPlayer = EntityManager.getEntityManager(game).player;
            myGameWorld = gw;
            myVelocity = new Vector2(0, 0);
            myRandom = new Random(DateTime.Now.Second * 666999 * DateTime.Now.Millisecond);
        }


        /// <summary>
        /// Collide method
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            if (otherThing.isPlayer)
            {
                float negY = myPlayer.my_Body.GetLinearVelocity().Y * -1;
                float posX = myPlayer.my_Body.GetLinearVelocity().X;

                myVelocity = new Vector2(myRandom.Next(10, 100) * posX + 300, myRandom.Next(4, 70) * negY); ;
                
                //if (posX <= 3)
                //{
                //    myVelocity.X = myVelocity.X + 1000;
                //}

                myPlayer.my_Body.SetLinearVelocity(myVelocity);

                gameWorld.removeEntity(this);
            }

            base.collide(otherThing, touching);
        }


    }
}
