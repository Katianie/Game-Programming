using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using Box2D.XNA;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Entities.AI;
using Entities.Singletons;
using Entities.Player;
namespace Entities.World
{
    /// <summary>
    /// Represents a single place in the game where, a world if you will.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class GameWorld
    {
        public bool navDebug = false;
        public Vector2 gravity{get;set;}
        public readonly Vector2 sizeInPixels;
        public List<Entity> entities;
        public Dictionary<String, Body> bodyDict;
        public Dictionary<String, AIBase> aiDict;
        public Viewport viewport;
        public readonly Game game;
        private TiledLayer tiledLayer;
        protected EntityManager entityManager;
        public Box2D.XNA.World physicsWorld;
        public NavMeshManager navStuff;
        public readonly String name;
        public List<String> idsOfThingsThatDontCollideWithThePlayer;
        public String fileLoadedFrom;
        public bool entered { get; set; }
        private readonly bool[] walls;
        public List<Vector2> checkpoints;
        public Vector2 lastCP;
        public Cannon cannon;
        public PowerBar bar;
        public SpriteBatch mySpriteBatch;
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_game"></param>
        public GameWorld(Game _game, float width, float height, int tileWidth, String _name)
        {
            sizeInPixels = new Vector2(width*tileWidth,height*tileWidth);
            game = _game;
            entityManager = EntityManager.getEntityManager(game);
            entities = new List<Entity>();
            physicsWorld = new Box2D.XNA.World(new Vector2(0,0f), true);
            physicsWorld.ContactFilter = new PCContactFilter();
            physicsWorld.ContactListener = new PCContactListener();
            navStuff = new NavMeshManager((int)sizeInPixels.X, (int)sizeInPixels.Y, game);
            viewport = new Viewport(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            name = _name;
            bodyDict = new Dictionary<string, Body>();
            idsOfThingsThatDontCollideWithThePlayer = new List<string>();
            entered = false;
            walls = new bool[4];
            checkpoints = new List<Vector2>();
            aiDict = new Dictionary<string, AIBase>();
        }

        /// <summary>
        /// Gets the main game object
        /// </summary>
        /// <returns>the game</returns>
        public Game getGame()
        {
            return game;
        }

        /// <summary>
        /// Returns a list of all entities in this gameworld.
        /// </summary>
        /// <returns></returns>
        public List<Entity> getEntities()
        {
            return entities;
        }
        /// <summary>
        /// Update the gameworld
        /// </summary>
        /// <param name="gameTime">GameTime</param>
        public virtual void update(GameTime gameTime)
        {
            Vector2 t_CP = getCheckPoint(entityManager.player.my_Body.Position);
            if(t_CP.X > lastCP.X)
            {
                lastCP = t_CP;
                entityManager.player.copyInventory();
                EntityManager.getEntityManager(game).pushAFloater("Checkpoint", Color.DarkBlue);
            }
            physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds, 15, 6);
            Box2D.XNA.Contact contact = physicsWorld.GetContactList();
            if (contact != null)
                entityManager.processContacts(contact);
            entityManager.updateAIs(entities.ToArray());
        }

        /// <summary>
        /// Draw the game world
        /// </summary>
        /// <param name="sb">Sprite Batch to draw with</param>
        public virtual void draw(SpriteBatch sb)
        {
            mySpriteBatch = sb;
            entityManager.handleEvents(entities.ToArray());
            entityManager.animateEntities(entities.ToArray());
            sb.Begin();
            tiledLayer.draw(sb);
            if(navDebug)
            {
                navStuff.DebugDraw(sb);
            }
            entityManager.drawEntities(sb,entities.ToArray());

            sb.End();
        }

        /// <summary>
        /// Resets the game world.
        /// Removes and reloads all entities.
        /// </summary>
        public void resetWorld()
        {
            if (fileLoadedFrom == null)
                return;
            entityManager.player.deleteBody(name);
            physicsWorld = new Box2D.XNA.World(new Vector2(0, 0f), true);
            physicsWorld.ContactFilter = new PCContactFilter();
            navStuff = new NavMeshManager((int)sizeInPixels.X, (int)sizeInPixels.Y, game);
            viewport = new Viewport(0, 0, game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            entities = new List<Entity>();
            bodyDict = new Dictionary<string, Body>();
            checkpoints = new List<Vector2>();
            idsOfThingsThatDontCollideWithThePlayer = new List<string>();
            loadGameWorld(fileLoadedFrom);
            entityManager.player.enteringWorld = 24;
        }

        #region Add and Remove

        /// <summary>
        /// Add physical body to this world
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="ent">Entity of body</param>
        /// <returns></returns>
        public Body AddBody(Vector2 position, Entity ent)
        {
            List<Shape> shapes = ent.type.getShapes();     
            BodyDef bd = new BodyDef();            
            bd.type = ent.type.getBodyType();
            bd.position = position;


            Body body = physicsWorld.CreateBody(bd);
            foreach (Shape s in shapes)
            {
                FixtureDef fd = new FixtureDef
                                    {
                                        shape = s,
                                        restitution = ent.type.restitution,
                                        friction = ent.type.friction,
                                        density = ent.type.density
                                    };
                body.CreateFixture(fd);
            }
            body.SetUserData(ent);
            body.SetFixedRotation(ent.type.fixedRotation);
            return body;
           
        }


        /// <summary>
        /// Add physical body to this world
        /// </summary>
        /// <param name="position">Starting position</param>
        /// <param name="ent">Entity of body</param>
        /// <returns></returns>
        public Body AddBody(Vector2 position, EntityType type, Entity ent)
        {
            List<Shape> shapes = type.getShapes();
            BodyDef bd = new BodyDef {type = type.getBodyType(), position = position};

            Body body = physicsWorld.CreateBody(bd);
            foreach (Shape s in shapes)
            {
                FixtureDef fd = new FixtureDef{shape = s, restitution = type.restitution, friction = type.friction, density = type.density};
                body.CreateFixture(fd);
            }
            body.SetUserData(ent);
            body.SetFixedRotation(type.fixedRotation);
            return body;
        }

        /// <summary>
        /// add entity to this world
        /// </summary>
        /// <param name="entity"></param>
        public void addEntity(Entity entity, String id)
        {
            entities.Add(entity);
            if (id != null && id != "NoId")
            {
                bodyDict.Add(id, entity.my_Body);
            }
        }

        /// <summary>
        /// Removes the entity from the game world and destroys its body
        /// </summary>
        /// <param name="e"></param>
        public void removeEntity(Entity e)
        {
            physicsWorld.DestroyBody(e.my_Body);
            entities.Remove(e);
        }

        #endregion

        #region JoinBodies

        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns></returns>
        public DistanceJoint joinBodies_Distance(Body b1, Body b2) 
        {
            Vector2 pos1 = b1.GetWorldCenter();
            Vector2 pos2 = b2.GetWorldCenter();
            DistanceJointDef djd = new DistanceJointDef();

            djd.Initialize(b1,b2,pos1,pos2);
            djd.collideConnected = false;
            DistanceJoint dj = physicsWorld.CreateJoint(djd) as DistanceJoint;
            return dj;
        }


        public MaxDistanceJoint joinBodies_Rope(Body b1, Body b2, float length)
        {
            Vector2 pos1 = b1.GetWorldCenter();
            Vector2 pos2 = b2.GetWorldCenter();
            MaxDistanceJointDef djd = new MaxDistanceJointDef();
           
            djd.Initialize(b1, b2, pos1, pos2,length);
            djd.collideConnected = false;
            MaxDistanceJoint dj = physicsWorld.CreateJoint(djd) as MaxDistanceJoint;
            return dj;
        }


        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="anchor"></param>
        /// <param name="axis"></param>
        public void joinBodies_Prismatic(Body b1, Body b2, Vector2 anchor, Vector2 axis)
        {
            PrismaticJointDef pjd = new PrismaticJointDef();
            pjd.upperTranslation = 1;
            pjd.lowerTranslation = -1;
            pjd.enableMotor = true;
            pjd.maxMotorForce = 1000;
            pjd.motorSpeed = 1;
            pjd.Initialize(b1,b2,anchor,axis);
            PrismaticJoint pj = physicsWorld.CreateJoint(pjd) as PrismaticJoint;
        }

        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="anchor"></param>
        /// <param name="axis"></param>
        /// <param name="uppertranslation"></param>
        /// <param name="lowertranslation"></param>
        /// <param name="motorspeed"></param>
        public void joinBodies_Prismatic(Body b1, Body b2, Vector2 anchor, Vector2 axis, float uppertranslation, float lowertranslation, float motorspeed) 
        {
            PrismaticJointDef pjd = new PrismaticJointDef();
            pjd.Initialize(b1, b2, anchor, axis);
            if(motorspeed != 0)
                pjd.enableMotor = true;
            pjd.motorSpeed = motorspeed;
            pjd.lowerTranslation = lowertranslation;
            pjd.upperTranslation = uppertranslation;

            PrismaticJoint pj = physicsWorld.CreateJoint(pjd) as PrismaticJoint;
        }

        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="anchor"></param>
        /// <param name="limits"></param>
        /// <param name="motor"></param>
        public RevoluteJoint joinBodies_Rotate(Body b1, Body b2, Vector2 anchor, Vector2 limits, float motor) 
        {
            RevoluteJointDef rjd = new RevoluteJointDef();
            rjd.enableLimit = false;
            if (motor == 0)
                rjd.enableMotor = false;
            else
                rjd.enableMotor = true;
            rjd.motorSpeed = motor;
            rjd.collideConnected = false;
            rjd.Initialize(b1,b2,anchor);
            RevoluteJoint rj = physicsWorld.CreateJoint(rjd) as RevoluteJoint;
            return rj;
           
        }
       
        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="anchor"></param>
        /// <returns></returns>
        public WeldJoint joinBodies_Weld(Body b1, Body b2, Vector2 anchor, bool collideCon) 
        {
            WeldJointDef wjd = new WeldJointDef();
            wjd.Initialize(b1,b2,anchor);
            wjd.collideConnected = collideCon;
            WeldJoint wj = physicsWorld.CreateJoint(wjd) as WeldJoint;
            return wj;
        }

        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <param name="gAnchor1"></param>
        /// <param name="gAnchor2"></param>
        /// <param name="ratio"></param>
        /// <returns></returns>
        public PulleyJoint joinBodies_Pulley(Body b1, Body b2, Vector2 gAnchor1,Vector2 gAnchor2, float ratio)
        {
            PulleyJointDef pjd = new PulleyJointDef();
            pjd.Initialize(b1,b2,gAnchor1,gAnchor2,b1.GetWorldCenter(),b2.GetWorldCenter(),ratio);
            PulleyJoint pj = physicsWorld.CreateJoint(pjd) as PulleyJoint;
            return pj;
        }

        #endregion

        #region LoadStuff

        /// <summary>
        /// Load method
        /// </summary>
        /// <param name="filename">File to load from</param>
        public virtual void loadGameWorld(String filename)
        {
            fileLoadedFrom = filename;
            if (!File.Exists(filename))
                throw new Exception("World file not found. --- " + filename);
            XmlTextReader reader = new XmlTextReader(filename);

            reader.ReadToFollowing("TileFile");
            String tile = reader.ReadElementContentAsString();
            reader.ReadToFollowing("MapFile");
            int x = int.Parse(reader.GetAttribute("width"));
            int y = int.Parse(reader.GetAttribute("height"));
            String map = reader.ReadElementContentAsString();
           

            tiledLayer = new TiledLayer(this);
            tiledLayer.load(tile, map, x, y);

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
                    checkpoints.Add(new Vector2(xl,yl));
                }
                while (reader.ReadToNextSibling("CheckPoint"));
            }
            lastCP = checkpoints[0];
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

            if (reader.ReadToFollowing("Joint"))
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
        /// <summary>
        /// Creates a distance joint between two bodies
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Entity loadEntity(XmlTextReader reader)
        {
            Entity newEntity;
            String type = reader.GetAttribute("type");
            String color = reader.GetAttribute("color");
            String noGrav = reader.GetAttribute("noGrav");
            String xLoc = reader.GetAttribute("locationX");
            String aiArgs = reader.GetAttribute("aiArgs");
            String angle = reader.GetAttribute("angle");
            float xPos = float.Parse(xLoc);
            float yPos = float.Parse(reader.GetAttribute("locationY"));
            String id = reader.GetAttribute("id");
            if (id == null)
                id = "NoId";
            String aiName = reader.GetAttribute("aiName");
            String aiFile = reader.GetAttribute("aiFile");
            Body body = null;
            if (type.Contains("Player"))
            {
                id = "Player";
                if (entityManager.player == null)
                {
                    newEntity = new Player.Player(this, entityManager.getType(type));
                    entityManager.addPlayer(newEntity as Player.Player);
                }
                else
                    newEntity = entityManager.player;
                newEntity.type.type = TypeOfThing.PLAYER;
                body = AddBody(new Vector2(xPos, yPos), entityManager.getType(type), newEntity);
                (newEntity as Player.Player).addBody(body, name);
                (newEntity as Player.Player).addType(entityManager.getType(type), name);
                bodyDict.Add(id, body);
                if (angle != null)
                    body.Rotation = (float.Parse(angle)/180)*3.14f;
                AIBase pai = null;
                if (aiName != null)
                {
                    pai = loadAI(newEntity, aiName, aiFile);
                    pai.init();
                 }
                (newEntity as Player.Player).addAI(pai,name);
                
            }
            else if (type.Contains("Character"))//Is anyone who can be talked to
            {
                newEntity = new Character(this, entityManager.getType(type))
                                {type = {type = TypeOfThing.NPC}, animation = "Standing Down"};//This is kind of cool, not really useful, but still cool.
            }
            else if (type.Contains("BuildingEnemy"))
            {
                newEntity = new Character(this, entityManager.getType(type)) { type = { type = TypeOfThing.BUILDING_ENEMY }, animation = "Standing Down" };

            }
            else if (type.Contains("ChopTree"))
            {
                newEntity = new ChopTree(this, entityManager.getType(type)) { type = { type = TypeOfThing.CHOPTREE }, animation = "full" };
                navStuff.addRectangle(new Rectangle((int)(xPos * 64), (int)(yPos * 64), (int)newEntity.type.size.X, (int)newEntity.type.size.Y));

            }
            else if (type.Contains("Bar"))
            {
                newEntity = new PowerBar(this, entityManager.getType(type)) { type = { type = TypeOfThing.BAR }, animation = "_" };
                bar = (PowerBar)newEntity;
            }
            else if (type.Contains("Cannon"))
            {
                newEntity = new Cannon(this, entityManager.getType(type)) { type = { type = TypeOfThing.CANNON }, animation = "_" };
                cannon = (Cannon)newEntity; 
            }
            else if (type.Contains("Building") || type.Contains("BallPlatform"))
            {
                newEntity = new Building(this, entityManager.getType(type))
                                {type = {type = TypeOfThing.BUILDING}, animation = "_"};
                navStuff.addRectangle(new Rectangle((int)(xPos * 64), (int)(yPos * 64), (int)newEntity.type.size.X, (int)newEntity.type.size.Y));
            }
            else if( type.Contains("Block"))//Block is anything that can be pushed around
            {
                newEntity = new Entity(this, entityManager.getType(type))
                                {type = {type = TypeOfThing.BUILDING}, animation = "_"};
            }
            else if (type.Contains("Item"))//Item is anything that can be picked up
            {
                newEntity = new Item(this, entityManager.getType(type), type.Replace("Item", string.Empty))
                                {type = {type = TypeOfThing.ITEM}, animation = "_"};
            }
            else if(type.Contains("Enemy"))
            {
                newEntity = new Character(this, entityManager.getType(type))
                                {type = {type = TypeOfThing.ENEMY}, animation = "Standing Down"};

            }
            else if (type.Contains("Explosive"))
            {
                newEntity = new Explosive(this, entityManager.getType(type)) { type = { type = TypeOfThing.EXPLOSIVE }, animation = "_" };
            }
            else
            {
                throw new Exception("Invalid Entity Type: " + type);
            }

            if (id==null || !id.Equals("Player"))
            {
                body = AddBody(new Vector2(xPos, yPos), newEntity);
                
                body.SetLinearVelocity(Vector2.Zero);
                newEntity.my_Body = body;
                addEntity(newEntity,id);
                if (aiName != null)
                {
                    newEntity.ai = loadAI(newEntity, aiName, aiFile);
                    if (aiArgs != null)
                        newEntity.ai.aiArgs = aiArgs;
                    newEntity.ai.init();
                }
                if (angle != null)
                    newEntity.my_Body.Rotation = (float.Parse(angle) / 180) * 3.14f;
            }
            newEntity.id = id;
            if (noGrav == null || noGrav == "false")
                body.noGravity = false;
            else if (noGrav == "true")
                body.noGravity = true;
            switch(color)
            {
                case("Green"):
            {
                newEntity.color = Color.Green;
                break;
            }
                case ("Purple"):
            {
                newEntity.color = Color.Purple;
                break;
            }
                case ("Red"):
            {
                newEntity.color = Color.Red;
                break;
            }
                default:
            {
                newEntity.color = Color.White;
                break;
            }   
            }

            if (name == "Main")
            {
                if (id != null && newEntity.ai != null && id != "NoId")
                {
                    aiDict.Add(id, newEntity.ai);
                }
            }
            return newEntity;
        }



        /// <summary>
        /// Loads a non entity
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Entity loadNonEntity(XmlTextReader reader)
        {
            Entity newEntity = null;
            String type = reader.GetAttribute("type");
            switch (type)
            {

                case ("CottonShooter"):
                    {
                        newEntity = new NonEntity(this);
                        newEntity.type = new EntityType(game, TypeOfThing.PLAYER);
                        newEntity.ai = new CottonShooter(this, newEntity);
                        entities.Add(newEntity);
                        break;
                    }
                case ("CottonSpawner"):
                    {
                        newEntity = new NonEntity(this);
                        newEntity.type = new EntityType(game, TypeOfThing.PLAYER);
                        newEntity.ai = new CottonSpawner(this, newEntity);
                        entities.Add(newEntity);
                        break;
                    }

                //case ("SoccerBall"):
                //    {
                //        newEntity = new NonEntity(this);
                //        newEntity.type = new EntityType(game, TypeOfThing.PLAYER);
                //        newEntity.ai = new SoccerBall(this, newEntity);
                //        //newEntity.ai.load(@"XML\CottonSpawner.xml");
                //        entities.Add(newEntity);
                //        break;
                //    }
            }
            return newEntity;
        }
        /// <summary>
        /// Loads a joint from file
        /// </summary>
        /// <param name="reader"></param>
        protected void loadJoint(XmlTextReader reader)
        {
            String type = reader.GetAttribute("type");
            String id1 = reader.GetAttribute("id1"); 
            String id2 = reader.GetAttribute("id2");
            Vector2 limits, axis, anchor;

            reader.GetAttribute("anchor");

            if (type.Equals("Distance"))
            {
                joinBodies_Distance(bodyDict[id1],bodyDict[id2]);
            }
            else if (type.Equals("Revolute"))
            {
                limits.X = float.Parse(reader.GetAttribute("lowerlimit"));
                limits.Y = float.Parse(reader.GetAttribute("upperlimit"));
                anchor.X = float.Parse(reader.GetAttribute("anchorX"));
                anchor.Y = float.Parse(reader.GetAttribute("anchorY"));
                float motor = float.Parse(reader.GetAttribute("motor"));
                joinBodies_Rotate(bodyDict[id1], bodyDict[id2], anchor, limits, motor);
            }
            else if (type.Equals("Weld"))
            {
                anchor.X = float.Parse(reader.GetAttribute("anchorX"));
                anchor.Y = float.Parse(reader.GetAttribute("anchorY"));
                joinBodies_Weld(bodyDict[id1],bodyDict[id2], anchor, false);
            }
            else if (type.Equals("Prismatic"))
            {
                anchor.X = float.Parse(reader.GetAttribute("anchorX"));
                anchor.Y = float.Parse(reader.GetAttribute("anchorY"));
                axis.X = float.Parse(reader.GetAttribute("axisX"));
                axis.Y = float.Parse(reader.GetAttribute("axisY"));
                joinBodies_Prismatic(bodyDict[id1], bodyDict[id2], anchor, axis);
            }
            else if (type.Equals("Pulley"))
            {
                anchor.X = float.Parse(reader.GetAttribute("anchor1X"));
                anchor.Y = float.Parse(reader.GetAttribute("anchor1Y"));
                Vector2 anchor2;
                anchor2.X = float.Parse(reader.GetAttribute("anchor2X"));
                anchor2.Y = float.Parse(reader.GetAttribute("anchor2Y"));
                float ratio = float.Parse(reader.GetAttribute("ratio"));
                joinBodies_Pulley(bodyDict[id1], bodyDict[id2],anchor,anchor2,ratio);
            }
            else if (type.Equals("Rope"))
            {
                String slength = reader.GetAttribute("length");
                float length;
                if (slength != null)
                {
                    length = float.Parse(slength);
                }
                else
                     length = Vector2.Distance(bodyDict[id1].Position, bodyDict[id2].Position);
                joinBodies_Rope(bodyDict[id1], bodyDict[id2], length);
            }
            else
            {
                throw new Exception("Bad joint!");
            }
        }

        /// <summary>
        /// Adds edges bounding the world
        /// </summary>
        protected void addBounds(Event[] events)
        {
            float width = sizeInPixels.X / 64;
            float height = sizeInPixels.Y / 64;
            WorldEdge left = new WorldEdge(this, SIDE.LEFT);
            left.type = new EntityType(game, TypeOfThing.WALL);
           WorldEdge right = new WorldEdge(this, SIDE.RIGHT);
            right.type = new EntityType(game,TypeOfThing.WALL);
           WorldEdge bottom = new WorldEdge(this,SIDE.BOTTOM);
            bottom.type = new EntityType(game, TypeOfThing.WALL);
           WorldEdge top = new WorldEdge(this, SIDE.TOP);
            top.type = new EntityType(game,TypeOfThing.WALL);

            if (events != null)
            {
                bottom.collideEvent = events[0];
                left.collideEvent = events[1];
                top.collideEvent = events[2];
                right.collideEvent = events[3];
            }

            BodyDef bd;
            Body body;
            PolygonShape shape;
            //Add physical bounds to the map
            if (walls[2])
            {
                bd = new BodyDef();
                bd.position = new Vector2(0, 0);
                bd.type = BodyType.Static;
                body = physicsWorld.CreateBody(bd);
                 shape = new PolygonShape();
                shape.SetAsEdge(new Vector2(.25f, .25f), new Vector2(width - .25f, .25f));
                body.CreateFixture(shape, 1f);
                body.SetUserData(top);
            }
            if (walls[3])//Right
            {
                bd = new BodyDef();
                bd.type = BodyType.Static;
                body = physicsWorld.CreateBody(bd);
                shape = new PolygonShape();
                shape.SetAsEdge(new Vector2(width - .25f, .25f), new Vector2(width - .25f, height - .5f));
                body.CreateFixture(shape, 1f);
                body.SetUserData(right);
            }
            if (walls[0])//Bottom
            {
                bd = new BodyDef();
                body = physicsWorld.CreateBody(bd);
                shape = new PolygonShape();
                shape.SetAsEdge(new Vector2(width + 25f, height - .75f), new Vector2(-25f, height - .75f));
                body.CreateFixture(shape, 1f);
                body.SetUserData(bottom);
            }
            if (walls[1])//Left
            {
                bd = new BodyDef();
                body = physicsWorld.CreateBody(bd);
                shape = new PolygonShape();
                shape.SetAsEdge(new Vector2(.25f, height - .25f), new Vector2(.25f, .25f));
                body.CreateFixture(shape, 1f);
                body.SetUserData(left);
            }

        }

        /// <summary>
        /// Reads in the collide events for the edges
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        protected Event[] readEdgeEvents(XmlReader reader)
        {
            Event[] events = new Event[4];
            if (reader.ReadToFollowing("Bottom"))
            {
               String type = reader.GetAttribute("type");
               String value = reader.GetAttribute("value");
               if (type.Equals("No_Wall"))
                   walls[0] = false;
               else
                   walls[0] = true;
                events[0] = new Event(type,value);
            }
            if (reader.ReadToNextSibling("Left"))
            {
                String type = reader.GetAttribute("type");
                String value = reader.GetAttribute("value");
                if(type.Equals("No_Wall" ))
                    walls[1] = false;
                else
                    walls[1] = true;
                events[1] = new Event(type, value);
            }
            if (reader.ReadToNextSibling("Top"))
            {
                String type = reader.GetAttribute("type");
                String value = reader.GetAttribute("value");
                 if(type.Equals("No_Wall" ))
                    walls[2] = false;
                else
                    walls[2] = true;
                events[2] = new Event(type, value);
            }
            if (reader.ReadToNextSibling("Right"))
            {
                String type = reader.GetAttribute("type");
                String value = reader.GetAttribute("value");
               if(type.Equals("No_Wall" ))
                    walls[3] = false;
                else
                    walls[3] = true;
                events[3] = new Event(type, value);
            }
            return events;
        }
        /// <summary>
        /// Returns false if the id has been black listed from colliding with player
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool collidesWithPlayer(String id)
        {
            return !idsOfThingsThatDontCollideWithThePlayer.Contains(id);
        }


        /// <summary>
        /// Loads the ai's from file
        /// </summary>
        /// <param name="e"></param>
        /// <param name="name"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        private AIBase loadAI(Entity e, String name, String file)
        {
            AIBase ai = null;
            switch (name)
            {
                case ("PathTester"):
                    {
                        //ai = new PathTester(this, e);
                        break;
                    }
                case ("BarPlacer"):
                    {
                        ai = new BarPlacer(this, e);
                        break;
                    }
                case ("RiceGrabber"):
                    {
                        ai = new RiceGrabber(this, e);
                        break;
                    }
                case ("DivingMonkey"):
                    {
                        ai = new DivingMonkey(this, e);
                        break;
                    }
                case ("SwingingMonkey"):
                    {
                        ai = new SwingingMonkey(this, e);
                        break;
                    }
                case ("StaticPacer"):
                    {
                        ai = new StaticPacer(this, e);
                        break;
                    }
                case ("GenericBuilding"):
                    {
                        ai = new AIBase(this, e);
                        break;
                    }
                case ("Base"):
                    {
                        ai = new AIBase(this, e);
                        break;
                    }
                case ("Gateway"):
                    {
                        ai = new EnterableBuilding(this, e);
                        break;
                    }
                case ("Maze"):
                    {
                        ai = new MazeTraverserAI(this, e);
                        break;
                    }
                case ("Maze2"):
                    {
                        ai = new MazeTraverserAI2(this, e);
                        break;
                    }
                case ("MovingPlatform"):
                    {
                        ai = new MovingPlatform(this, e);
                        break;
                    }
                case ("DrugAddictAI"):
                    {
                        ai = new DrugAddictAI(this, e);
                        break;
                    }
                case ("HangGliderAI"):
                    {
                        ai = new HangGliderAI(this, e);
                        break;
                    }


            }
            if (file != null)
                ai.load(file);
            return ai;
        }


        #endregion

        /// <summary>
        /// Deletes all entities from this world and anything related to entities is reset.
        /// </summary>
        public void deleteAllEntities()
        {
            physicsWorld = new Box2D.XNA.World(gravity,true);
            physicsWorld.ContactFilter = new PCContactFilter();
            physicsWorld.ContactListener = new PCContactListener();
            navStuff = new NavMeshManager((int)sizeInPixels.X, (int)sizeInPixels.Y, game);
            entities = new List<Entity>();
            bodyDict = new Dictionary<string, Body>();
           
            
        }

        /// <summary>
        /// Just load the entities from a world file. (For resetting)
        /// </summary>
        public void loadEntitesFromFile()
        {

            XmlTextReader reader = new XmlTextReader(fileLoadedFrom);
            Event[] events = readEdgeEvents(reader);
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

            if (reader.ReadToFollowing("Joint"))
            {
                do
                {
                    loadJoint(reader);
                } while (reader.ReadToNextSibling("Joint"));
            }
            addBounds(events);
        }


        /// <summary>
        /// Returns the first checkpoint before the given pos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Vector2 getCheckPoint(Vector2 pos)
        {
            Vector2 re = checkpoints[0];
            foreach (Vector2 v in checkpoints)
            {
                if (pos.X >= v.X)
                    re = v;
                else
                    break;
            }
            return re;
        }
    }
}
