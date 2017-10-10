using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
namespace Entities.World
{
    /// <summary>
    /// Gameworld that uses an image as a fixed background.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class PictureSideScrollGameWorld : GameWorld
    {
        public Texture2D bgTexture;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_game"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="tileWidth"></param>
        /// <param name="_name"></param>
        public PictureSideScrollGameWorld(Game _game, float width, float height, int tileWidth, String _name)
            :base( _game,  width,  height,  tileWidth, _name)
        {

        }

        /// <summary>
        /// Draws the image instead of a tiled layer
        /// </summary>
        /// <param name="sb"></param>
        public override void draw(SpriteBatch sb)
        {
            entityManager.handleEvents(entities.ToArray());
            entityManager.animateEntities(entities.ToArray());
            sb.Begin();
            sb.Draw(bgTexture, Vector2.Zero, null, Color.White);
            if (navDebug)
            {
                navStuff.DebugDraw(sb);
            }
            entityManager.drawEntities(sb, entities.ToArray());
            entityManager.player.draw(sb);
            sb.End();
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="gameTime"></param>
        public override void update(GameTime gameTime)
        {
            base.update(gameTime);
        }

        /// <summary>
        /// Loads the picture gameworld
        /// </summary>
        /// <param name="filename"></param>
        public override void loadGameWorld(string filename)
        {
            //Separate into base and header call at some point

            fileLoadedFrom = filename;
            XmlTextReader reader = new XmlTextReader(filename);
            reader.ReadToFollowing("BackGround");
            String bgImage = reader.ReadElementContentAsString();
            bgTexture = game.Content.Load<Texture2D>(bgImage);
            reader.ReadToFollowing("Gravity");
            String[] grav = reader.ReadElementContentAsString().Split(',');
            Vector2 _gravity;
            _gravity.X = float.Parse(grav[0]);
            _gravity.Y = float.Parse(grav[1]);
            gravity = _gravity;
            physicsWorld.Gravity = gravity;

            Event[] events = null;
            if (reader.ReadToFollowing("Edges"))
            {
                events = readEdgeEvents(reader.ReadSubtree());
            }
            reader.ReadToFollowing("CheckPoints");
            if (reader.ReadToDescendant("CheckPoint"))
            {
                do
                {
                    float xl = float.Parse(reader.GetAttribute("xLoc"));
                    float yl = float.Parse(reader.GetAttribute("yLoc"));
                    checkpoints.Add(new Vector2(xl, yl));
                }
                while (reader.ReadToNextSibling("CheckPoint"));
            }

            if (reader.ReadToFollowing("Entity"))
            {
                do
                {
                    if (reader.AttributeCount == 1)
                    {
                        loadNonEntity(reader);
                        continue;
                    }
                    loadEntity(reader);
                }
                while (reader.ReadToNextSibling("Entity"));
            }


            if (reader.ReadToFollowing("Joints")&&reader.ReadToFollowing("Joint"))
            {
    
                do
                {
                    loadJoint(reader);
                } while (reader.ReadToNextSibling("Joint"));
            }


            if (reader.ReadToFollowing("NoPlayerCollision"))
            {
                String[] strs = reader.ReadElementContentAsString().Split(',');
                foreach (String s in strs)
                {
                    idsOfThingsThatDontCollideWithThePlayer.Add(s);
                }
            }

            addBounds(events);
        }

    }
}
