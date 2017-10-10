using Microsoft.Xna.Framework;

namespace Entities.World
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    class Projectile:Entity
    {
        private readonly int lifeTime;
        private readonly Event passOnCollision;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw">Gameworld reference</param>
        /// <param name="et">EntityType</param>
        /// <param name="_lifeTime">Lifetime. (Not Implemented)</param>
        /// <param name="_passOnCollision">Event to pass on collision.</param>
        public Projectile(GameWorld gw, EntityType et, int _lifeTime, Event _passOnCollision):base(gw,et)
        {
            passOnCollision = _passOnCollision;
            lifeTime = _lifeTime;
        }

        public override void handleEvents()
        {
            if (!(game.GraphicsDevice.Viewport.Bounds.Intersects(new Rectangle((int)my_Body.Position.X, (int)my_Body.Position.Y, (int)type.size.X, (int)type.size.Y))))
            {
                gameWorld.removeEntity(this);
            }
            //base.handleEvents();
        }

        /// <summary>
        /// Collision Method
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            if (touching)
            {
                otherThing.addEvent(passOnCollision);
                gameWorld.removeEntity(this);
            }
            //base.collide(otherThing, touching);
        }
    }
}
