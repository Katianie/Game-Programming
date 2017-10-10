namespace Entities.World
{
    /// <summary>
    /// This represents an item or a discrete group of items as it against in the gameworld.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class Item : Entity
    {
        public readonly string itemType = "Something";
        public readonly int count;

        ///// <summary>
        ///// Constructor
        ///// </summary>
        ///// <param name="gw">Gameworld reference</param>
        ///// <param name="et">EntityType being used</param>
        //public Item(GameWorld gw, EntityType et)
        //    : base(gw, et)
        //{
        //    currentFrame = type.images.ToArray()[0];
        //    lastAnimation = animation;
        //    itemType = "Ball";
        //    count = 0;
        //}

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw">Gameworld reference</param>
        /// <param name="et">EntityType being used</param>
        /// <param name="_type">Type of item.  (This is what is used in the inventory it should be the EntityType name w/o the Item suffix)</param>
        public Item(GameWorld gw, EntityType et, string _type)
            : base(gw, et)
        {
            currentFrame = type.images.ToArray()[0];
            lastAnimation = animation;
            itemType = _type;
            count = 1;
        }

        /// <summary>
        /// Constructor for when a group is being represented by one sprite.
        /// </summary>
        /// <param name="gw">Gameworld reference</param>
        /// <param name="et">EntityType being used</param>
        /// <param name="_type">Type of item.  (This is what is used in the inventory it should be the EntityType name w/o the Item suffix)</param>
        /// <param name="_count">Amount of items.</param>
        public Item(GameWorld gw, EntityType et, string _type, int _count)
            : base(gw, et)
        {
            currentFrame = type.images.ToArray()[0];
            lastAnimation = animation;
            itemType = _type;
            count = _count;
        }


        /// <summary>
        /// Handle events.
        /// </summary>
        public override void handleEvents()
        {
            while (eventList.Count > 0)
            {
                Event e = eventList[0];
                eventList.RemoveAt(0);
                if (e.type.Equals("Kill"))
                {

                }
            }
        }

        /// <summary>
        /// Collision response method
        /// </summary>
        /// <param name="otherThing">Other entity</param>
        public override void collide(Entity otherThing, bool touching)
        {
            if (otherThing.isPlayer)
            {
                switch (itemType)
                {
                    case "Ball":
                        {
                            //my_Body.SetActive(false);
                            //my_Body.Position = new Vector2(-10, -10);
                            //otherThing.addEvent(new Event(EventType.PICK_UP_MONEY, 1));
                            break;
                        }
                }

            }
        }


        /// <summary>
        /// Should collide method.  Only the player can collide with items.
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        public override bool shouldCollide(Entity ent)
        {
            if (type.type == TypeOfThing.ITEM2 && (ent.type.type == TypeOfThing.ITEM2|| ent.type.type == TypeOfThing.WALL))
                return true;

            if (ent.isPlayer)
                return true;
            else
                return false;
        }
    }
}
