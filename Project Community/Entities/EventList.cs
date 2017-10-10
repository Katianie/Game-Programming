using System;
namespace Entities
{
    /// <summary>
    /// List of valid in-game events
    /// Lets try to keep this in some sort of order
    /// </summary>
    public class EventList
    {
        #region Player Events
        public    const String PlaceBuilding = "Place_Building";
        public    const String AbsorbObjects = "ABSORB_OBJECTS";
        public    const String PlayerUp = "PLAYER_UP";
        public const String AcceptCurrentQuest = "Accept_Current_Quest";
        public const String ChopTreeDown = "Chop_Tree_Down";
        public const String RotateCannonCCW = "Rotate_Cannon_CCW";
        public const String RotateCannonCW = "Rotate_Cannon_CW";
        public const String DontRotateCannon = "Dont_Rotate_Cannon";
        public const String PlantTree = "Plant_Tree";
        public const String TakeItem = "TakeItem";
        public const String GiveItem = "GiveItem";
        public const String TakeItems = "TakeItems";
        public const String GiveItems = "GiveItems";
        public const String FireVaccine = "FireVaccine";
        public const String IncreaseVelocity = "IncreaseVelocity";
        public const String DecreaseVelocity = "DecreaseVelocity";
        public const String ExitShopItems = "ExitShopItems";
        public const String NotUp = "NotUp";
        public const String Instruct = "Instruct";
        public const String FloatText = "FloatText";
        public    const String PlayerDown = "PLAYER_DOWN";
        public    const String PlayerLeft = "PLAYER_LEFT";
        public    const String PlayerRight = "PLAYER_RIGHT";
        public    const String GoToPoint = "GO_TO_POINT";
        public    const String SwitchWorld = "SWITCH_WORLD";
        public    const String GrabObject = "GRAB_OBJECT";
        public    const String QuestCompletedReturn = "Quest_Completed_Return";
        public    const String ResetWorld = "ResetWorld";
        public const String ResetWorld_Checkpoint = "ResetWorld_Checkpoint";
        public const String DisplayInventory = "DisplayInventory";
        public const String DisplayQuestLog = "DiaplayQuestLog";
        public const String TogglePlayerGUI = "TogglePlayerGUI";
        public const String playerCannonBallLaunch = "launch";
        #endregion


        public    const String PlaySound = "PLAY_SOUND";
        public    const String ReplaySound = "REPLAY_SOUND";
        public    const String PauseSound = "PAUSE_SOUND";
        public    const String StopSound = "STOP_SOUND";

        public const String PlaySong = "PLAY_SONG";
        public const String PauseSong = "PAUSE_SONG";
        public const String StopSong = "STOP_SONG";

        public    const String ExitConversation = "Exit_Conversation";
        public    const String SetPlayerColor = "Set_Player_Color";
        public    const String GrabIRemainingtems = "Grab_Remaining_Items";
        public const String JoinPolice = "Join_Police";
        public const String DisplayShopItems = "Display_Shop_Items";
    }
}
