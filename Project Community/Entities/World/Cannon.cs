using System;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Entities.Singletons;
using Microsoft.Xna.Framework.Graphics;


namespace Entities.World
{
    /// <summary>
    /// Represents a chop able tree
    /// </summary>
    /// <Owner>Edward Francis Katianie O'Hagan</Owner>
    public class Cannon : Entity
    {
        private Player.Player myPlayer;
        private KeyboardState myKeyState;
        private KeyboardState myLastState;
        private float myRotationAngle;
        private GameWorld myGameWorld;
        private Vector2 velocity;

        bool once = false;
        int buffer = 3;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw"></param>
        /// <param name="et"></param>
        public Cannon(GameWorld gw, EntityType et)
            : base(gw, et)
        {
            myPlayer = EntityManager.getEntityManager(game).player;
            myGameWorld = gw;
            velocity = new Vector2(0, 0);
        }


        public override void animate()
        {
            if (once == false)//dirty init
            {
                this.origin = new Vector2(100, 25);
                my_Body.Position = new Vector2(my_Body.Position.X + 1.75f, my_Body.Position.Y + 0.70f);
                myRotationAngle = 0.0f;
                EntityManager.getEntityManager(myGameWorld.game).player.my_Body.Position = new Vector2(1, 1920);
                once = true;
            }

            this.my_Body.Rotation += myRotationAngle;

            base.animate();
        }


        /// <summary>
        /// Collide method
        /// </summary>
        /// <param name="otherThing"></param>
        /// <param name="touching"></param>
        public override void collide(Entity otherThing, bool touching)
        {

            if (otherThing.isPlayer)
            {
                if (!myPlayer.myHasLaunched)
                {
                    myPlayer.myRestrictMovement = true;
                }
            }
            base.collide(otherThing, touching);
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

                if (buffer <= 0)
                {
                    buffer = 3;
                    if (e.type.Equals("Rotate_Cannon_CCW"))
                    {
                        if (this.my_Body.Rotation > -0.9)
                        {
                            myRotationAngle -= 0.05f;
                        }
                    }
                    else if (e.type.Equals("Rotate_Cannon_CW"))
                    {
                        if (this.my_Body.Rotation < 0.5)
                        {
                            myRotationAngle += 0.05f;
                        }
                    }
                }
                else
                {
                    buffer--;
                    myRotationAngle = 0.0f;
                }

                if (e.type.Equals("launch"))
                {
                    int pow = base.PowerBar.CurrentPower;
                    double x = ((pow) * (Math.Cos(-this.my_Body.Rotation)));
                    double y = ((pow) * -(Math.Sin(-this.my_Body.Rotation)));
                    myPlayer.myRestrictMovement = false;
                    myPlayer.my_Body.Position = new Vector2((float)(myPlayer.my_Body.Position.X + 1 + Math.Cos(-this.my_Body.Rotation)) ,(float)(myPlayer.my_Body.Position.Y + -Math.Sin(-this.my_Body.Rotation)));
                    //"Angle:" + this.my_Body.Rotation + 
                    GUI.GUIManager.getGUIManager(myGameWorld.game, myGameWorld.game.Content).createFloatingText("X:" + x + "Y:" + y, new Vector2(50, 300), Color.Red);
                    velocity = new Vector2((float)x + 300, (float)y);
                    myPlayer.my_Body.Rotation = MathHelper.ToRadians(90);
                    myPlayer.myHasLaunched = true;
                    myPlayer.my_Body.SetLinearVelocity(velocity);
                    
                }

            }
        }

    }
}
