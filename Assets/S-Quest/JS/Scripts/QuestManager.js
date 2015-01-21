/*

Quest Manager Script Created by Oussama Bouanani (SoumiDelRio).

*/

var Player : GameObject; //Main player.

//Quest controls:
public enum ControlTypes {Mouse = 0, Keyboard = 1, MouseAndKeyboard = 2}
var ControlType = ControlTypes.Mouse & ControlTypes.Keyboard & ControlTypes.MouseAndKeyboard;
var TogQuest = KeyCode.Q;
var AcceptQuest = KeyCode.A;
var AbandonQuest = KeyCode.D;
var CollectKey = KeyCode.C;
var EliminateKey = KeyCode.E;

var MinDistance : float = 4; //Minimum distance required to show the quest info.

var ProgressBar : boolean = true; //Use a progress bar in the objective bar UI?


//Quest Log:
var MaxQuests : int = 10; //Maximum number of active quests at one time.
@HideInInspector
var Amount : int = 0; //Current amount of active quests.
@HideInInspector
var LogOpen : boolean = false;
var LogKey  = KeyCode.L; //Key used to show the Quest log.

var ShowObjectiveBar : boolean = true; //Show the Objective bar?
var ShowQuestLog : boolean = true; //Show the quest log or not?
@HideInInspector
var TargetQuest : Quest = null;

@HideInInspector
var QuestOpen : boolean = false;

//Sounds:
var CompleteQuestSound : AudioClip;
var AcceptQuestSound : AudioClip;
var AbandonQuestSound : AudioClip;

var SaveAndLoad : boolean = false;

//Scripts:
@HideInInspector
var LogUI : QuestLogUI;

function Awake () 
{
    LogUI = FindObjectOfType(QuestLogUI);
    LogOpen = false;
}

function Update () 
{
    if(Input.GetKeyDown(LogKey) && ShowQuestLog == true) //Show or hide the quest log.
    {
        LogOpen = !LogOpen;
        if(LogOpen == true) LogUI.OpenQuestLog();
        if(LogOpen == false) LogUI.CloseQuestLog();
    }
}