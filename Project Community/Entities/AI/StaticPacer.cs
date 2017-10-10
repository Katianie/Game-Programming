using System;
using Microsoft.Xna.Framework;
using Entities.World;
namespace Entities.AI
{
    public class StaticPacer : AIBase
    {
        const float maxDistance = 4.0f;

        Vector2 start;
        PolygonNode[] path;
        Vector2 newVelocity;
        Vector2 stuff;
        float angle;
        double radianRot;
        bool pause;
        int[] arry;
        int[] rad;
        int count; 

        public StaticPacer(GameWorld gameworld, Entity ent)
            : base(gameworld, ent)
        {

            angle = 0f;
            count = 1;
            radianRot = 0.0;
            pause = true;
            //newVelocity = new Vector2();
            newVelocity = Vector2.Zero;
            stuff = new Vector2();
            path = new PolygonNode[30];
            start = entity.my_Body.Position ;
            arry = new int[360];
            rad = new int[] { 2, 3, 4};
        }

        public override void update()
        {
            //This will keep them from walking away while in the midst of a conversation.
            if (stopUpdating)
            {
                pause = true;
                entity.my_Body.SetLinearVelocity(Vector2.Zero);
                return;
            }
                movePlayerToPoint();
        }

        public void movePlayerToPoint()
        {
            //int result = 0;

            //result = lookForPath(pacer);
            

            //if (result == -1)
            //{
            //    movePlayerToPoint();
            //}

            //else
            //{

            /*
            if (count > 0)
            {

                stuff = moveDirectionAlongVector();

                stuff.Normalize();

                newVelocity = stuff * 2;

                pacer.my_Body.SetLinearVelocity(newVelocity);

                count --;

            }

            else
            {
                stuff = Vector2.Zero;
                pacer.my_Body.SetLinearVelocity(newVelocity);
                count = 1;
            }


           // Thread.Sleep(1000);
            //}*/

            //My attempt at a constraint
            float f;
            Vector2 now = entity.my_Body.Position;
            Vector2.Distance(ref start, ref now, out f);
            if (f >= maxDistance)
            {
                pause = false;
               // angle = (Vector2.Dot(Vector2.Normalize(now), Vector2.Normalize(start)));
                //newVelocity.X = (float)(6 * Math.Cos(angle));
                //newVelocity.Y = (float)(6 * Math.Sin(angle));
                newVelocity = start - now;
                newVelocity.Normalize();
                newVelocity *= 3;

            }
            //End attempt

            double radius = rad[random.Next(0, rad.Length)];//This does not constrain them to a given area.
            if (pause)
            {
                if (random.Next() % 300 == 0 || newVelocity == Vector2.Zero)
                {
                    angle = random.Next(-1800, 1800) / 10.0f;
                    angle *= (180 / 3.14f);
                    
                    //newVelocity.X = (float)(radius * Math.Cos(angle));
                    //newVelocity.Y = (float)(radius * Math.Sin(angle));
                    //newVelocity.Normalize();//This is the same as always using radius of 1

                    //I just added this part which uses a random speed.
                    int speed = random.Next(0, 3);
                    newVelocity.X = (float)(speed * Math.Cos(angle));
                    newVelocity.Y = (float)(speed * Math.Sin(angle));

                    pause = false;
                }
            }
            else
            {
                if (random.Next() % 150 == 0)
                {
                    newVelocity = Vector2.Zero;
                    pause = true;
                }
            }
            entity.my_Body.SetLinearVelocity(newVelocity);


        }

        //public int randomDegree()
        //{

        //    int random = 0;

        //    for (int i = 0; i < arry.Length; i++)
        //    {
        //        arry[i] = i;
        //    }

        //    random = arry[rand.Next(0, 360)];

        //    return random;
            

        //}

        //public Point moveDirection()
        //{

        //    Point move;

        //    double radianRot = MathHelper.ToRadians(randomDegree());


        //    double radius = rad[rand.Next(0, 3)];

        //    move = new Point();

        //    move.X = (int)(radius * (Math.Cos(radianRot)));
        //    move.Y = (int)(radius * (Math.Sin(radianRot)));

            

        //    return move;

        //}

        //public Vector2 moveDirectionAlongVector()
        //{

        //    Vector2 move;

        //    if (!pause)
        //    {
        //        radianRot = MathHelper.ToRadians(randomDegree());

        //    }


        //    relativeDirection(radianRot);

        //    double radius = rad[rand.Next(0, 12)];

        //    move = new Vector2();

        //    move.X = (int)(radius * (Math.Cos(radianRot)));
        //    move.Y = (int)(radius * (Math.Sin(radianRot)));



        //    return move;

        //}


        //public double relativeDirection(double direction)
        //{

        //    double random = direction;
        //    double random2 = direction;
        //    int selection = 0;


        //    random = rand.Next((int)direction, (int)direction + (int)MathHelper.ToRadians(3));
        //    random2 = rand.Next((int)direction, (int)direction - (int)MathHelper.ToRadians(3));


        //    selection = rand.Next((int)random2, (int)random);

        //    return selection;


        //}


        //public int lookForPath(Entity thing)
        //{


        //    int result = 0;

        //    Point searchPoint = new Point();

        //    Point endPoint = new Point();

        //    endPoint = moveDirection();

        //    Point startPoint = new Point();

        //    startPoint.X = (int)thing.my_Body.Position.X;
        //    startPoint.Y = (int)thing.my_Body.Position.Y;

        //    PolygonNode check;


        //    try
        //    {



        //        path = gameWorld.navStuff.getPath
        //             (startPoint, endPoint);

        //    }

        //    catch (Exception e)
        //    {
        //        lookForPath(thing);
        //    }

        //   for (int i = 0; i < path.Length; i++)
        //   {
        //       searchPoint.X = path[i].rectangle.X;
        //       searchPoint.Y = path[i].rectangle.Y;
        //       check = gameWorld.navStuff.getNodeFromPosition(searchPoint);

        //       if (check == null)
        //       {
        //           result = -1;
        //       }

        //       else
        //       {
        //           result = 1;
        //       }
        //   }

        //   return result;
        //}


        public override void collide(Entity otherThing, bool isTouching)
        {
            pause = true;
        }

    }
}
