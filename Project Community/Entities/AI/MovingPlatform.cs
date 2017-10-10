using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Entities.AI
{
    /// <summary>
    /// A moving platform.  
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class MovingPlatform:AIBase
    {
        private readonly List<Vector2> speeds;
        private readonly List<int> times;
        int state = 0;
        int time = 0;
        Vector2 velocity;
        Vector2 axis;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld">Gameworld reference</param>
        /// <param name="e">Entity attached to.</param>
        public MovingPlatform(World.GameWorld _gameWorld, World.Entity e):base(_gameWorld,e)
        {
            speeds = new List<Vector2>();
            times = new List<int>();
            axis = new Vector2(0,5);

        }

        /// <summary>
        /// Update. Controls the movement of the platform.
        /// </summary>
        public override void update()
        {
            if (time <= 0)
            {
                entity.my_Body.noGravity = true;
                state++;
                if (state >= speeds.Count)
                    state = 0;
                velocity = speeds[state];
                time = times[state];
            }
            else
                time--;
            lockAxis();
            entity.my_Body.SetLinearVelocity(velocity);
        }

        /// <summary>
        /// Keeps the platform on its axis.
        /// </summary>
        private void lockAxis()
        {
            Vector2 pos = entity.my_Body.Position;
            if (axis.Y != 0)
                pos.Y = axis.Y;
            else
                pos.X = axis.X;
            entity.my_Body.Position = pos;
        }

        /// <summary>
        /// Read in the movement data from the aiArgs value.
        /// </summary>
        public override void init()
        {
            List<String> list = aiArgs.Split(',').ToList();
            axis.X = float.Parse(list[0]);
            axis.Y = float.Parse(list[1]);
            int i = 2;
            while (i < list.Count)
            {
                times.Add(int.Parse(list[i]));
                Vector2 tSpeed = new Vector2(float.Parse(list[++i]), float.Parse(list[++i]));
                speeds.Add(tSpeed);
                i++;
            }
        }
    }
}
