using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Entities.Singletons;
using Entities.Player;
using GUI;

namespace Entities
{
    /// <summary>
    /// This converts device input into game logic in the form of Events.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class InputManager : GameComponent
    {
        private readonly Game game;
        private readonly EntityManager myEntityManager;
        private GUIManager myGUIManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public InputManager(Game _game)
            : base(_game)
        {
            speed = 3;
            game = _game;
            myEntityManager = EntityManager.getEntityManager(game);
            lastState = Keyboard.GetState();
        }


        /// <summary>
        /// Speed, can be changed with 9 and 0 for testing purposes.
        /// </summary>
        public static int speed { get; set; }

        /// <summary>
        /// Last Keyboard state used for checking whether a key is down for the first time.
        /// Can be used as current keyboard state by other classes.
        /// </summary>
        public static KeyboardState lastState { get; set; }



        /// <summary>
        /// Called each frame. Reads in keyboard and
        /// mouse input and triggers events accordingly.
        /// </summary>
        public override void Update(GameTime gt)
        {
            KeyboardState state = Keyboard.GetState();
            movementKeys(state);
            cheatKeys(state);
            menuKeys(state);
            gameplayKeys(state);
            lastState = state;
        }

        /// <summary>
        /// Handles all gameplay keys such as shooting and placing buildings
        /// </summary>
        /// <param name="state"></param>
        private void gameplayKeys(KeyboardState state)
        {
            Player.Player player = EntityManager.getEntityManager(game).player;
            if (state.IsKeyDown(Keys.B) && lastState.IsKeyUp(Keys.B))
            {
                player.addEvent(new Event(EventList.PlaceBuilding, "Bar"));
            }
            if (state.IsKeyDown(Keys.F) && !lastState.IsKeyDown(Keys.F))
            {
                player.addEvent(new Event(EventList.FireVaccine, "nothing"));
            }
            if (state.IsKeyDown(Keys.Z) && lastState.IsKeyUp(Keys.Z))
            {
                myEntityManager.saveGame();
            }
            if (state.IsKeyDown(Keys.X) && !lastState.IsKeyDown(Keys.X))
            {
                myEntityManager.loadGame();
            }

        }

        /// <summary>
        /// Checks for the keys relating to menu states.
        /// </summary>
        /// <param name="state"></param>
        private void menuKeys(KeyboardState state)
        {
            Player.Player player = EntityManager.getEntityManager(game).player;
            if (state.IsKeyDown(Keys.I) && !lastState.IsKeyDown(Keys.I))
            {
                player.addEvent(new Event(EventList.DisplayInventory, null));
            }

            if (state.IsKeyDown(Keys.L) && !lastState.IsKeyDown(Keys.L))
            {
                player.addEvent(new Event(EventList.DisplayQuestLog, null));
            }

            if (state.IsKeyDown(Keys.U) && !lastState.IsKeyDown(Keys.U))
            {
                player.addEvent(new Event(EventList.TogglePlayerGUI, null));
            }

            if (state.IsKeyDown(Keys.J) && !lastState.IsKeyDown(Keys.J))
            {
                player.addEvent(new Event(EventList.PlaceBuilding, "SmlResBuilding"));
            }

            if (state.IsKeyDown(Keys.P) && !lastState.IsKeyDown(Keys.P))
            {
                if (player.inventory["Acorn"] > 0)
                {
                    player.addEvent(new Event(EventList.PlantTree, "CHOPTREE"));
                    player.inventory["Acorn"]--;
                }
            }

            if (state.IsKeyDown(Keys.Escape))
            {
                //game.Exit();
                myGUIManager = GUIManager.getGUIManager(game, game.Content);

                player.addEvent(new Event(EventList.ExitShopItems, null));

            }
        }

        /// <summary>
        /// Checks for cheat keys.
        /// </summary>
        /// <param name="state"></param>
        private void cheatKeys(KeyboardState state)
        {
            Player.Player player = EntityManager.getEntityManager(game).player;
            //World Switching
            if (state.IsKeyDown(Keys.Delete) && !lastState.IsKeyDown(Keys.Delete))
            {
                bool n = EntityManager.getEntityManager(game).getCurrentGameWorld().navDebug;
                myEntityManager.getCurrentGameWorld().navDebug = !n;
            }
            if (state.IsKeyDown(Keys.PageDown) && !lastState.IsKeyDown(Keys.PageDown))
            {
                player.addEvent(new Event(EventList.QuestCompletedReturn, "Main"));
                // myEntityManager.setCurrentGameWorld("Main");
                //player.switchBody("Main");
            }
            if (state.IsKeyDown(Keys.End) && !lastState.IsKeyDown(Keys.End))
            {
                myEntityManager.setCurrentGameWorld(QuestList.Drugs);
                player.switchBody(QuestList.Drugs);
            }
            if (state.IsKeyDown(Keys.R) && !lastState.IsKeyDown(Keys.R))
            {
                Vector2 pos = player.my_Body.Position;
                player.addEvent(new Event(EventList.ResetWorld, pos.X.ToString() + "," + pos.Y.ToString()));
            }
            //if (state.IsKeyDown(Keys.PageUp))
            //{
            //    EntityManager.getEntityManager(game).setCurrentGameWorld(QuestList.Switchboard);
            //    player.switchBody(QuestList.Switchboard);
            //    player.animation = "Standing Right";
            //}


            if (state.IsKeyDown(Keys.D9) && !lastState.IsKeyDown(Keys.D9) && speed < 12)
                speed++;
            if (state.IsKeyDown(Keys.D0) && !lastState.IsKeyDown(Keys.D0) && speed > 1)
                speed--;

            if (state.IsKeyDown(Keys.LeftControl) && state.IsKeyDown(Keys.C))
                myEntityManager.clearStringList();
        }

        private void movementKeys(KeyboardState state)
        {
            Player.Player player = EntityManager.getEntityManager(game).player;
            if (state.IsKeyDown(Keys.Down) || state.IsKeyDown(Keys.S))
            {
                player.addEvent(new Event(EventList.PlayerDown, speed));
            }


            if (myEntityManager.getCurrentGameWorld().name.Contains("Hang Glide"))
            {
                if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
                {
                    player.addEvent(new Event(EventList.IncreaseVelocity, speed));
                }
                else
                {
                    player.addEvent(new Event(EventList.NotUp, null));
                }

                if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                {
                    player.addEvent(new Event(EventList.PlayerRight, speed));
                }

            }


            

            else
            {


                if (myEntityManager.getCurrentGameWorld().name.Contains("Cannon Game"))
                {
                    if (player.myRestrictMovement)
                    {

                        if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                        {
                            player.addEvent(new Event(EventList.RotateCannonCCW, null));
                        }
                        else if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                        {
                            player.addEvent(new Event(EventList.RotateCannonCW, null));
                        }

                        else if (state.IsKeyDown(Keys.P))
                        {
                            player.myRestrictMovement = false;
                            player.myHasLaunched = true;
                            player.addEvent(new Event(EventList.playerCannonBallLaunch, null));
                        }

                        else
                        {
                            player.addEvent(new Event(EventList.DontRotateCannon, null));
                        }
                    }
                }

                if (!player.myRestrictMovement && !player.myHasLaunched)
                {
                    if (state.IsKeyDown(Keys.Left) || state.IsKeyDown(Keys.A))
                    {
                        player.addEvent(new Event(EventList.PlayerLeft, speed));
                    }

                    if (state.IsKeyDown(Keys.Right) || state.IsKeyDown(Keys.D))
                    {
                        player.addEvent(new Event(EventList.PlayerRight, speed));
                    }
                }

                //No jumping or moving up allowed for pitfall game
                if (!myEntityManager.getCurrentGameWorld().name.Contains("Pit Fall"))
                {
                    //check to see whether to use jump or move up
                    if (myEntityManager.getCurrentGameWorld().gravity == Vector2.Zero || player.my_Body.noGravity)
                    {
                        if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Up))
                        {
                            player.addEvent(new Event(EventList.PlayerUp, speed));
                        }
                        else
                        {
                            player.addEvent(new Event(EventList.NotUp, null));
                        }
                    }
                    else
                    {

                        if(!player.myRestrictMovement)
                        {
                            if (state.IsKeyDown(Keys.W) || state.IsKeyDown(Keys.Space) || state.IsKeyDown(Keys.Up))
                            {
                                if (!lastState.IsKeyDown(Keys.W) && !lastState.IsKeyDown(Keys.Space) && !lastState.IsKeyDown(Keys.Up))
                                {
                                    player.addEvent(new Event(EventList.PlayerUp, speed + 3));
                                }
                                else
                                {
                                    player.addEvent(new Event(EventList.NotUp, null));
                                }
                            }
                            else
                            {
                                player.addEvent(new Event(EventList.NotUp, null));
                            }
                        }

                    }
                }
            }

        }
    }
}
