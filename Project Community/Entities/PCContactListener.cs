using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Box2D.XNA;
namespace Entities
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class PCContactListener: IContactListener
    {
        public void BeginContact(Contact contact)
        {
           // throw new NotImplementedException();
        }

        public void EndContact(Contact contact)
        {

        }

        public void PreSolve(Contact contact, ref Manifold oldManifold)
        {
           // throw new NotImplementedException();
        }

        public void PostSolve(Contact contact, ref ContactImpulse impulse)
        {

        }
    }
}
