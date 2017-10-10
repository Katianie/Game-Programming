using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

//This code is a modified version of the code laid out in the tutorial below.
//http://rbwhitaker.wikidot.com/2d-particle-engine-1
namespace Particle_Engine
{
    public class ParticleEngine
    {
        private Random myRand;
        private List<Particle> myParticleList;
        private List<Texture2D> myTextureList;

        public Vector2 EmitterLocation { get; set; }
        public Vector2 lastEmitterLocation { get; set; }
        public ParticleEngine(List<Texture2D> textureList, Vector2 emitterPos)
        {
            myRand = new Random();
            myTextureList = textureList;
            myParticleList = new List<Particle>();
            EmitterLocation = emitterPos;
            lastEmitterLocation = EmitterLocation;
        }

        private Particle GenerateNewParticle()
        {
            Texture2D theTexture = myTextureList[myRand.Next(myTextureList.Count)];
            Vector2 velocity = new Vector2((float)myRand.NextDouble() * 3 - 1,(float)myRand.NextDouble() * -5 );
            float angle = 0;
            float angularVelocity = 0.1f * (float)(myRand.NextDouble() * 3 - 1);

            int randR = myRand.Next(0, 255);
            int randG = 0;
            int randB = 255;
            
            Color theColor = new Color(randR, randG, randB);
            float size = 1;//myRand.Next(2);
            int ttl = myRand.Next(18,23);

            return new Particle(theTexture, EmitterLocation, velocity, theColor, angle, angularVelocity, size, ttl);
        }

        public void Update()
        {
            int total = 6;
            Vector2 dif = lastEmitterLocation - EmitterLocation;

            for (int i = 0; i < total; i++)
            {
                myParticleList.Add(GenerateNewParticle());
            }

            for (int particle = 0; particle < myParticleList.Count; particle++)
            {
                myParticleList[particle].Update(dif);

                if (myParticleList[particle].timeToLive <= 0)
                {
                    myParticleList.RemoveAt(particle);
                    particle--;
                }
            }
            lastEmitterLocation = EmitterLocation;

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);

            for (int i = 0; i < myParticleList.Count; i++)
            {
                myParticleList[i].Draw(spriteBatch);
            }

            spriteBatch.End();
        }

    }
}
