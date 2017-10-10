using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Box2D.XNA;
using Entities.Singletons;
using Entities.AI;
using Entities.World;
using Microsoft.Xna.Framework.Input;
using GUI;

namespace Entities.Player
{
    /// <summary>
    ///Player Object.  Currently an ever shifting flow from hacks to solid game logic.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class Player : Entity
    {
        public int vaccinate = 0;
        private readonly SpriteFont myFont;
        public readonly Dictionary<String, Body> bodies;
        private Dictionary<String, EntityType> types;
        private readonly Dictionary<String, AIBase> ais;
        private int jumpcount = 0;
        private bool canJump = true;
        public int enteringWorld = 25;
        public Inventory inventory;
        public Quest activeQuest;
        SoundManager audio;
        private QuestManager myQuestManager;
        private Cannon myCannon;
        private WeldJoint tempJoint;
        public String gamerName = "none";
        private GUIManager myGUIManager;
        private Inventory lastInventory;
        private float myGlidingAcceleration;
        public bool myRestrictMovement;
        public bool myHasLaunched;
        public int myCannonHighScore;
        private float myLastScore;

        /// <summary>
        ///Constructor 
        /// </summary>
        /// <param name="_gameWorld">Gameworld starting in</param>
        /// <param name="et">EntityType</param>
        /// <param name="x">init X location</param>
        /// <param name="y">init Y location</param>
        public Player(GameWorld _gameWorld, EntityType et)
            : base(_gameWorld, et)
        {
            animation = "Standing Down";
            myGlidingAcceleration = 0.2f;

            gameWorld = _gameWorld;
            game = gameWorld.getGame();
            bodies = new Dictionary<string, Body>();
            types = new Dictionary<string, EntityType>();
            ais = new Dictionary<string, AIBase>();

            myFont = FontManager.getFontManager(game.Content).getFont("Whatever");
            inventory = new Inventory();
            isPlayer = true;
            
            audio = SoundManager.createSoundManager(game.Content);
            myQuestManager = QuestManager.createQuestManager();

            myGUIManager = GUIManager.getGUIManager(game, game.Content);
            animate();

            myRestrictMovement = false;
            myHasLaunched = false;
            myCannonHighScore = 0;

        }

        public float LastScore
        {
            get
            {
                return myLastScore;
            }
            set
            {
                myLastScore = value;
            }
        }

        public Inventory PlayerInventory
        {
            get
            {
                return inventory;
            }
        }

        public Cannon PlayerCannon
        {
            get
            {
                return myCannon;
            }
            set
            {
                myCannon = value;
            }
        }

        /// <summary>
        /// Draw method, maintains viewport, draws debug data on position
        /// </summary>
        /// <param name="sb">SpriteBatch</param>
        public override void draw(SpriteBatch sb)
        {
            updateViewport();

            if (gameWorld.name.Contains("Cannon"))
            {
                SpriteFont aFont = GUI.FontManager.getFontManager(game.Content).getFont("Beyond Wonderland");
                sb.DrawString(aFont, "High Score:" + myCannonHighScore, new Vector2(200, 100), Color.Gold); 
            }

            if (!myRestrictMovement)
            {
                base.draw(sb);
            }
        }


        /// <summary>
        /// Draws debugging data. Should be called last.
        /// </summary>
        /// <param name="sb"></param>
        public void debugDraw(SpriteBatch sb)
        {
            Vector2 v = new Vector2(35, 35);
            sb.Begin();
            sb.DrawString(myFont, (my_Body.GetPosition()).ToString(), v, Color.White);
            v.Y += 40;
            if (Gamer.SignedInGamers.Count > 0 && gamerName == "none")
                gamerName = Gamer.SignedInGamers[0].DisplayName;
            sb.DrawString(myFont, gamerName, v, Color.White);

            v = new Vector2(635, 35);
            foreach (String s in inventory.lastThree)
            {
                String str = s + ": " + inventory[s].ToString();
                sb.DrawString(myFont, str, v, Color.Wheat);
                v.Y += 35;
            }
            sb.End();
        }

        /// <summary>
        /// Handle the various events that may have occurred.
        /// </summary>
        public override void handleEvents()
        {

            Vector2 newVelocity = Vector2.Zero;
            Vector2 newImpulse = Vector2.Zero;
            if (gameWorld.gravity.Y != 0 && !my_Body.noGravity)
            {
                newVelocity = my_Body.GetLinearVelocity();
                my_Body.SetLinearDamping(1f);
            }
            else
            {
                my_Body.SetLinearDamping(0f);
            }

            GUI.GUIManager.getGUIManager(game, game.Content).Inventory = inventory;

            while (eventList.Count > 0)
            {
                Event e = eventList[0];
                eventList.RemoveAt(0);
                switch (e.type)
                {
                    case ("Set_Player_Color"):
                        {
                            if ((e._value as String).Equals("Blue"))
                                color = Color.Blue;
                            if ((e._value as String).Equals("Red"))
                                color = Color.Red;
                            if ((e._value as String).Equals("Green"))
                                color = Color.Green;
                            break;
                        }
                    case (EventList.DisplayInventory):
                        {
                            myGUIManager.toggleDisplayInventory();
                            myGUIManager.setInventoryData(inventory, EntityManager.inventoryImages, "Player");
                            break;
                        }
                    case (EventList.DisplayQuestLog):
                        {
                            myGUIManager.displayQuestLog();
                            break;
                        }
                    case (EventList.TogglePlayerGUI):
                        {
                            myGUIManager.PlayersGUI.IsHidden = !myGUIManager.PlayersGUI.IsHidden;
                            break;
                        }
                    case (EventList.RotateCannonCCW):
                        {
                            myCannon = gameWorld.cannon;
                            if (myCannon != null)
                            {
                                myCannon.addEvent(new Event(EventList.RotateCannonCCW, null));
                            }
                            break;
                        }
                    case (EventList.RotateCannonCW):
                        {
                            myCannon = gameWorld.cannon;
                            if (myCannon != null)
                            {
                                myCannon.addEvent(new Event(EventList.RotateCannonCW, null));
                            }
                            break;
                        }

                    case (EventList.playerCannonBallLaunch):
                        {
                            myCannon = gameWorld.cannon;
                            if (myCannon != null)
                            {
                                myCannon.addEvent(new Event(EventList.playerCannonBallLaunch, null));
                            }
                            break;
                        }
                    case (EventList.PlayerDown):
                        {
                            if (gameWorld.gravity.Y == 0 || my_Body.noGravity)
                            {
                                int value = (int)e._value;
                                newVelocity.Y = value;
                            }
                            break;
                        }

                    case (EventList.PlayerUp):
                        {
                            if (gameWorld.gravity.Y == 0 || my_Body.noGravity)
                            {
                                int value = (int)e._value;
                                newVelocity.Y = -value;
                            }
                            else
                            {
                                if (jumpcount < 2 && canJump)
                                {
                                    int value = (int)e._value;
                                    float mass = my_Body.GetMass();
                                    newImpulse = new Vector2(0, -value * mass);
                                    canJump = false;
                                    jumpcount++;
                                    newImpulse /= jumpcount;
                                }
                            }
                            break;
                        }
                    case ("NotUp"):
                        {
                            canJump = true;
                            break;
                        }
                    case (EventList.PlayerLeft):
                        {
                            int value = (int)e._value;
                            newVelocity.X = -value;
                            break;
                        }
                    case (EventList.PlayerRight):
                        {
                            int value = (int)e._value;
                            newVelocity.X = value;
                            break;
                        }
                    case (EventList.FireVaccine):
                        {
                            if (inventory.ContainsKey("DartGun") && inventory.takeItems("Vaccine", 1))
                            {
                                EntityManager entityManager = EntityManager.getEntityManager(game);
                                Entity newEntity = new Projectile(gameWorld,
                                    entityManager.getType("SyringeBlock"), 1000, new Event("Vaccinate", "PlaciboVax"));
                                Vector2 position = my_Body.Position * 64;
                                Vector2 vel = my_Body.GetLinearVelocity();
                                if (animation.Contains("Right"))
                                {
                                    position.X += type.images[0].Width + newEntity.type.images[0].Width + 5;
                                    position.Y += (type.images[0].Height / 2) - (newEntity.type.images[0].Height / 2);
                                    newEntity.animation = "3";
                                    vel.X += 10;
                                }
                                if (animation.Contains("Left"))
                                {
                                    position.X -= newEntity.type.images[0].Width - 5;
                                    position.Y += (type.images[0].Height / 2) - (newEntity.type.images[0].Height / 2);
                                    newEntity.animation = "2";
                                    vel.X -= 10;
                                }
                                if (animation.Contains("Up"))
                                {
                                    position.Y -= newEntity.type.images[0].Height - 5;
                                    position.X += (type.images[0].Width / 2) - (newEntity.type.images[0].Width / 2);
                                    newEntity.animation = "_";
                                    vel.Y -= 10;
                                }
                                if (animation.Contains("Down"))
                                {
                                    position.Y += type.images[0].Height + newEntity.type.images[0].Height + 5;
                                    position.X += (type.images[0].Width / 2) - (newEntity.type.images[0].Width / 2);
                                    newEntity.animation = "1";
                                    vel.Y += 10;
                                }
                                newEntity.color = Color.White;
                                newEntity.type.type = TypeOfThing.PROJECTILE;
                                newEntity.my_Body = gameWorld.AddBody(position / 64, newEntity);
                                newEntity.my_Body.SetLinearVelocity(vel);
                                gameWorld.addEntity(newEntity, null);

                            }
                            break;
                        }
                    case (EventList.PlaceBuilding):
                        {
                            String str = e._value as String;
                            if (str.Equals("Bar") && ai != null)
                            {
                                (ai as BarPlacer).placeBar();
                            }
                            if (str.Equals("SmlResBuilding"))
                            {
                                EntityManager entityManager = EntityManager.getEntityManager(game);
                                Entity newEntity = new Building(gameWorld, entityManager.getType("SmlResBuilding"));
                                Vector2 position = my_Body.Position * 64;
                                if (animation.Contains("Right"))
                                {
                                    position.X += type.images[0].Width + 5;
                                }
                                if (animation.Contains("Left"))
                                {
                                    position.X -= newEntity.type.images[0].Width - 5;
                                }
                                if (animation.Contains("Up"))
                                {
                                    position.Y -= newEntity.type.images[0].Height - 5;
                                }
                                if (animation.Contains("Down"))
                                {
                                    position.Y += type.images[0].Height + 5;
                                }
                                newEntity.color = Color.Moccasin;
                                newEntity.type.type = TypeOfThing.BUILDING;
                                newEntity.animation = "_";
                                gameWorld.navStuff.addRectangle(new Rectangle((int)position.X, (int)position.Y, (int)newEntity.type.size.X,
                                    (int)newEntity.type.size.Y));
                                newEntity.my_Body = gameWorld.AddBody(position / 64, newEntity);
                                gameWorld.addEntity(newEntity, null);
                            }
                            break;
                        }
                    case (EventList.AbsorbObjects):
                        {
                            collectItems();
                            break;
                        }
                    case (EventList.IncreaseVelocity):
                        {
                            newVelocity.Y -= myGlidingAcceleration;
                            newVelocity.X += (myGlidingAcceleration / 5.0f);
                            break;
                        }
                    case (EventList.DecreaseVelocity):
                        {
                            newVelocity.Y += myGlidingAcceleration;
                            newVelocity.X += (myGlidingAcceleration / 5.0f);
                            break;
                        }
                    case (EventList.ExitShopItems):
                        {
                            if (myGUIManager.isShopMenuShowing())
                            {
                                myGUIManager.displayShopMenu();
                            }
                            break;
                        }
                    case (EventList.PlantTree):
                        {
                            EntityManager entityManager = EntityManager.getEntityManager(game);
                            Entity newEntity = new ChopTree(gameWorld, entityManager.getType("ChopTree"));
                            Vector2 position = my_Body.Position * 64;

                            newEntity.color = Color.White;
                            newEntity.type.type = TypeOfThing.CHOPTREE;
                            newEntity.animation = "baby";
                            gameWorld.navStuff.addRectangle(new Rectangle((int)position.X, (int)position.Y, (int)newEntity.type.size.X,
                                (int)newEntity.type.size.Y));
                            newEntity.my_Body = gameWorld.AddBody(position / 64, newEntity);
                            gameWorld.addEntity(newEntity, null);

                            break;
                        }
                    case (EventList.GoToPoint):
                        {
                            try
                            {
                                my_Body.Position = (Vector2)e._value;
                            }
                            catch (InvalidCastException ice)
                            {
                                String[] strs = (e._value as String).Split(',');
                                Vector2 point = new Vector2(float.Parse(strs[0]), float.Parse(strs[1]));
                                my_Body.Position = point;
                            }
                            my_Body.SetLinearVelocity(Vector2.Zero);
                            enteringWorld = 20;
                            break;
                        }
                    case (EventList.GrabIRemainingtems):
                        {
                            EntityManager em = EntityManager.getEntityManager(game);
                            String worldName = em.getCurrentGameWorld().name;
                            List<Entity> ents = e._value as List<Entity>;
                            int i = 0;
                            foreach (Entity itemEnt in ents)
                            {
                                if (itemEnt.type.type == TypeOfThing.ITEM2)
                                {
                                    if (itemEnt.my_Body.Position.Y < 15)
                                    {
                                        i++;
                                        inventory.AddItems((itemEnt as Item).itemType, 1);
                                    }
                                }

                            }
                            if (activeQuest != null)
                            {
                                if (activeQuest.completedCallback != null)
                                    activeQuest.completedCallback(i);
                            }
                            switchBody("Main");
                            em.reloadGameWorld(worldName);
                            break;
                        }
                    case (EventList.SwitchWorld):
                        {
                            String[] strs = (e._value as String).Split(',');
                            switchBody(strs[0]);
                            EntityManager.getEntityManager(game).setCurrentGameWorld(strs[0]);
                            copyInventory();
                            if (strs.Length > 1)
                            {
                                Vector2 pos = new Vector2(float.Parse(strs[1]), float.Parse(strs[2]));
                                my_Body.Position = pos;
                            }
                            break;
                        }
                    case (EventList.QuestCompletedReturn):
                        {
                            EntityManager em = EntityManager.getEntityManager(game);
                            String worldName = em.getCurrentGameWorld().name;
                            String worldToReturnTo = e._value as String;
                            if (activeQuest != null)
                            {
                                if (activeQuest.completedCallback != null)
                                    activeQuest.completedCallback(null);
                            }
                            if (worldName != worldToReturnTo)
                            {
                                switchBody(worldToReturnTo);
                                em.reloadGameWorld(worldName);
                            }
                            // ai.nextConversation();
                            //SoundManager.createSoundManager(game.Content).addEvent(new Event(EventList.PlaySound, "pickup"));
                            break;
                        }
                    case (EventList.ResetWorld):
                        {
                            String[] strs = (e._value as String).Split(',');
                            Vector2 pos = new Vector2(float.Parse(strs[0]), float.Parse(strs[1]));
                            EntityManager em = EntityManager.getEntityManager(game);
                            String worldName = em.getCurrentGameWorld().name;
                            switchBody("Main");
                            em.reloadGameWorld(worldName);
                            switchBody(worldName);
                            my_Body.Position = pos;
                            break;
                        }
                    case (EventList.ResetWorld_Checkpoint):
                        {
                            Vector2 pos = gameWorld.lastCP;
                            EntityManager em = EntityManager.getEntityManager(game);
                            String worldName = em.getCurrentGameWorld().name;
                            switchBody("Main");
                            em.reloadGameWorld(worldName);
                            switchBody(worldName);
                            my_Body.Position = pos;
                            resetInventory();
                            break;
                        }
                    default:
                        {
                            //throw new Exception("Unknown Event: " + e.type);
                            break;
                        }
                }
            }


            if (enteringWorld <= 0)
            {
                if (!gameWorld.name.Contains("Cannon Game"))
                {
                    if (newVelocity.Y < -4)
                    {
                        newVelocity.Y = -4;
                    }
                }
                else
                {
                    if (newVelocity.X < 0)
                    {
                        newVelocity.X = 0;
                    }
                }

                if(gameWorld.name.Contains("Doctor3") && newVelocity.Y > 3)
                {
                    newVelocity.Y = 3;
                }

                my_Body.SetLinearVelocity(newVelocity);
                my_Body.ApplyLinearImpulse(newImpulse, my_Body.GetWorldCenter());
            }
            else
            {
                newVelocity.X = 0;
                my_Body.SetLinearVelocity(newVelocity);
                enteringWorld--;
            }

            if (gameWorld.gravity == Vector2.Zero)
            {
                if (newVelocity.Equals(Vector2.Zero))
                {
                    if (animation.Contains("Standing"))
                    {

                    }
                    else if (animation.Contains("Walking"))
                    {
                        animation = animation.Replace("Walking", "Standing");
                    }
                    else
                        throw new Exception("Animation State not recognized");
                }
                else if (newVelocity.X > 0)
                {
                    animation = "Walking Right";
                }
                else if (newVelocity.X < 0)
                {
                    animation = "Walking Left";
                }
                else if (newVelocity.Y > 0)
                {
                    animation = "Walking Down";
                }
                else if (newVelocity.Y < 0)
                {
                    animation = "Walking Up";
                }
            }
            else
            {
                if (newVelocity.Equals(Vector2.Zero))
                {
                    if (animation.Contains("Standing"))
                    {

                    }
                    else if (animation.Contains("Walking"))
                    {
                        animation = animation.Replace("Walking", "Standing");
                    }
                    else
                        throw new Exception("Animation State not recognized");
                }
                if (newVelocity.X > .5)
                {
                    animation = "Walking Right";
                }
                else if (newVelocity.X < -.5)
                {
                    animation = "Walking Left";
                }
                else
                {
                    animation = animation.Replace("Walking", "Standing");
                }
            }
        }

        /// <summary>
        /// Centers viewport on player
        /// NOTE: Viewport jitters when running into wall, should smooth if possible
        /// </summary>
        public void updateViewport()
        {
            int width = gameWorld.viewport.Width;
            int height = gameWorld.viewport.Height;

            Vector2 position = my_Body.GetPosition();
            position *= 64;
            int viewportX = (int)position.X - width / 2;
            int viewportY = (int)position.Y - height / 2;

            //HACK FOR FALLING GAME
            if(gameWorld.name == "EcoGuy2")
                viewportY = (int)position.Y - height / 5;


            if (viewportX < 0)
            {
                viewportX = 0;
            }
            if (viewportY < 0)
            {
                viewportY = 0;
            }
            if (viewportX > gameWorld.sizeInPixels.X - gameWorld.viewport.Width - 1)
            {
                viewportX = (int)(gameWorld.sizeInPixels.X - gameWorld.viewport.Width - 1);
            }
            if (viewportY > gameWorld.sizeInPixels.Y - gameWorld.viewport.Height - 1)
            {
                viewportY = (int)(gameWorld.sizeInPixels.Y - gameWorld.viewport.Height - 1);
            }

            gameWorld.viewport.X = viewportX;
            gameWorld.viewport.Y = viewportY;
        }


        /// <summary>
        /// Collide method
        /// </summary>
        /// <param name="otherThing"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            //Reset jumping
            if (touching && otherThing.type.type == TypeOfThing.BUILDING)
            {
                if (otherThing.my_Body.Position.Y - 1 > my_Body.Position.Y)
                    jumpcount = 0;
            }
            //Pick up item
            if (otherThing.type.type == TypeOfThing.ITEM && touching)
            {
                inventory.AddItems((otherThing as Item).itemType, 1);
                gameWorld.removeEntity(otherThing);
                SoundManager.createSoundManager(game.Content).addEvent(new Event(EventList.PlaySound, "pickup"));
            }
            //Reset to checkpoint
            if ((otherThing.type.type == TypeOfThing.ENEMY || otherThing.type.type == TypeOfThing.BUILDING_ENEMY) && touching)
            {
                Event e = new Event(EventList.ResetWorld_Checkpoint, "3,3");
                addEvent(e);
            }

            //Grabbing control
            if (InputManager.lastState.IsKeyDown(Keys.G))
            {
                if (tempJoint == null)
                {
                    if (otherThing.id != null && otherThing.id.Contains("hook"))
                    {
                        otherThing.my_Body.Position = my_Body.Position;

                        tempJoint = gameWorld.joinBodies_Weld(my_Body, otherThing.my_Body, new Vector2(my_Body.Position.X, my_Body.Position.Y + 2), true);
                        
                    }
                }

            }
            else
            {
                if (tempJoint != null)
                {
                    gameWorld.physicsWorld.DestroyJoint(tempJoint);
                    tempJoint = null;
                    //canGrab = false;
                }
            }
          




        }



        #region Add Switch, and Remove

        /// <summary>
        /// Add a player AI
        /// </summary>
        /// <param name="_ai"></param>
        /// <param name="worldName"></param>
        public void addAI(AIBase _ai, String worldName)
        {
            ais.Add(worldName, _ai);
        }

        /// <summary>
        /// Removes an instance of the player for the corresponding world
        /// </summary>
        /// <param name="worldname"></param>
        public void removeAPlayer(String worldname)
        {
            types.Remove(worldname);
            bodies.Remove(worldname);
            ais.Remove(worldname);

        }

        /// <summary>
        /// Add a player body
        /// </summary>
        /// <param name="_body"></param>
        /// <param name="worldName"></param>
        public void addBody(Body _body, String worldName)
        {
            if (my_Body == null)
            {
                my_Body = _body;
            }
            bodies.Add(worldName, _body);

        }

        /// <summary>
        /// remove a player body
        /// </summary>
        /// <param name="worldName"></param>
        public void deleteBody(String worldName)
        {
            bodies.Remove(worldName);
        }

        /// <summary>
        /// Add an EntityType a player can be
        /// </summary>
        /// <param name="_type"></param>
        /// <param name="worldName"></param>
        public void addType(EntityType _type, String worldName)
        {
            if (type == null)
            {
                type = _type;
            }
            if (!types.ContainsKey(worldName))
                types.Add(worldName, _type);

        }

        /// <summary>
        /// Switches the player between bodies that exist in different gameworlds.
        /// </summary>
        /// <param name="worldName"></param>
        public void switchBody(String worldName)
        {
            my_Body = bodies[worldName];
            type = types[worldName];
            ai = ais[worldName];
            frame = 0;
            EntityManager.getEntityManager(game).setCurrentGameWorld(worldName);
            gameWorld = EntityManager.getEntityManager(game).getCurrentGameWorld();
         enteringWorld = 30;
        }

        #endregion
        
        /// <summary>
        /// Kills all joints the player is involved with
        /// </summary>
        public void killJoints()
        {
            JointEdge je = my_Body.GetJointList();
            while (je != null)
            {
                gameWorld.physicsWorld.DestroyJoint(je.Joint);
                je = je.Next;
            }
        }

        /// <summary>
        /// Removes all jointed items and adds them to the inventory
        /// </summary>
        public void collectItems()
        {
            JointEdge je = my_Body.GetJointList();
            while (je != null)
            {
                Body b = je.Other;
                Joint j = je.Joint;
                Item itemEnt = b.GetUserData() as Item;
                if (itemEnt.type.type == TypeOfThing.ITEM)
                    inventory.AddItems((itemEnt).itemType, 1);

                je = je.Next;
                gameWorld.physicsWorld.DestroyJoint(j);
                gameWorld.removeEntity(itemEnt);
            }

        }


        /// <summary>
        /// Should collide override, checks the list
        /// loaded from file of things the player shouldn't collide with
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public override bool shouldCollide(Entity ent)
        {
            return gameWorld.collidesWithPlayer(ent.id);
        }

        /// <summary>
        /// Copies the current inventory to lastInventory
        /// </summary>
        public void copyInventory()
        {
            lastInventory = new Inventory();
            foreach (String s in inventory.Keys)
            {
                lastInventory.Add(s, inventory[s]);
            }
        }

        /// <summary>
        /// Sets the current inventory to last inventory
        /// </summary>
        public void resetInventory()
        {
            inventory = lastInventory;
            copyInventory();
        }
    }
}
