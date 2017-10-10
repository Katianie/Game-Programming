using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities.Player;
using System.Collections;
namespace Entities.Singletons
{
    /// <summary>
    /// 
    /// </summary>
    /// <Owner>A Carbon baised lifeform</Owner>
    public class QuestManager
    {
        private Boolean myDoctorDone;
        private Boolean myEcoGuyDone;
        private Boolean myPCheifDone;
        private Boolean myFCheifDone;
        private ArrayList myCompletedQuestList;
        private ArrayList myCurrentQuestList;
        public Inventory myInventory;

        //Used for singleton purposes
        private static QuestManager myQuestManager;

        public static QuestManager createQuestManager()
        {
            if (myQuestManager == null)
            {
                myQuestManager = new QuestManager();
            }

            return myQuestManager;
        }

        private QuestManager()
        {
            myDoctorDone = false;
            myEcoGuyDone = false;
            myPCheifDone = false;
            myFCheifDone = false;
#if DEBUG
            myDoctorDone = true;
            myEcoGuyDone = true;
            myPCheifDone = true;
            myFCheifDone = true;
#endif
            myCompletedQuestList = new ArrayList();
            myCurrentQuestList = new ArrayList();
            myInventory = new Inventory();

        }

        public ArrayList CurrentQuestList
        {
            get
            {
                return myCurrentQuestList;
            }
            set
            {
                myCurrentQuestList = value;
            }
        }

        public ArrayList CompletedQuestList
        {
            get
            {
                return myCompletedQuestList;
            }
            set
            {
                myCompletedQuestList = value;
            }
        }

        public Boolean DoctorDone
        {
            get
            {
                return myDoctorDone;
            }
            set
            {
                myDoctorDone = value;
            }
        }

        public Boolean EcoGuyDone
        {
            get
            {
                return myEcoGuyDone;
            }
            set
            {
                myEcoGuyDone = value;
            }
        }

        public Boolean PoliceCheifDone
        {
            get
            {
                return myPCheifDone;
            }
            set
            {
                myPCheifDone = value;
            }
        }

        public Boolean FireCheifDone
        {
            get
            {
                return myFCheifDone;
            }
            set
            {
                myFCheifDone = value;
            }
        }

        public void addCompletedQuest(Quest completedQuest)
        {
            myCompletedQuestList.Add(completedQuest);
        }

        public void removeCompletedQuest(Quest completedQuest)
        {
            myCompletedQuestList.Remove(completedQuest);
        }

        public void addCurrentQuest(Quest currentQuest)
        {
            myCurrentQuestList.Add(currentQuest);
        }

        public void removeCurrentQuest(Quest currentQuest)
        {
            myCurrentQuestList.Remove(currentQuest);
        }
    }
}
