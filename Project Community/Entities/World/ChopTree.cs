using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Entities.Singletons;
namespace Entities.World
{
    /// <summary>
    /// Represents a chop able tree
    /// </summary>
    /// <Owner>Edward Francis Katianie O'Hagan</Owner>
    public class ChopTree : Entity
    {
        private Player.Player myPlayer;
        private Random myRand;
        private DateTime myTimeCreated;
        private int myTimeFromBabyToHalf; //in minutes
        private int myTimeFromHalfToFull;
        private KeyboardState myKeyState;
        private KeyboardState myLastState;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="et"></param>
        public ChopTree(GameWorld gw, EntityType et)
            : base(gw, et)
        {
            myPlayer = EntityManager.getEntityManager(game).player;
            myRand = new Random(DateTime.Now.Second * 69 * DateTime.Now.Millisecond * (DateTime.Now.Millisecond / 2));
            myTimeCreated = DateTime.Now;

            myTimeFromBabyToHalf = 2;//minutes
            myTimeFromHalfToFull = 2;

        }


        public override void animate()
        {
            int diff = DateTime.Now.Minute - myTimeCreated.Minute;

            if (diff >= (myTimeFromBabyToHalf + myTimeFromHalfToFull))
            {
                animation = "full";
            }
            else if (diff >= myTimeFromBabyToHalf)
            {
                animation = "half";
            }
            else
            {
                animation = "baby";
            }

            base.animate();
        }
        /// <summary>
        /// Collide method
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {
            myKeyState = Keyboard.GetState();
            int randWood = myRand.Next(myRand.Next(1, 3), myRand.Next(3, 8));
            int randAcorn = myRand.Next(myRand.Next(1, 5), myRand.Next(5, 10));

            if (myKeyState.IsKeyDown(Keys.K) && !myLastState.IsKeyDown(Keys.K))
            {
                if (otherThing.isPlayer)
                {
                    if (myPlayer.inventory["Axe"] > 0)
                    {
                        if (animation == "full")
                        { 
                            myPlayer = EntityManager.getEntityManager(game).player;
                            myPlayer.inventory.AddItems("RawWood", randWood);
                            myPlayer.inventory.AddItems("Acorn", randAcorn);
                            GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText("Raw Wood x" + randWood, new Vector2(370, 300), Color.BurlyWood);
                            GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText("Acorn x" + randAcorn, new Vector2(370, 350), Color.BurlyWood);

                            //delete tree
                            gameWorld.removeEntity(this);
                        }
                        else
                        {
                            GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText("Tree must be fully grown to be cut down.", new Vector2(150, 300), Color.Red);
                        }
                    }
                    else
                    {
                        GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText("Go buy an Axe first!", new Vector2(370, 300), Color.Red);
                    }
                }
            }

            myLastState = myKeyState;
            //base.collide(otherThing, touching);
        }

        /// <summary>
        /// Handle Events
        /// </summary>
        public override void handleEvents()
        {
            while (eventList.Count > 0)
            {
                Event e = eventList[0];
                eventList.RemoveAt(0);
                if (e.type.Equals("Chop_Tree_Down"))
                {
                    int randWood = myRand.Next(myRand.Next(1, 3), myRand.Next(3, 8));
                    int randAcorn = myRand.Next(myRand.Next(1, 5), myRand.Next(5, 10));

                    myPlayer = EntityManager.getEntityManager(game).player;
                    myPlayer.inventory.AddItems("RawWood", randWood);
                    myPlayer.inventory.AddItems("Acorn", randAcorn);
                    GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText("Raw Wood x" + randWood, new Vector2(370, 300), Color.BurlyWood);
                    GUI.GUIManager.getGUIManager(game, game.Content).createFloatingText("Acorn x" + randAcorn, new Vector2(370, 350), Color.BurlyWood);

                    //delete tree
                    gameWorld.removeEntity(this);
                }
            }
        }
    }
}
