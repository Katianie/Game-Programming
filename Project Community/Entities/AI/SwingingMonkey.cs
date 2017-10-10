using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Box2D.XNA;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// AI for a randomly swinging monkey.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class SwingingMonkey :AIBase
    {
        private int pauseTime = 0;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld">Gameworld this is in</param>
        /// <param name="e">Entity to attach to</param>
        public SwingingMonkey(GameWorld _gameWorld, Entity e) :base(_gameWorld,e) {  }

        /// <summary>
        /// Update method. Randomly applies force.
        /// </summary>
        public override void update()
        {
            if (pauseTime <= 0)
            {
                entity.my_Body.SetLinearVelocity(new Vector2(-5,0));
                pauseTime = random.Next(150, 350);
            }
            else
                pauseTime--;
        }
    }
}