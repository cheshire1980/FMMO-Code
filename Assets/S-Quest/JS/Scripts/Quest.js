/*

Quest Script Created by Oussama Bouanani (SoumiDelRio).

*/

//Start quest when the game starts:
var ActivateOnStart : boolean = false;

//Quest Info:
var QuestCode : String; //Quest's code.
var QuestOrder : int = 1; //This can't be negative or null. If it's 1 it'll be available for the player to do.
//The order is used to make a progress of quests. Use the same code and different order to make a story.
//And use other codes and 1 as the order to make optinal activities.
var QuestTitle : String; //Title of the quest.
var QuestObjective : String; //Please keep it short. Required. 
var Description : String; //This is used for detailed explanation.
var FinishedQuestMessage : String; //This is the message that the player sees when he talks to the quest giver after finishing the quest.
var RewardPlayer : boolean = true; //Reward the player when he finishes the quest?
var XP : int = 50; //How many experience points to give the player when he finishes the quest?
@HideInInspector
var QuestActive : boolean = false; //Is the player doing the quest now?
@HideInInspector
var QuestFinished : boolean = false; //Has the player done the quest?
@HideInInspector
var IsAvailable : boolean = false; //Is the quest available for player to do?
@HideInInspector
var ReturningToGiver : boolean = false; //Are we returning to the quest giver after completing the quest.
@HideInInspector
var Meeting : boolean = false; //This is true if the quest giver is expecting someone to meet him.

//QuestGiver:
var QuestGiverName : String; //You can have a quest giver if you would like to.
var QuestGiverSprite : Sprite; //Leave this blank if you don't want an image. 
var QuestGiverImage : Texture2D; //Leave this blank if you don't want an image.

@HideInInspector
var QuestOpen : boolean = false;
var Speech : AudioClip;

public enum QuestTypes {Goto = 0, Collection = 1, Meeting = 2, Elimination = 3} //Quest's types.
//Quest Objective:
class Objective
{
    var Type = QuestTypes.Goto & QuestTypes.Collection & QuestTypes.Meeting & QuestTypes.Elimination;
    var Message : String;
    var Icon : Texture2D;
    @HideInInspector
    var Show : boolean = false;
    
    //Goto quest type vars:
    var Destination : Vector3;
    var TimeToStay : float = 0;
    @HideInInspector
    var Timer : float = 0;
    var Range : float = 1;
    
    //Meeting quest type vars:
    var Target : Quest;
    var ArrivalMessage : String;
    
    //Collecting/Elimination quest type vars:
    var ItemName : String;
    var AmountRequired : int = 5;
    @HideInInspector
    var Amount : int = 0;
    
    @HideInInspector
    var Achieved : boolean = false;
}
var Goal : Objective[];
var ReturnToGiver : boolean = true;
var ShowAfterFinished : boolean = false; //Please don't tick this if the quest giver has more quests for the player.
@HideInInspector
var Progress : int = 0; //Player's progress in the quest.


//Meeting another quest giver:
@HideInInspector
var MeetingProgress : int; 
@HideInInspector
var Sender : Quest;

@HideInInspector
var MyTransform : Transform;

//Scripts:
@HideInInspector
var Manager : QuestManager;
@HideInInspector
var XPManager : ExperienceManager;
@HideInInspector
var InvQuest : InventoryQuest;
@HideInInspector
var QuestUI : QuestUIManager;


//Reward a player with items:
var Item : GameObject;


function Awake () 
{
    if(QuestCode == "")
    {
        Debug.LogError("S-Quest Error: Quest desactivated - Reason: Please give the quest a code.");
        gameObject.active = false;
    }
    if(QuestOrder <= 0)
    {
        QuestOrder = 1;
    }
    if(QuestObjective == "")
    {
        Debug.LogError("S-Quest Error: Quest desactivated - Reason: The quest objective is required.");
        gameObject.active = false;
    }
    if(Goal.Length < 1)
    {
        Debug.LogError("S-Quest Error: Quest desactivated - Reason: Your quest must have at least one goal.");
        gameObject.active = false;
    }
    
    
    Manager = FindObjectOfType(QuestManager);
    XPManager = FindObjectOfType(ExperienceManager);
    InvQuest = FindObjectOfType(InventoryQuest);
    QuestUI = FindObjectOfType(QuestUIManager);
    
    if(Manager.SaveAndLoad == true)
    {
        if(PlayerPrefs.GetInt(QuestCode+QuestOrder.ToString()) == 1)
        {
            QuestFinished = true;

            //Make the next quest available for the player if it's not already finished.
            var Quests : Quest[] = FindObjectsOfType(Quest) as Quest[];
		    for(var NextQuest : Quest in Quests) 
		    {
		        if(NextQuest.QuestCode == QuestCode)
		        {
		            if(NextQuest.QuestOrder == QuestOrder+1 && NextQuest.QuestFinished == false)
		            {
		                NextQuest.IsAvailable = true;
		            }
		        }
	        }
        }
    }
    
    
    MyTransform = gameObject.transform; //Set the object tranform.
    QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
    QuestOpen = false;
    Progress = 0;
    
    if(QuestOrder == 1)
    {
        IsAvailable = true;
    }
    else
    {
        IsAvailable = false;
    }
    
    if(QuestFinished == false && ActivateOnStart == true)
    {
        QuestActive = true; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
    }
}


function OnMouseDown()
{
    //If the quest isn't available and nobody sent the player to meet this quest giver then don't show the quest.
    if(IsAvailable == false && Meeting == false)
    {
        return;
    }
    //If we choose not to show anything after finishing the quest and the quest is already finished, return;
    if(ShowAfterFinished == false && QuestFinished == true)
    {
        return; 
    }
    
    var Distance: float;
    Distance = Vector3.Distance(MyTransform.position, Manager.Player.transform.position); //Get the current distance between the player and the quest holder.
	if(Distance <= Manager.MinDistance && QuestOpen == false && Manager.ControlType == 0 || Manager.ControlType == 3) //Check if that distance is below or equal to the minimum distance.
	{
			//Open the quest and play an audio if it exists.
			QuestOpen = true;
			QuestUI.ActiveQuest = this; QuestUI.OpenQuest();
	}
}

function Update () 
{
    //If the quest isn't available and nobody sent the player to meet this quest giver then don't show the quest.
    if(IsAvailable == false && Meeting == false)
    {
        return;
    }
    //If we choose not to show anything after finishing the quest and the quest is already finished, return;
    if(ShowAfterFinished == false && QuestFinished == true)
    {
        return; 
    }
    
    if(QuestOpen == true) //If the quest is already open:
    {
        var Distance2: float;
        Distance2 = Vector3.Distance(MyTransform.position, Manager.Player.transform.position); //Get the current distance between the player and the quest holder.
        if(Distance2 > Manager.MinDistance) //Check if the distance is no longer below or equal to the minimum distance required to open the quest.
        {
            //The player moves away so we close the quest.
            QuestOpen = false;
            QuestUI.CloseQuest();
        }
        if(Distance2 < Manager.MinDistance && Manager.ControlType == 1 || Manager.ControlType == 3)//Check if the player is in range of the quest giver.
		{
			if(Input.GetKeyDown(Manager.TogQuest)) //Checking if the player pressed the key used to open/close the quest.
			{
				//Open the quest or close it and play an audio if it exists.
				QuestOpen = !QuestOpen;
				if(QuestOpen == true) QuestUI.ActiveQuest = this; QuestUI.OpenQuest();
				if(QuestOpen == false) QuestUI.CloseQuest();
			}
		}
    }
        //Controlling the quest by keyboard:
        if(QuestOpen == true) //If the quest is active.
		{
			if(Manager.ControlType == 1 || Manager.ControlType == 3) //If we are controlling the quest with the keyboard/joystick.
			{
				if(Meeting == true)
				{
					if(Input.GetKey(Manager.AcceptQuest)) 
					{
						FinishMeetingQuest();
					}
				}
				//If we are returning to the quest giver after finishing the quest.
				else if(ReturningToGiver == true)
				{
					if(Input.GetKey(Manager.AcceptQuest)) 
					{
						FinishQuest();
					}
				}
				//If the quest is already finished.
				else if(QuestFinished == true)
				{
					if(Input.GetKey(Manager.AcceptQuest) || Input.GetKey(Manager.AbandonQuest)) 
					{
						QuestOpen = false;
						QuestUI.CloseQuest();
					}
				}
				else if(QuestActive == false) //If the quest is still active.
				{
					if(Input.GetKey(Manager.AcceptQuest)) 
					{
						AcceptQuest ();    
					}
				}  
				else if(QuestActive == true) //If the quest is inactive.
				{
					if(Input.GetKey(Manager.AbandonQuest)) 
					{
						AbandonQuest ();
					}
				}
			}
		}
    
    
    if(QuestActive == true) //If the quest is active.
    {
        if(Goal[Progress].Achieved == false) //If the goal is not achieved?
        {
            if(Goal[Progress].Type == QuestTypes.Goto) //If the type is a quest requires going to a destination.
            {
                Goal[Progress].Timer -= Time.deltaTime; //Time required to stay in the destination in a goto goal. 
                if(Goal[Progress].Timer < 0) Goal[Progress].Timer = 0;
                if(IsObjectInRangeOfPosition(Manager.Player.transform, Goal[Progress].Destination, Goal[Progress].Range) == true) //If the player is in range of the destination.
                {
                    if(Goal[Progress].Timer == 0) //If we finished the time required to stay.
                    {
                        Goal[Progress].Achieved = true;
                    } 
                }
                else
                {
                    Goal[Progress].Timer = Goal[Progress].TimeToStay; //Reset the timer if the player moves away from the destination.
                }
            }    
            else if(Goal[Progress].Type == QuestTypes.Meeting) //If the current quest type is a meeting.
            {
                Goal[Progress].Target.Sender = this;
                Goal[Progress].Target.MeetingProgress = Progress;
                Goal[Progress].Target.Meeting = true;
            }
            else if(Goal[Progress].Type == QuestTypes.Collection || Goal[Progress].Type == QuestTypes.Elimination) //If the current quest type is collecting something or eliminating something.
            {
                if(Goal[Progress].Amount == Goal[Progress].AmountRequired) //If the player collected/eliminated the required amount.
                {
                    Goal[Progress].Achieved = true;
                }
            }
        }
        else //If the goal is achieved.
        {
            
            //Check if finished the quest or not.
            if(Goal.Length <= Progress+1)
            {
                if(ReturnToGiver == true) //If we have to get back to the quest giver to end the 
                {
                    ReturningToGiver = true;
                }
                else //Else, we finish the quest here. :D
                {
                    if(Manager.CompleteQuestSound) Manager.audio.PlayOneShot(Manager.CompleteQuestSound);

	                if(Manager.SaveAndLoad == true)
	                {
	                    PlayerPrefs.SetInt(QuestCode+QuestOrder.ToString(), 1);
	                }
	                
	                //Reward the player with XP.
                    if(RewardPlayer == true)
                    {
                        XPManager.AddXP(XP);
                        
                        if(Item != null)
                        {
                            InvQuest.Reward(Item); 
                        }
                    }
	                
	                QuestFinished = true; NextQuest();
                    QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests(); 
                    Manager.Amount--;
	         	
                }    
            }
            else //If we didn't, then move to the next goal.
            {
                Progress++;
            }
        }
    }
}

public function FinishMeetingQuest () //called when your meeting type quest has finished.
{
    if(Manager.CompleteQuestSound) Manager.audio.PlayOneShot(Manager.CompleteQuestSound);
	if(Manager.SaveAndLoad == true)
	{
	    PlayerPrefs.SetInt(QuestCode+QuestOrder.ToString(), 1);
	}
	    
    Sender.ReturnToGiver = false;
    Sender.Goal[MeetingProgress].Achieved = true;
    QuestOpen = false;
    Meeting = false;
}

public function FinishQuest () //called when the whole quest was finished.
{
    if(Manager.CompleteQuestSound) Manager.audio.PlayOneShot(Manager.CompleteQuestSound);
    if(Manager.SaveAndLoad == true)
	{
	    PlayerPrefs.SetInt(QuestCode+QuestOrder.ToString(), 1);
	}
	    
	//Reward the player with XP.
    if(RewardPlayer == true)
    {
        XPManager.AddXP(XP);
        
            if(Item != null)
            {
                InvQuest.Reward(Item); 
            }
    }
	            
    QuestOpen = false;
    ReturningToGiver = false;
    QuestFinished = true; NextQuest();
    QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests(); 
    Manager.Amount--;
}

public function AcceptQuest ()
{
    if(Manager.Amount < Manager.MaxQuests) //If we still have space for new quests.
    {
        //Accept the quest.
        QuestActive = true; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests(); 
        if(Manager.AcceptQuestSound) Manager.audio.PlayOneShot(Manager.AcceptQuestSound);
        QuestOpen = false;
        QuestUI.CloseQuest();
        Manager.Amount++;
    }
    else
    {
        //QUESTS FULL
    }
}

public function AbandonQuest ()
{
    //Show the info message:
	            //Abandon the quest.
    QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests(); 
    if(Manager.AbandonQuestSound) Manager.audio.PlayOneShot(Manager.AbandonQuestSound);
    QuestOpen = false;
    QuestUI.CloseQuest();
    Manager.Amount--;
}

//This function determines if two objects are in range of each other or not.
function IsObjectInRangeOfPosition(Obj : Transform, Pos : Vector3, Range : float)
{
   var Distance : float;
   Distance = Vector3.Distance(Obj.position, Pos); 
   if(Distance > Range) 
   {
       return false;
   }
   else
   {
       return true;
   }
}

//This function determines the text out put for the quest for both of the classic themes.
function GetQuestString ()
{
    var QuestString : String; 
    
    if(Meeting == true)
    {
        QuestString = Sender.Goal[MeetingProgress].ArrivalMessage;
    }
    else if(ReturningToGiver == true)
    {
        QuestString = "Objective Achieved:\n" + FinishedQuestMessage;
    }
    else if(QuestFinished == true)
    {
        QuestString = FinishedQuestMessage;
    }
    else
    {
        QuestString = Description + "\n" + "Objective:\n" + QuestObjective;
    } 
    
    
    return QuestString;
}

function NextQuest ()
{
    //Make the next quest available for the player.
    var Quests : Quest[] = FindObjectsOfType(Quest) as Quest[];
	for(var NextQuest : Quest in Quests) 
	{
	    if(NextQuest.QuestCode == QuestCode)
		{
		    if(NextQuest.QuestOrder == QuestOrder+1)
		    {
		        NextQuest.IsAvailable = true;
		    }
		}
	}
}