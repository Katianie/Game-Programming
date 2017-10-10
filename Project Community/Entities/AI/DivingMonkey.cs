using System;
using Box2D.XNA;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// Randomly rising and falling monkey enemy ai.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class DivingMonkey :AIBase
    {
        public bool diving = false;
        bool retracting = false;
        bool first = true;
        public int diveIn =100;
        private DistanceJoint toAnchor;
        //Should be read from aiArgs
        private String anchorName = "DivingAnchor";
        private float OGLength;
        
        private int pauseTime;


        public DivingMonkey(GameWorld _gameWorld, Entity e) :base(_gameWorld,e)
        {
        }

        public override void update()
        {

            if (first)
            {
                Body b = gameWorld.bodyDict[anchorName];
                toAnchor = gameWorld.joinBodies_Distance(entity.my_Body, b);
                OGLength = toAnchor.GetLength();
                first = false;
                diving = true;
                retracting = true;
                return;
            }

            float length = toAnchor.GetLength();

            if (!diving && diveIn <= 0)
            {
                diving = true;
            }
            else if (diving)
            {
                if (retracting)
                {
                    length -= .05f;
                    toAnchor.SetLength(length);
                    if (length < 2)
                    {
                        diving = false;
                        retracting = false;
                        diveIn = random.Next(50,250);
                        pauseTime = random.Next(50, 150);
                    }
                }
                else
                {
                    if (length >= OGLength)
                    {
                        if (pauseTime <= 0)
                            retracting = true;
                        else
                            pauseTime--;
                    }
                    else
                    {
                        length += .05f;
                        toAnchor.SetLength(length);
                    }

                }  
            }
            else
            {
                diveIn--;
            }
        }
    }
}















