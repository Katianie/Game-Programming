using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Entities.Singletons;
using Entities.Player;
using GUI;
namespace Entities.World
{
    /// <summary>
    /// Represents an in game character.
    /// Most notably these have four way animation.
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class Character:Entity
    {
        bool vaccinated = false;
        private readonly QuestManager myQuestManager;
        private Quest newQuest;
        
        private String name;
        private readonly GameWorld gameworld;
        private readonly SoundManager sounds;

        private GUIManager myGUIManager;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="gw">Gameworld</param>
        /// <param name="et">Type derived from</param>
        public Character(GameWorld gw, EntityType et):base(gw,et)
        {
            gameworld = gw;
            sounds = SoundManager.createSoundManager(gw.getGame().Content);
            myQuestManager = QuestManager.createQuestManager();
            myGUIManager = GUIManager.getGUIManager(gw.game, gw.getGame().Content);
        }

        /// <summary>
        /// Collision override
        /// </summary>
        /// <param name="otherThing">Other thing</param>
        /// <param name="touching">Are they touching?</param>
        public override void collide(Entity otherThing, bool touching)
        {
            if (otherThing.isPlayer && InputManager.lastState.IsKeyDown(Keys.T))
            {
                if (ai != null)
                {

                    if (ai.entity.id.Equals("EcologyGuy") && myQuestManager.DoctorDone)
                    {

                        ai.playCurrentConversation();
                    }
                    else if (ai.entity.id.Equals("EcologyGuy") && !myQuestManager.DoctorDone)
                    {
                        sounds.addEvent(new Event(EventList.PlaySound, "playerDamage"));
                    }
                    else if (ai.entity.id.Equals("PoliceCheif") && myQuestManager.EcoGuyDone)
                    {

                        ai.playCurrentConversation();
                    }
                    else if (ai.entity.id.Equals("PoliceCheif") && !myQuestManager.EcoGuyDone)
                    {
                        sounds.addEvent(new Event(EventList.PlaySound, "playerDamage"));
                    }
                    else if (ai.entity.id.Equals("FireCheif") && myQuestManager.PoliceCheifDone)
                    {

                        ai.playCurrentConversation();
                    }
                    else if (ai.entity.id.Equals("FireCheif") && !myQuestManager.PoliceCheifDone)
                    {
                        sounds.addEvent(new Event(EventList.PlaySound, "playerDamage"));
                    }
                    else if (ai.entity.id.Equals("Doctor"))
                    {

                        ai.playCurrentConversation();
                    }
                   else
                       ai.playCurrentConversation();

                }
            }
        }

        /// <summary>
        /// Handle events override. 
        /// Entry point for all quest start events.
        /// </summary>
        public override void handleEvents()
        {
            handleTakeEvents();
            while(eventList.Count > 0)
            {
                Player.Player p = EntityManager.getEntityManager(game).player;
                Event e = eventList[0];
                int width = 100;
                int height = 50;

                eventList.RemoveAt(0);

                if (e.type.Equals(EventList.AcceptCurrentQuest))
                {
                    name = e._value.ToString();
                    //Add the quest to the quest log
                    newQuest = new Quest(this);
                    newQuest.QuestName = name;

                    if (name.Equals(QuestList.Vaccine))
                    {

                    }
                    else if (name.Equals(QuestList.Shoot))
                    {

                    }
                    else if (name.Equals(QuestList.Catch))
                    {
                        newQuest.QuestDetails = "The farmer needs you to place the given blocks in an efficient arrangement, in order to catch the most cotton. You can rotate any piece with Q and E keys. To place a block press B. When the timer runs out, all of the cotton that you caught will be given to you permanently. The cotton can be used for many different things such as creating bandages.";
                    }
                    else if (name.Equals(QuestList.Monkey))
                    {

                    }
                    else if (name.Equals("Soccer"))
                    {

                    }
                    else if (name.Equals(QuestList.Solar))
                    {

                    }
                    else if (name.Equals("EcoGuy2"))
                    {
                        newQuest.QuestDetails = "pit fall game lol";
                    }
                    else if (name.Equals("EcoGuy3"))
                    {

                    }
                    else if (name.Equals("Doctor1"))
                    {

                    }
                    else if (name.Equals("Doctor2"))
                    {
                        newQuest.QuestDetails = "blah";
                    }
                    else if (name.Equals("Doctor3"))
                    {

                    }
                    else if (name.Equals("Doctor4"))
                    {

                    }
                    else if (name.Equals("Tower"))//This is part of doctor 4
                    {

                    }
                    else if (name.Equals("PoliceCheif1"))
                    {

                    }
                    else if (name.Equals(QuestList.Switchboard))
                    {

                    }
                    else if (name.Equals("PoliceCheif3"))
                    {

                    }
                    else if (name.Equals("FireCheif1"))
                    {

                    }
                    else if (name.Equals("FireCheif2"))
                    {

                    }
                    else if (name.Equals("FireCheif3"))
                    {

                    }

                    //add the quest to the model and the view (MVC)
                    myQuestManager.addCurrentQuest(newQuest);

                    myGUIManager.GameWorldName = newQuest.QuestName;
                    
                    myGUIManager.addQuestLogItem(newQuest.QuestName, newQuest.QuestDetails, 
                                                 new Rectangle(myGUIManager.QuestLogMenu.BoundingRectangle.X + 50,
                                                               myGUIManager.QuestLogMenu.BoundingRectangle.Y + (myQuestManager.CurrentQuestList.Count * height),
                                                               width,
                                                               height));

                    newQuest.completedCallback = questCompletedCallback;
                    
                    p.activeQuest = newQuest;
                    String v = e._value as String;
                    if (EntityManager.getEntityManager(game).isWorld(v))
                    {
                        p.switchBody(v);                       
                    }                    
                }
                if (e.type.Equals(EventList.GiveItem))
                {
                    p.inventory.AddItems(e._value as String, 1);
                   // GUI.GUIManager.getGUIManager(game.Content).createFloatingText(,new Vector2(370,300),);
                    EntityManager.getEntityManager(game).pushAFloater("+1 " +(e._value as String),Color.Blue);
                }
                if (e.type.Equals(EventList.GiveItems))
                {
                    String[] strs = (e._value as String).Split(',');
                    p.inventory.AddItems(strs[0], int.Parse(strs[1]));
                    GUI.GUIManager.getGUIManager(game, game.Content).Money = int.Parse(strs[1]);
                    EntityManager.getEntityManager(game).pushAFloater("+" + strs[1] + " " + strs[0] + "s", Color.Blue);
                }

                if (e.type.Equals(EventList.Instruct))
                {
                    String s = e._value as String;
                    GUI.GUIManager.getGUIManager(game, game.Content).createInformationText(s, new Vector2(190, 80), Color.Black);
                }
                if (e.type.Equals("Vaccinate"))
                {
                    if (!vaccinated)
                    {
                        vaccinated = true;
                        color = Color.White;
                        p.vaccinate++;
                        SoundManager.createSoundManager(game.Content).addEvent(new Event(EventList.PlaySound,"ouch"));
                        if (p.activeQuest != null && p.activeQuest.QuestName == "Vaccine" && p.vaccinate == 10)
                        {
                            p.addEvent(new Event(EventList.QuestCompletedReturn, "Main"));
                        }
                        if (type.type ==  TypeOfThing.ENEMY)
                            type = EntityManager.getEntityManager(game).getType("PoorManCharacter") ;


                    }
                }
                if (e.type.Equals(EventList.QuestCompletedReturn))
                {
                    EntityManager em = EntityManager.getEntityManager(game);
                    String worldName = em.getCurrentGameWorld().name;
                    String worldToReturnTo = e._value as String;
                    if (newQuest != null)
                    {
                        myQuestManager.addCompletedQuest(newQuest);
                        myQuestManager.removeCurrentQuest(newQuest);
                   
                        newQuest.completedCallback(null); 
                    }
                    else
                    {
                        ai.nextConversation();                        
                    }

                    if (worldName != worldToReturnTo)
                    {
                        p.switchBody(worldToReturnTo);
                        em.reloadGameWorld(worldName);
                    }
                }

                if (e.type == EventList.JoinPolice)
                {
                    gameWorld.removeEntity(this);                   
                }
                if (e.type == EventList.DisplayShopItems)
                {
                    myGUIManager.displayShopMenu();
                }
                if (e.type == EventList.FloatText)
                {
                    String s = e._value as string;
                    EntityManager.getEntityManager(game).pushAFloater(s, Color.Wheat);
                }
             }
            //base.handleEvents();
        }


        private String handleTakeEvents()
        {
            Player.Player p = EntityManager.getEntityManager(game).player;
            List<Event> toRemove = new List<Event>();
            bool clearList = false;
            String itemName = string.Empty;
            foreach(Event e in eventList)
            {
                if (e.type.Equals(EventList.TakeItem))
                {
                    if (p.inventory[e._value as String] > 1);
                    else
                    {
                        sounds.addEvent(new Event(EventList.PlaySound, "squeal"));
                        clearList = true;
                        itemName = e._value as String;
                        break;
                    }
                    toRemove.Add(e);
                }
                if (e.type.Equals(EventList.TakeItems))
                {
                    String[] strs = (e._value as String).Split(',');
                    if (p.inventory[strs[0]] > int.Parse(strs[1]));
                    else
                    {
                        sounds.addEvent(new Event(EventList.PlaySound, "squeal"));
                        clearList = true;
                        itemName = strs[0] as String;
                        break;
                    }
                    toRemove.Add(e);
                }
            }
            if (clearList)
            {
                eventList.Clear();
                EntityManager.getEntityManager(game).pushAFloater("Not Enough " +itemName, Color.DarkBlue);
            }
            else
            {
                foreach (Event e in toRemove)
                {
                    if (e.type.Equals(EventList.TakeItem))
                    {
                        p.inventory.takeItems(e._value as String, 1);
                    }
                    if (e.type.Equals(EventList.TakeItems))
                    {
                        String[] strs = (e._value as String).Split(',');
                        p.inventory.takeItems(strs[0], int.Parse(strs[1]));
                    }
                }
                foreach (Event e in toRemove)
                {
                    eventList.Remove(e);
                }
            }
            return itemName;
        }

        /// <summary>
        /// Method called on the character once the last quest it assigned was finished.
        /// </summary>
        /// <param name="_object"></param>
        public virtual void questCompletedCallback(Object _object)
        {
           //Default as number collected
            if (_object != null)
            {
                int number = (int)_object;
                Vector2 pos =my_Body.Position;
                pos *= 64;
                pos.X -= gameWorld.viewport.X;
                pos.Y -= gameWorld.viewport.Y;
                EntityManager.getEntityManager(game).pushAFloater("+" + number, Color.DarkBlue);
            }
            else
            {
                //GUI.GUIManager.getGUIManager(game.Content).createFloatingText("Thank You", new Vector2(300, 300), Color.DarkBlue);
                EntityManager.getEntityManager(game).pushAFloater("Thank You", Color.DarkBlue);
            }

            myQuestManager.addCompletedQuest(newQuest);

            /*if (newQuest.QuestName.Equals("Doctor2"))
            {
                int addSyringe = 3;
                myQuestManager.myInventory.AddItems("Syringe", addSyringe);
                GUI.GUIManager.getGUIManager(game.Content).createFloatingText("+" +  addSyringe + " Syringe", new Vector2(300, 300), Color.DarkBlue);
            }*/


            if (newQuest.QuestName.Equals(QuestList.Drugs))
            {
                EntityManager.NextGuy = EntityManager.getEntityManager(game).worlds["Main"].bodyDict["EcologyGuy"];
                myQuestManager.DoctorDone = true;
            }

            if (newQuest.QuestName.Equals(QuestList.Pitfall))
            {
                EntityManager.NextGuy = EntityManager.getEntityManager(game).worlds["Main"].bodyDict["PoliceCheif"];
                myQuestManager.EcoGuyDone = true;
            }

            if (newQuest.QuestName.Equals(QuestList.Switchboard))
            {
               // EntityManager.NextGuy = EntityManager.getEntityManager(game).worlds["Main"].bodyDict["FireCheif"];
                myQuestManager.PoliceCheifDone = true;
            }

            //if (newQuest.QuestName.Equals("FireCheif4"))
            //{
            //    EntityManager.NextGuy = null;
            //    myQuestManager.FireCheifDone = true;
            //}
            ai.nextConversation();
            
            ai.stopUpdating = false;
            
        }

        /// <summary>
        /// Four way animation.
        /// </summary>
        public override void animate()
        {
            Vector2 newVelocity = my_Body.GetLinearVelocity();
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
            base.animate();
        }
    }
}
