using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
//This code is a modified version of the code laid out in the tutorial below.
//http://rbwhitaker.wikidot.com/2d-particle-engine-1
namespace Particle_Engine
{
    public class Particle
    {
        public Texture2D texture {get; set;} //texture that will be drawn to represent particle
        public Vector2 position {get; set;} //current position of the particle
        public Vector2 velocity {get; set;} //current speed of the particle
        public Color color {get; set;}

        public float angle {get; set;} 
        public float angularVelocity {get; set;} //speed that the angle is changeing
        public float size {get; set;}

        public int timeToLive{get; set;} //the life span of the particle 

        public Particle(Texture2D aTexture, Vector2 aPos, Vector2 aVelocity, Color aColor, float aAngle,
            float aAngularVelocity, float aSize, int aTimeToLive)
        {
            texture = aTexture;
            position = aPos;
            velocity = aVelocity;
            color = aColor;

            angle = aAngle;
            angularVelocity = aAngularVelocity;
            size = aSize;

            timeToLive = aTimeToLive;

        }

        public void Update(Vector2 dif)
        {
            timeToLive--;
            position += velocity;
            position -= dif;
            angle += angularVelocity;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Rectangle sourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
            Vector2 origin = new Vector2(texture.Width / 2, texture.Height / 2);

            spriteBatch.Draw(texture, position, sourceRectangle, color, angle, origin, size, SpriteEffects.None, 0f);

        }
    }
}
