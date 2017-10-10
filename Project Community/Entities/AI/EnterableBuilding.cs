using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Entities.World;
using Entities.Singletons;
namespace Entities.AI
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class EnterableBuilding : AIBase
    {
        
        private String otherWorld = null;
        private String originWorld = null;
        private Vector2 pos = Vector2.Zero;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld">Gameworld reference</param>
        /// <param name="e">Entity attached to.</param>
        public EnterableBuilding(GameWorld _gameWorld, Entity e):base(_gameWorld,e)
        {

        }

        /// <summary>
        /// Read in the world to switch to and the location to switch to if it is supplied.
        /// </summary>
        public override void init()
        {
            String[] strs = aiArgs.Split(',');
            otherWorld = strs[0];
            if(strs.Count() > 1)
            {
                pos.X = float.Parse(strs[1]);
                pos.Y = float.Parse(strs[2]);
            }
            originWorld = gameWorld.name;
        }

        /// <summary>
        /// Collide method.  If player, switches to the given world and sets the position if specified.
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="isTouching"></param>
        public override void collide(Entity otherThing, bool isTouching)
        {
            if (otherThing.isPlayer && isTouching)
            {
                EntityManager em = EntityManager.getEntityManager(gameWorld.game);
                em.setCurrentGameWorld(otherWorld);
                em.player.switchBody(otherWorld);
                if (pos != Vector2.Zero)
                    em.player.my_Body.Position = pos;    
            }
            else
                base.collide(otherThing, isTouching);
        }
    }
}
