using System;
using System.IO;
using System.Xml;
using Entities.Player;
using Entities.World;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.GamerServices;
using Entities;
using Entities.Singletons;
using GUI;

namespace CSE308Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class ProjectCommunity : Game
    {
        Vector2 viewportSize;
        public readonly GraphicsDeviceManager graphics;
        private InputManager input;
        public EntityManager entityManager;
        private SpriteBatch spriteBatch;
        private GameLoader loader;
        private GUIManager myGUIManager;
        private StreamWriter exceptionWriter;
        String fileName = "ErrorLog" + DateTime.Now.DayOfYear + "__" + DateTime.Now.Minute + ".txt";
        private SoundManager audio;

       
        public ProjectCommunity()
        {
            Components.Add(new GamerServicesComponent(this));
            IsMouseVisible = true;
            viewportSize = new Vector2(800,600);
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            loader = GameLoader.getGameLoader(this);
            input = new InputManager(this);
            Components.Add(input);
            entityManager = EntityManager.getEntityManager(this);
            audio = SoundManager.createSoundManager(Content);

        }

        public EntityManager EntityManager
        {
            get
            {
                return entityManager;
            }
        }

        public SpriteBatch SpriteBatch
        {
            get
            {
                return spriteBatch;
            }
        }

        
        public SoundManager SoundManager
        {
            get
            {
                return audio;
            }
        }
  
        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            audio.addEvent(new Event(EventList.PlaySound, "roughThemeSmall"));
            exceptionWriter = new StreamWriter(fileName);
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            viewportSize = loader.loadGameFromFile(@"XML\GamePrototype.xml");
            //Set the window size
            graphics.PreferredBackBufferHeight = (int)viewportSize.Y;
            graphics.PreferredBackBufferWidth = (int)viewportSize.X;
            graphics.ApplyChanges();
            
            myGUIManager = GUIManager.getGUIManager(this, Content);
            myGUIManager.createInformationText("Welcome. Seek out the doctor, he will be indicated by light above his head.", new Vector2(190, 80), Color.DarkSlateGray);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
#if DEBUG
#else
            try
            {
#endif
            //stop gameplay if the current menu is a pause menu
            if (!myGUIManager.CurrentMenu.IsPauseMenu)
            {
                entityManager.getCurrentGameWorld().update(gameTime);
            }
            audio.handleEvents();
            //set first parameter to true to check for GUI input.
            myGUIManager.Update(true, gameTime);
            base.Update(gameTime);

#if DEBUG
#else
            }

            catch (Exception e)
            {
                entityManager.drawString("An Exception Occurred.");
                entityManager.drawString("Please Examine Logfile: " + fileName);
                writeToLog(e);
            }
        
#endif
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

#if DEBUG
#else
            try
            {
#endif
                GraphicsDevice.Clear(Color.CornflowerBlue);
                entityManager.drawCurrentWorld(spriteBatch);
                myGUIManager.Draw(spriteBatch);
#if DEBUG
                entityManager.player.debugDraw(spriteBatch);
#endif
                base.Draw(gameTime);
#if DEBUG
#else
            }

            catch (Exception e)
            {
                entityManager.drawString("An Exception Occurred.");
                entityManager.drawString("Please Examine Logfile: " + fileName);
                writeToLog(e);
            }
#endif
        

        }



        public void writeToLog(Exception e)
        {
            exceptionWriter.WriteLine("**************************************************");
            exceptionWriter.WriteLine(DateTime.Now);
            exceptionWriter.WriteLine(e.Message);
            exceptionWriter.WriteLine(e.StackTrace);
        }





    }
}
