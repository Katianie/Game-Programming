/** XNA_Bababooy_ShootEmUp
* A 2D shooting game where the object is to kill aproaching
* enimies; similar to space invaders.
*
* This was uploaded to Katianie.com, Feel free to use this
* code and share it with others, Just give me credit ^_^.
*
* Eddie O'Hagan
* Copyright © 2009 Katianie.com
*/
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNA_Bababooy_ShootEmUp
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;
        private Texture2D backgroundTex;
        private Rectangle backgroundRect;
        private SpriteBatch backgroundBatch;
        private Bababooy playerOne;
        private Random rand;
        private CollisionClass collisionClass;
        private MenuItem[] myMenuItemArray;
        private MenuItem[] myControlItemArray;
        private Menu myMainMenu;
        private Menu myControlMenu;
        private KeyboardState myKeyState;
        private KeyboardState myPrevKeyState;

        private SpriteBatch myScoreBatch;
        private SpriteBatch myPowerUpBatch;
        private SpriteFont myScoreFont;
        private Vector2 myScorePos;

        private bool myIsPaused;
        private bool myIsMainMenuUp;
        private bool myIsControlMenuUp;

        private Enemy[] myEnemyArray;
        private Powerup[] myPowerups;

        public Game1()
        {
            this.IsMouseVisible = true;
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        private void SetUpXNADevice()
        {
            graphics.PreferredBackBufferWidth = 700;
            graphics.PreferredBackBufferHeight = 550;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            Window.Title = "Bababooy Shoot-em-up";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            SetUpXNADevice();

            myIsMainMenuUp = true;
            myIsControlMenuUp = false;


            playerOne = new Bababooy(graphics, Content);
            myEnemyArray = new Enemy[80];
            myPowerups = new Powerup[4];
            myIsPaused = false;
            myMenuItemArray = new MenuItem[2];
            myControlItemArray = new MenuItem[1];
            myControlItemArray[0] = new MenuItem(graphics, Content, "playbutton", new Rectangle(450, 200, 200, 50));
            myMenuItemArray[0] = new MenuItem(graphics, Content, "playbutton", new Rectangle(50, 200, 200, 50));
            myMenuItemArray[1] = new MenuItem(graphics, Content, "controlsbutton", new Rectangle(50, 300, 200, 50));

            myMainMenu = new Menu(graphics, Content, "splash", new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), myMenuItemArray);
            myControlMenu = new Menu(graphics, Content, "controls", new Rectangle(0, 0, graphics.PreferredBackBufferWidth, graphics.PreferredBackBufferHeight), myControlItemArray);


            myScoreFont = Content.Load<SpriteFont>("TimesNewRoman");
            myScoreBatch = new SpriteBatch(graphics.GraphicsDevice);
            myPowerUpBatch = new SpriteBatch(graphics.GraphicsDevice);
            myScorePos = new Vector2(550, 10);
            rand = new Random();

            backgroundBatch = new SpriteBatch(graphics.GraphicsDevice);
            myKeyState = Keyboard.GetState();
            myPrevKeyState = myKeyState;

            for (int i = 0; i < myEnemyArray.Length; i++)
            {
                if (rand.Next(0, 5) == 1)
                {
                    myEnemyArray[i] = new Enemy(graphics, Content, rand.Next(300, 16000), rand.Next(100, 500), rand.Next(1, 4), "terrorest4", 400);
                }
                else if (rand.Next(0, 4) == 1)
                {
                    myEnemyArray[i] = new Enemy(graphics, Content, rand.Next(300, 16000), rand.Next(100, 500), rand.Next(1, 4), "terrorest3", 300);
                }
                else if (rand.Next(0, 3) == 1)
                {
                    myEnemyArray[i] = new Enemy(graphics, Content, rand.Next(300, 16000), rand.Next(100, 500), rand.Next(1, 4), "terrorest2", 200);
                }
                else
                {
                    myEnemyArray[i] = new Enemy(graphics, Content, rand.Next(300, 16000), rand.Next(100, 500), rand.Next(1, 4));
                }

            }

            myPowerups[0] = new Powerup(Content.Load<Texture2D>("shotgun"), "Shotgun");
            myPowerups[1] = new Powerup(Content.Load<Texture2D>("shotgun"), "Shotgun");
            myPowerups[2] = new Powerup(Content.Load<Texture2D>("Flack"), "Flack");
            myPowerups[3] = new Powerup(Content.Load<Texture2D>("Flack"), "Flack");

            for (int i = 0; i < myPowerups.Length; i++)
            {
                myPowerups[i].X = rand.Next(700, 16000);
                myPowerups[i].Y = rand.Next(100, 500);
            }

            collisionClass = new CollisionClass(playerOne, playerOne.getBullets(), myEnemyArray, myPowerups);

            base.Initialize();
        }

        /// <summary>
        /// Load your graphics content.  If loadAllContent is true, you should
        /// load content from both ResourceManagementMode pools.  Otherwise, just
        /// load ResourceManagementMode.Manual content.
        /// </summary>
        /// <param name="loadAllContent">Which type of content to load.</param>
        protected override void LoadGraphicsContent(bool loadAllContent)
        {
            if (loadAllContent)
            {
                backgroundTex = Content.Load<Texture2D>("field");
                backgroundRect = new Rectangle(0, 0, 700, 550);
            }

            // TODO: Load any ResourceManagementMode.Manual content
        }

        /// <summary>
        /// Unload your graphics content.  If unloadAllContent is true, you should
        /// unload content from both ResourceManagementMode pools.  Otherwise, just
        /// unload ResourceManagementMode.Manual content.  Manual content will get
        /// Disposed by the GraphicsDevice during a Reset.
        /// </summary>
        /// <param name="unloadAllContent">Which type of content to unload.</param>
        protected override void UnloadGraphicsContent(bool unloadAllContent)
        {
            if (unloadAllContent)
            {
                // TODO: Unload any ResourceManagementMode.Automatic content
                Content.Unload();
            }

            // TODO: Unload any ResourceManagementMode.Manual content
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {

            myMainMenu.update();
            myControlMenu.update();
            myKeyState = Keyboard.GetState();

            if (myMainMenu.getItemsArray()[0].isClicked())
            {
                myIsMainMenuUp = false;
            }

            if (myControlMenu.getItemsArray()[0].isClicked())
            {
                myIsControlMenuUp = false;
                myIsMainMenuUp = false;
            }

            if (myMainMenu.getItemsArray()[1].isClicked())
            {
                myIsControlMenuUp = true;
            }

            if (!myIsMainMenuUp && !myIsControlMenuUp)
            {
                myIsMainMenuUp = false;

                if (myIsPaused == false)
                {
                    // Allows the game to exit
                    if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                        this.Exit();

                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        this.Exit();
                    }

                    collisionClass.bulletColideEnemy();

                    if (playerOne.IsRemoved == false)
                    {
                        collisionClass.playerColideEnemy();
                        collisionClass.playerColidePowerUp();
                        collisionClass.Update(playerOne.Points);
                    }

                    for (int i = 0; i < myPowerups.Length; i++)
                    {
                        myPowerups[i].Update();
                    }

                    base.Update(gameTime);
                }
                if (myKeyState.IsKeyDown(Keys.Enter) && myPrevKeyState.IsKeyUp(Keys.Enter))
                {
                    myIsPaused = !myIsPaused;
                }

                myPrevKeyState = Keyboard.GetState();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
            if (myIsMainMenuUp)
            {
                myMainMenu.draw();
            }

            if (myIsControlMenuUp)
            {
                myControlMenu.draw();
            }

            if (!myIsMainMenuUp && !myIsControlMenuUp)
            {
                //Background must be drawn first before any other batches other wise the other
                //batches will not be visible
                backgroundBatch.Begin();
                backgroundBatch.Draw(backgroundTex, backgroundRect, Color.White);
                backgroundBatch.End();

                if (myIsPaused == false)
                {
                    playerOne.Draw();

                    for (int i = 0; i < myEnemyArray.Length; i++)
                    {
                        myEnemyArray[i].draw();
                    }


                }

                myScoreBatch.Begin();
                myScoreBatch.DrawString(myScoreFont, "Score: " + playerOne.Points, myScorePos, Color.White);

                for (int i = 0; i < myPowerups.Length; i++)
                {
                    myPowerUpBatch.Begin();
                    myPowerups[i].Draw(myPowerUpBatch);
                    myPowerUpBatch.End();
                }

                if (myIsPaused)
                {
                    myScoreBatch.DrawString(myScoreFont, "~PAUSED~", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.Red);
                }
                else if (playerOne.IsRemoved == true)
                {
                    myScoreBatch.DrawString(myScoreFont, "   GAME OVER \nPress y to try again", new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 2), Color.Red);

                    if (Keyboard.GetState().IsKeyDown(Keys.Y))
                    {
                        playerOne.IsRemoved = false;
                        playerOne.CurrentPowerup = "None";
                        playerOne.Points = 0;
                        collisionClass.MaxSpeed = 4;
                        collisionClass.resetEnemyVelocity();
                    }
                }
                myScoreBatch.End();

                base.Draw(gameTime);
            }
        }
    }
}
