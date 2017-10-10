using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Box2D.XNA;
using Entities.Player;
using Entities.World;
using Particle_Engine;
namespace Entities.Singletons
{
    /// <summary>
    /// Manages the game worlds, types and entities.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class EntityManager
    {
        private static EntityManager entityManager;
        private readonly Game game;
        public Player.Player player { get; set; }
        private readonly Dictionary<string, EntityType> types;
        private GameWorld currentWorld;
        public Dictionary<String, GameWorld> worlds;
        public List<string> stringStack;
        public SpriteFont debugFont;
        public ParticleEngine particleEngine;
        public static Body NextGuy = null;
        public static Dictionary<String, Texture2D> inventoryImages = new Dictionary<String,Texture2D>();
        private List<Tuple<String,Color>> floaters;
        private int waitToFloat = 0;
        private static String saveFile = "PCSaveFile.pcg";
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        private EntityManager(Game _game)
        {
            stringStack = new List<string>();
            game = _game;
            types = new Dictionary<string, EntityType>();
            worlds = new Dictionary<string, GameWorld>();
            floaters = new List<Tuple<String, Color>>();
        }

        /// <summary>
        /// Get a reference to the entity manager (Singleton)
        /// </summary>
        /// <param name="game"></param>
        /// <returns></returns>
        public static EntityManager getEntityManager(Game game)
        {
            if (entityManager == null)
                entityManager = new EntityManager(game);
            return entityManager;
        }

        /// <summary>
        /// Draw entities
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        /// <param name="entities">Array of entities</param>
        public void drawEntities(SpriteBatch sb, Entity[] entities)

        {
#if DEBUG
            if (debugFont == null)
                debugFont = game.Content.Load<SpriteFont>("defaultFont");
            int i = 0;
            foreach (String s in stringStack)
            {
                i++;
                sb.DrawString(debugFont,
                    s, new Vector2(35, 125 + i*24), Color.Aqua);
            }

#endif
            foreach (Entity entity in entities)
            {
                entity.draw(sb);
            }
            player.draw(sb);
        }


        /// <summary>
        /// Calls event handling for each entity
        /// </summary>
        /// <param name="entities">Array of entities</param>
        public void handleEvents(Entity[] entities)
        {
            player.handleEvents();
            foreach (Entity e in entities)
            {
                e.handleEvents();
            }

        }

        /// <summary>
        /// calls the animate method of each entity
        /// </summary>
        /// <param name="entities">Array of entities</param>
        public void animateEntities(Entity[] entities)
        {
            player.animate();
            foreach (Entity e in entities)
            {
                e.animate();
            }
        }

        /// <summary>
        /// Adds an entity type
        /// </summary>
        /// <param name="typename">Name of type</param>
        /// <param name="type">Type</param>
        public void addType(String typename, EntityType type)
        {
            types.Add(typename, type);
        }

        /// <summary>
        /// Set universal player reference
        /// </summary>
        /// <param name="entity">Player entity</param>
        public void addPlayer(Player.Player entity)
        {
            player = entity;
        }

        /// <summary>
        /// Removes a gameworld.
        /// </summary>
        /// <param name="name"></param>
        public void deleteGameWorld(String name)
        {
            if (currentWorld == worlds[name])
                throw new Exception("You can delete the current gameworld.");
            worlds.Remove(name);
        }


        /// <summary>
        /// Reloads the game world.  Cannot be the active game world.
        /// </summary>
        /// <param name="name"></param>
        public void reloadGameWorld(String name)
        {
            if (name == "Main")
                return;
            if (currentWorld == worlds[name])
                throw new Exception("You can delete the current gameworld.");

            GameWorld w = worlds[name];
            w.deleteAllEntities();
            player.removeAPlayer(name);
            w.loadEntitesFromFile();
        }
        /// <summary>
        /// Get entity type from name.
        /// </summary>
        /// <param name="name">name of type</param>
        /// <returns>EntityType</returns>
        public EntityType getType(String name)
        {
            EntityType entType;
            types.TryGetValue(name, out entType);
            return entType;
        }

        /// <summary>
        /// Adds a debugging string.
        /// </summary>
        /// <param name="text">String to add</param>
        public void drawString(string text)
        {
            stringStack.Add(text);
        }

        /// <summary>
        /// Clears the debugging strings.
        /// </summary>
        public void clearStringList()
        {
            stringStack.Clear();
        }

        /// <summary>
        /// Recursively calls collision methods on both entities in each collision.
        /// </summary>
        /// <param name="contact">Contact to be processed</param>
        public void processContacts(Contact contact)
        {
            if (contact == null)
                return;
            do
            {
                Entity ent1 = contact.GetFixtureA().GetBody().GetUserData() as Entity;
                Entity ent2 = contact.GetFixtureB().GetBody().GetUserData() as Entity;
                if (ent1 == null || ent2 == null)
                    return;
                ent1.collide(ent2, contact.IsTouching());
                ent2.collide(ent1, contact.IsTouching());
                if(ent1.ai != null)
                    ent1.ai.collide(ent2,contact.IsTouching());
                if (ent2.ai != null)
                    ent2.ai.collide(ent1, contact.IsTouching());
                
                contact = contact.GetNext();
            } while (contact != null);

            
        }

        /// <summary>
        /// Calls the update method of each ai.
        /// </summary>
        /// <param name="entities">Array of entities</param>
        public void updateAIs(Entity[] entities)
        {
            if(player.ai != null)
                player.ai.update();
            foreach (Entity e in entities)
            {
                if(e.ai != null)
                   e.ai.update();
            }
        }

        /// <summary>
        /// Draws current world.
        /// </summary>
        /// <param name="sb"></param>
        public void drawCurrentWorld(SpriteBatch sb)
        {
            currentWorld.draw(sb);
            if (currentWorld.name == "Main")
                drawHalo(sb);
            if (waitToFloat < 0 && floaters.Count > 0)
            {
                GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText(floaters[0].Item1, new Vector2(370, 300), floaters[0].Item2);
                floaters.RemoveAt(0);
                waitToFloat = 20;
            }
            else
            {
                waitToFloat--;
            }
        }

        /// <summary>
        /// Sets the current game world.
        /// </summary>
        /// <param name="key">World name to switch to</param>
        public void setCurrentGameWorld(String key)
        {
            currentWorld = worlds[key];
            clearStringList();
            InputManager.speed = 3;
            floaters.Clear();
        }

        /// <summary>
        /// Return current gameworld
        /// </summary>
        /// <returns>Return current gameworld</returns>
        public GameWorld getCurrentGameWorld() { return currentWorld; }

        /// <summary>
        /// Adds a gameworld to the list of worlds
        /// </summary>
        /// <param name="_world">GameWorld</param>
        /// <param name="_name">Name of world (key)</param>
        public void addGameWorld(GameWorld _world, String _name)
        {
            worlds.Add(_name, _world);
            if (currentWorld == null)
                currentWorld = _world;
        }

        /// <summary>
        /// Returns true if string is the name of a gameworld
        /// </summary>
        /// <param name="s">WorldName</param>
        /// <returns>Returns true if string is the name of a gameworld</returns>
        public bool isWorld(String s)
        {
            return worlds.ContainsKey(s);
        }


        /// <summary>
        /// Draws the indicator over the target NPC's head.
        /// </summary>
        /// <param name="sb"></param>
        public void drawHalo(SpriteBatch sb)
        {
            if (particleEngine == null)
            {
                List<Texture2D> textures = new List<Texture2D>();
                textures.Add(game.Content.Load<Texture2D>("particle"));
                particleEngine = new ParticleEngine(textures, new Vector2(400, 240));
                NextGuy = worlds["Main"].bodyDict["Doctor"];
            }
            if (NextGuy != null)
            {
                Vector2 pos = NextGuy.Position * 64;
                pos.X -= currentWorld.viewport.X;
                pos.X += 32;
                pos.Y -= currentWorld.viewport.Y;
                pos.Y -= 5;
                particleEngine.EmitterLocation = pos;
                particleEngine.Update();
                particleEngine.Draw(sb);
            }       
        }


        /// <summary>
        /// Add a floating text to a queue.
        /// </summary>
        /// <param name="floatingText"></param>
        /// <param name="floatingColor"></param>
        public void pushAFloater(String floatingText, Color floatingColor)
        {
            var tuple = new Tuple<string, Color>(floatingText,floatingColor);
            floaters.Add(tuple);
        }


        public void loadGame()
        {
            GameWorld gw = entityManager.worlds["Main"];
            XmlTextReader reader = new XmlTextReader(saveFile);
            reader.ReadToFollowing("Characters");
            reader.ReadToDescendant("Character");
            do
            {
                String id = reader.GetAttribute("id");
                String convo = reader.GetAttribute("convoName");
                gw.aiDict[id].conversationName = convo;
                gw.aiDict[id].resetConvo();

            } while (reader.ReadToNextSibling("Character"));


            reader.ReadToFollowing("Inventory");
            reader.ReadToDescendant("Item");
            Inventory inventory = new Inventory();
            do
            {
                String name = reader.GetAttribute("name");
                String amount = reader.GetAttribute("amount");
                inventory.AddItems(name, int.Parse(amount));

            } while (reader.ReadToNextSibling("Item"));
            player.inventory = inventory;
        }




        public void saveGame()
        {
            GameWorld gw = entityManager.worlds["Main"];
            XmlDocument xdoc = new XmlDocument();
            XmlElement xmlElement = xdoc.CreateElement("SaveFile");
            xdoc.AppendChild(xmlElement);
            XmlElement charElement = xdoc.CreateElement("Characters");


            foreach (Entity e in gw.getEntities())
            {
                if (e.ai != null && e.id != null && e.id != "NoId")
                {
                    xmlElement = xdoc.CreateElement("Character");
                    xmlElement.SetAttribute("id", e.id.ToString());
                    xmlElement.SetAttribute("convoName", e.ai.conversationName);
                    charElement.AppendChild(xmlElement);
                }

            }
            xdoc.DocumentElement.AppendChild(charElement);

            XmlElement inventory = xdoc.CreateElement("Inventory");
            foreach (String key in player.inventory.Keys)
            {
                XmlElement item = xdoc.CreateElement("Item");
                item.SetAttribute("name", key);
                item.SetAttribute("amount", player.inventory[key].ToString());
                inventory.AppendChild(item);
            }
            xdoc.DocumentElement.AppendChild(inventory);

            StreamWriter writer = new StreamWriter(saveFile);
            writer.Write(xdoc.OuterXml);
            writer.Close();

        }


    }
}
