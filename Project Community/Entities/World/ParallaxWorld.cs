using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
namespace Entities.World
{
    class ParallaxWorld: PictureSideScrollGameWorld
    {
        public ParallaxWorld(Game _game, float width, float height, int tileWidth, String _name)
            :base( _game,  width,  height,  tileWidth, _name)
        {

        }

        public override void  draw(Microsoft.Xna.Framework.Graphics.SpriteBatch sb)
        {
            entityManager.handleEvents(entities.ToArray());
            entityManager.animateEntities(entities.ToArray());
            sb.Begin();
            sb.Draw(bgTexture, calculateShift(), null, Color.White);
            if (navDebug)
            {
                navStuff.DebugDraw(sb);
            }
            entityManager.drawEntities(sb, entities.ToArray());
            entityManager.player.draw(sb);
            sb.End();
        }

        private Vector2 calculateShift()
        {
            Vector2 shift = Vector2.Zero;


            if (bgTexture.Width > viewport.Width)
            {
                float percentIn = viewport.X / (sizeInPixels.X);
                shift.X = -(bgTexture.Width - viewport.Width) * percentIn;
                if (shift.X < -(bgTexture.Width - viewport.Width))
                    shift.X = -(bgTexture.Width - viewport.Width);
            }
            else
                shift.X = 0;

            if (bgTexture.Height > viewport.Height)
            {
                float percentIn = viewport.Y / (sizeInPixels.Y);
                shift.Y = -(bgTexture.Height - viewport.Height) * percentIn;
                if (shift.Y < -(bgTexture.Height - viewport.Height))
                    shift.Y = -(bgTexture.Height - viewport.Height);
            }
            else
            {
                shift.Y = 0;
            }

            return shift;
        }
    }
}
