using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class ToBeVaccinated:StaticPacer
    {
        public bool vaccinated = false;
        public ToBeVaccinated(GameWorld gameworld, Entity ent):base(gameworld,ent)
        {

        }


        public override void collide(Entity otherThing, bool isTouching)
        {
            if (otherThing.type.type == TypeOfThing.PROJECTILE && isTouching)
            {

               
            }
            base.collide(otherThing, isTouching);
        }


    }
}
