using System;
using System.IO;
using System.Xml;
using Entities.Player;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.World;
using Entities.AI;
namespace Entities.Singletons
{
    /// <summary>
    /// Class for data loading entry point.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
   public  class GameLoader
    {
        private readonly Game game;
        private static GameLoader gameLoader;

       /// <summary>
       /// Constructor
       /// </summary>
       /// <param name="_gameWorld">Game refernce</param>
        private GameLoader(Game _game)
        {
            game = _game;
        }

       /// <summary>
       /// Get a gameloader reference.
       /// </summary>
       /// <param name="_game"></param>
       /// <returns>GameLoader</returns>
        public static GameLoader getGameLoader(Game _game)
        {
            if (gameLoader == null)
                gameLoader = new GameLoader(_game);
            return gameLoader;
        }

        /// <summary>
        /// Entry point for loading a game
        /// </summary>
        /// <param name="fileName"></param>
        public Vector2 loadGameFromFile(String fileName)
        {
            if (!File.Exists(fileName))
                throw new Exception("Main Game File Not Found!!");
            EntityManager entityManager = EntityManager.getEntityManager(game);

            XmlTextReader reader = new XmlTextReader(fileName);
            reader.ReadToFollowing("Name");
            String str = reader.ReadElementContentAsString();
            game.Window.Title = str;

            reader.ReadToFollowing("GameWindow");
            int vWidth = int.Parse(reader.GetAttribute("width"));
            int vHeight = int.Parse(reader.GetAttribute("height"));
            Vector2 viewPortSize = new Vector2(vWidth,vHeight);
            bool fullscreen = bool.Parse(reader.GetAttribute("fullscreen"));

            reader.ReadToFollowing("FontFile");
            String fontFileLoc = reader.ReadElementContentAsString();
            GUI.FontManager fontMan = GUI.FontManager.getFontManager(game.Content);
            fontMan.loadFonts(fontFileLoc);

            reader.ReadToFollowing("ItemFile");
            String itemFileLoc = reader.ReadElementContentAsString();

            reader.ReadToFollowing("EntityType");
            do
            {
                String typeName = reader.GetAttribute("name");
                entityManager.addType(typeName,new EntityType(reader.ReadElementContentAsString(), game));
            }
            while(reader.ReadToNextSibling("EntityType"));



            reader.ReadToFollowing("SubWorld");
            do
            {
                String worldName = reader.GetAttribute("name");
                String type = reader.GetAttribute("type");
                float width = float.Parse(reader.GetAttribute("width"));
                float height = float.Parse(reader.GetAttribute("height"));
                String worldFile = reader.ReadElementContentAsString();
                GameWorld newWorld = null;
                if (type == null || type.Equals("TopDown"))
                {
                    newWorld = new GameWorld(game, width, height, 64, worldName);
                }
                else if (type.Equals("PictureSideScroller"))
                {
                    newWorld = new PictureSideScrollGameWorld(game,width,height,64,worldName);
                }
                else if (type.Equals("ParallaxSideScroller"))
                {
                    newWorld = new ParallaxWorld(game, width, height, 64, worldName);
                }

                else if (type.Equals("Soccer"))
                {
                    newWorld = new SoccerGameWorld(game, width, height, 64, worldName);
                }
                newWorld.loadGameWorld(worldFile);
                newWorld.viewport.Width = vWidth;
                newWorld.viewport.Height = vHeight;
                // newWorld.addEntity(entityManager.player);
                entityManager.addGameWorld(newWorld, worldName);
            }
            while (reader.ReadToNextSibling("SubWorld"));
            entityManager.player.switchBody("Main");

            loadInventory(itemFileLoc);

            return viewPortSize;

           

        }

       /// <summary>
       /// Loads default inventory from file
       /// </summary>
       /// <param name="filename"></param>
        public void loadInventory(String filename)
        {
            XmlTextReader reader = new XmlTextReader(filename);
            while (reader.ReadToFollowing("Item"))
            {
                String name = reader.GetAttribute("name");
                String image = reader.GetAttribute("texture");
                int quantity = int.Parse(reader.GetAttribute("quantity"));
                EntityManager.inventoryImages.Add(name,game.Content.Load<Texture2D>(image));
                EntityManager.getEntityManager(game).player.inventory.AddItems(name,quantity);
            }
        }




    }

}
