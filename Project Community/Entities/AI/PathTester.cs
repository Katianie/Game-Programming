using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Entities.World;
using Entities.Singletons;
namespace Entities.AI
{
    //Debugging AI
    public class PathTester : AIBase
    {
       // PolygonNode[] path;
        List<PolygonNode> path;
        int i;
        Point point1, point2;
        static Random rand = new Random(DateTime.Now.Millisecond);
        Texture2D tex;
        Vector2 vel = Vector2.Zero;
        float angle = 0f;
        bool pause = true;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_gameworld"></param>
        /// <param name="e"></param>
        public PathTester(GameWorld _gameworld, Entity e):base(_gameworld, e)
        {
            gameWorld = _gameworld;
            point2 = new Point(250,1200);
            tex = gameWorld.getGame().Content.Load<Texture2D>("Black");

            i =1;
        }

        public override void update()
        {
            gameWorld = EntityManager.getEntityManager(gameWorld.game).getCurrentGameWorld();
            point1 = new Point(100, 100);
#if DEBUG
            if(Keyboard.GetState().IsKeyDown(Keys.P))
                path = gameWorld.navStuff.getPath(new Point((int)entity.my_Body.Position.X * 64, (int)entity.my_Body.Position.Y * 64), point1).ToList();
            #region Random Movement
            if (stopUpdating)
            {
                pause = true;
                
                vel = Vector2.Zero;
                entity.my_Body.SetLinearVelocity(vel);
                return;
            }
            if (pause)
            {
                if (rand.Next() % 300 == 0)
                {
                    angle = rand.Next(-1800, 1800) / 10.0f;
                    angle *= (180/3.14f);
                    vel.X = (float)(3.0 * Math.Cos(angle));
                    vel.Y = (float)(3.0 * Math.Sin(angle));
                    pause = false;
                }
            }
            else
            {
                if (rand.Next() % 150 == 0)
                {
                    vel = Vector2.Zero;
                    pause = true;
                }
            }
            entity.my_Body.SetLinearVelocity(vel);
            #endregion


#endif
            //Vector2 newVel = Vector2.Zero;
            //Vector2 pos = entity.my_Body.Position;
            //if (first)
            //{
            //    goal = path.ElementAt<PolygonNode>(0);
            //    aimFor = goal.rectangle.Center;
            //    if (pos.X - aimFor.X / 64 < .1)
            //        newVel.X = 3;
            //    if (pos.X - aimFor.X / 64 > -.1)
            //        newVel.X = -3;
            //    if (pos.Y - aimFor.Y / 64 < .1)
            //        newVel.Y = 3;
            //    if (pos.Y - aimFor.Y / 64 > -.1)
            //        newVel.Y = -3;

            //  Vector2 normalized =  new Vector2(Math.Abs(pos.X - aimFor.X), Math.Abs(pos.Y - aimFor.Y));
            //  normalized.Normalize();
            //  newVel *= normalized;


            //    if ((Math.Abs(pos.X - (aimFor.X) / 64) < .2) && (Math.Abs(pos.Y - (aimFor.Y / 64)) < .2))
            //    {
            //        first = false;
            //        //i++;
            //        if (i < path.Count)
            //        {
            //            current = goal;
            //            goal = path.ElementAt<PolygonNode>(i);
            //            aimFor = goal.rectangle.Center;
            //        }
            //        i++;
                   
            //    }
            //}
            //else
            //{

            //    if (goal != null)
            //    {
            //        if (pos.X - aimFor.X / 64 < .1)
            //            newVel.X = 5;
            //        if (pos.X - aimFor.X / 64 > -.1)
            //            newVel.X = -5;
            //        if (pos.Y - aimFor.Y / 64 < .1)
            //            newVel.Y = 5;
            //        if (pos.Y - aimFor.Y / 64 > -.1)
            //            newVel.Y = -5;

            //        if (goal.inPolygon(entity.my_Body.Position * 64))
            //        {
            //            current = goal;
            //            goal = null;
            //        }
            //        Vector2 normalized = new Vector2(Math.Abs(pos.X - aimFor.X), Math.Abs(pos.Y - aimFor.Y));
            //        normalized.Normalize();
            //        newVel *= normalized;
            //    }
            //    else if (path.Count > i)
            //    {
            //        goal = path.ElementAt<PolygonNode>(i);
            //        aimFor = goal.rectangle.Center;
            //        i++;
            //    }
            //    else
            //    {
            //        Point temp;
            //        try
            //        {
            //            path = gameWorld.navStuff.getPath(new Point((int)entity.my_Body.Position.X * 64,
            //                (int)entity.my_Body.Position.Y * 64), point2).ToList();
            //            gameWorld.entityManager.drawString(point2.ToString());
            //        }
            //        catch (Exception e) { gameWorld.entityManager.drawString(e.Message); }
            //        /*temp = point2;
            //        point2 = point1;
            //        point1 = temp;*/
            //        point2 = new Point(rand.Next() % 2000, rand.Next() % 2000);
            //        i = 1;
            //    }
            //}
            //entity.my_Body.SetLinearVelocity(newVel);
        }

        public void drawPath(SpriteBatch sb)
        {
            if (path == null)
                return;
            foreach (PolygonNode pn in path)
            {
                Rectangle r = new Rectangle(pn.rectangle.X, pn.rectangle.Y, pn.rectangle.Width, pn.rectangle.Height);
                r.X -= gameWorld.viewport.X;
                r.Y -= gameWorld.viewport.Y;
                sb.Draw(tex, r, Color.AntiqueWhite);
            }

        }


        public override void collide(Entity otherThing, bool isTouching)
        {
            if (isTouching)
            {
                    angle = rand.Next(-1800, 1800) / 10.0f;
                    vel.X = (float)(3.0*Math.Cos(angle));
                    vel.Y = (float)(3.0 * Math.Sin(angle));
                    pause = false;
            }
            //base.collide(otherThing, isTouching);
        }

 
    }
}
