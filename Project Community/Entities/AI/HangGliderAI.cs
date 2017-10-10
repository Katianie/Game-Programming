using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Entities.World;
using Microsoft.Xna.Framework;
using Entities.Singletons;

namespace Entities.AI
{
    /// <summary>
    /// HangGliderAI applies a downward impulse on the character
    /// </summary>
    /// <Owner>Eddie O'Hagan</Owner>
    public class HangGliderAI : AIBase
    {
        private EntityManager myEntityManager;

        public HangGliderAI(GameWorld gameWorld, Entity entity)
            : base(gameWorld, entity)
        {
            myEntityManager = EntityManager.getEntityManager(gameWorld.game);
        }

        public override void init()
        {
            base.init();

            myEntityManager.player.bodies[gameWorld.name].noGravity = true;
        }

        public override void update()
        {
            base.update();

            entity.my_Body.ApplyLinearImpulse(new Vector2(0.0f, 4.0f), entity.my_Body.GetWorldCenter());
        }
    }
}
