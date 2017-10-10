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
using Entities;
using GUI;

namespace CSE308Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        InputManager input;
        SpriteBatch spriteBatch
        {
            get;
            set;
        }

        GameWorld gameWorld;
        private GameLoader loader;
        private GUIManager myGUIManager;

        public Game1()
        {
            this.IsMouseVisible = true;

            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            gameWorld = new GameWorld(this);
            loader = new GameLoader(gameWorld);
            input = new InputManager(gameWorld);

        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            gameWorld.loadFonts();
            gameWorld.viewport = new Viewport(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            loader.loadGameFromFile("GamePrototype.xml");
            myGUIManager = GUIManager.createGUIManager(graphics, Content, gameWorld);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // TODO: Add your update logic here
            // gameWorld.update(spriteBatch);
            input.update();

            //stop gameplay if the current menu is a pause menu

            if (!myGUIManager.CurrentMenu.IsPauseMenu)
            {
                gameWorld.update(gameTime);
            }

            myGUIManager.Update();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            // gameWorld. entityManager.drawEntities();
            // TODO: Add your drawing code here

            gameWorld.draw(spriteBatch);
            myGUIManager.Draw();

            base.Draw(gameTime);
        }
    }
}
