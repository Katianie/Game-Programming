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
    public class Powerup
    {
        private Rectangle myBoundingBox;
        private Texture2D myTexture;
        private String myType;
        private Boolean myIsVisible;
        private int mySpeed;

        public Powerup(Texture2D texture, String type)
        {
            myBoundingBox = new Rectangle(-100, -100, 30, 30);
            myTexture = texture;
            myType = type;
            myIsVisible = true;
            mySpeed = 5;
        }

        public Rectangle BoundingBox
        {
            get
            {
                return myBoundingBox;
            }
            set
            {
                myBoundingBox = value;
            }
        }

        public int X
        {
            get
            {
                return myBoundingBox.X;
            }
            set
            {
                myBoundingBox.X = value;
            }
        }

        public int Y
        {
            get
            {
                return myBoundingBox.Y;
            }
            set
            {
                myBoundingBox.Y = value;
            }
        }

        public String Type
        {
            get
            {
                return myType;
            }
            set
            {
                myType = value;
            }
        }

        public int Speed
        {
            get
            {
                return mySpeed;
            }
            set
            {
                mySpeed = value;
            }
        }

        public Boolean IsVisible
        {
            get
            {
                return myIsVisible;
            }
            set
            {
                myIsVisible = value;
            }
        }

        public void Update()
        {
            myBoundingBox.X -= mySpeed;
        }

        public void Draw(SpriteBatch theSpriteBatch)
        {
            if (myIsVisible)
            {
                theSpriteBatch.Draw(myTexture, myBoundingBox, Color.White);
            }
        }
    }
}
