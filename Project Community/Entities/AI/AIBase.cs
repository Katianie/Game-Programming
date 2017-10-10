using System;
using System.Collections.Generic;
using System.Xml;
using Entities.World;
namespace Entities.AI
{
    /// <summary>
    /// Base class for all NPC's
    /// </summary>
    /// <Owner>Justin Dale</Owner>
    public class AIBase
    {
        /// <summary>
        /// Gameworld reference
        /// </summary>
        protected GameWorld gameWorld;
        public Entity entity;
        protected Conversation currentConversation;
        public String conversationName = "default";
        protected Dictionary<String, Conversation> conversationList;
        protected ConversationResponse[] conversationResponses;
        public bool stopUpdating = false;
        public String aiArgs { get; set; }
        protected static Random random = new Random(DateTime.Now.Millisecond);
        protected String voiceFont = "default";
        /// <summary>
        /// Base Constructor
        /// </summary>
        /// <param name="_gameWorld"></param>
        public AIBase(GameWorld _gameWorld,Entity e)
        {
            gameWorld = _gameWorld;
            entity = e;
            conversationList = new Dictionary<string, Conversation>();
           
        }

        /// <summary>
        /// Every AI should have an update
        /// </summary>
        public virtual void update(){}


        /// <summary>
        /// Every AI will have either its own specific state file or will use a generic type file.
        /// </summary>
        /// <param name="filename"></param>
        public virtual void load(String filename){
            XmlTextReader xmlReader = new XmlTextReader(filename);
            xmlReader.ReadToFollowing("FontToUse");
            voiceFont = xmlReader.GetAttribute("value");
           if( xmlReader.ReadToFollowing("ConversationFile"))
           {
                do{
                    String convoName = xmlReader.GetAttribute("name");
                    String nextName = xmlReader.GetAttribute("next");
                    Conversation c = new Conversation(this);
                    c = Conversation.loadConversation(xmlReader.ReadElementContentAsString(), this, nextName);
                    conversationList.Add(convoName,c);
                }while(xmlReader.ReadToNextSibling("ConversationFile"));
               currentConversation = conversationList["default"];
           }
        }

        /// <summary>
        /// Puts the current conversation up on the screen.
        /// </summary>
        public virtual void playCurrentConversation() {
            if (conversationList.Count == 0 || currentConversation == null)
                return;
            GUI.GUIManager.getGUIManager(gameWorld.game, gameWorld.game.Content).playConversation(currentConversation.Question, currentConversation.getResponseStringArray(), getResponse, voiceFont);
            conversationResponses = currentConversation.responses.ToArray();
            stopUpdating = true;
        }

        /// <summary>
        /// Callback for conversation responses
        /// </summary>
        /// <param name="i">index of response</param>
        public virtual void getResponse(int i)
        {

            GUI.GUIManager gm = GUI.GUIManager.getGUIManager(gameWorld.game, gameWorld.game.Content);
            if (conversationResponses[i].convo == null)
            {
                foreach (Event e in conversationResponses[i].eventsToSend)
                {
                    entity.addEvent(e);
                }
                gm.closeConversationMenu();
                stopUpdating = false;
            }
            else
            {
                Conversation c = conversationResponses[i].convo;
                gm.closeConversationMenu();
                gm.playConversation(c.Question, c.getResponseStringArray(), getResponse, voiceFont);
                conversationResponses = c.responses.ToArray();
            }
        }

        /// <summary>
        /// Called foreach contact involving this AI's entity.
        /// </summary>
        /// <param name="otherThing">Entity of other thing</param>
        /// <param name="isTouching">Are they touching?</param>
        public virtual void collide(Entity otherThing, bool isTouching)
        {

        }

        /// <summary>
        /// Tries to move to the next conversation.
        /// </summary>
        public virtual void nextConversation()
        {
            conversationName = currentConversation.next;
            if (!String.IsNullOrEmpty((currentConversation.next)))
                currentConversation = conversationList[currentConversation.next];
        }

        /// <summary>
        /// Init method. Called after the AI has been setup before the first frame.
        /// </summary>
        public virtual void init(){ }

        public void resetConvo()
        {
            currentConversation = conversationList[conversationName];
        }

     }
}
