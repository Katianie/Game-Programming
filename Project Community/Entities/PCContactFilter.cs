using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
using Entities.World;
namespace Entities
{
    /// <summary>
    /// Contact filter for box2d world,
    /// calls the should collide method of both entites if either is false there is no collision
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class PCContactFilter : IContactFilter
    {
        public bool  ShouldCollide(Fixture fixtureA, Fixture fixtureB)
        {
            Entity ent1 = fixtureA.GetBody().GetUserData() as Entity;
            Entity ent2 = fixtureB.GetBody().GetUserData() as Entity;
            if (ent1 == null || ent2 == null)
                return true;
            return (ent1.shouldCollide(ent2) && ent2.shouldCollide(ent1));
        }
}
}
