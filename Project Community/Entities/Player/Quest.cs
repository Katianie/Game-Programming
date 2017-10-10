using System;

namespace Entities.Player
{
    /// <summary>
    /// Represents a single quest
    /// This is the Quest manager 
    /// status states are prevQuest, currQuest, inactiveQuests, activeQuests
    /// </summary>
    public class Quest
    {
       // private Boolean myIsCompleted;
        private String myQuestName;
        private String myQuestDetails;
        public World.Character character;
       

        public Quest(World.Character _c)
        {
            character = _c;
            myQuestName = "";
            myQuestDetails = "";
        }


        public String QuestName
        {
            get { return myQuestName; }
            set { myQuestName = value; }
        }

        public String QuestDetails
        {
            get { return myQuestDetails; }
            set { myQuestDetails = value; }
        }

        public delegate void ResponseMethod(object o);
        public ResponseMethod completedCallback;
    }
}
