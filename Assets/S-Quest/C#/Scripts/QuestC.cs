/*

Quest Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

public class QuestC : MonoBehaviour {


	//Quest Info:
	public string QuestCode; //Quest's code.
	public int QuestOrder = 1; //This can't be negative or null. If it's 1 it'll be available for the player to do.
	//The order is used to make a progress of quests. Use the same code and different order to make a story.
	//And use other codes and 1 as the order to make optinal activities.
	public string QuestTitle; //Title of the quest.
	public string QuestObjective; //Please keep it short. Required. 
	public string Description; //This is used for detailed explanation.
	public string FinishedQuestMessage; //This is the message that the player sees when he talks to the quest giver after finishing the quest.
	public bool  RewardPlayer = true; //Reward the player when he finishes the quest?
	public int XP = 50; //How many experience points to give the player when he finishes the quest?
	[HideInInspector]
	public bool  QuestActive = false;
	[HideInInspector]
	public bool  QuestFinished = false; //Has the player done the quest?
	[HideInInspector]
	public bool  IsAvailable = false; //Is the quest available for player to do?
	[HideInInspector]
	public bool  ReturningToGiver = false; //Are we returning to the quest giver after completing the quest.
	[HideInInspector]
	public bool  Meeting = false; //This is true if the quest giver is expecting someone to meet him.
	
	//QuestGiver:
	public string QuestGiverName; //You can have a quest giver if you would like to. 
	public Sprite QuestGiverSprite;
	public Texture2D QuestGiverImage; //Leave this blank if you don't want an image.
	//GUI:
	[HideInInspector]
	public bool  QuestOpen = false;
	public AudioClip Speech;

	public enum QuestTypes {Goto = 0, Collection = 1, Meeting = 2, Elimination = 3} //Quest's types.
	//Quest Objective:
	[System.Serializable]
	public class Objective
	{
		public QuestTypes Type= QuestTypes.Goto & QuestTypes.Collection & QuestTypes.Meeting & QuestTypes.Elimination;
		public string Message;
		public Texture2D Icon;
		[HideInInspector]
		public bool  Show = false;
		
		//Goto quest type vars:
		public Vector3 Destination;
		public float TimeToStay = 0;
		[HideInInspector]
		public float Timer = 0;
		public float Range = 1;
		
		//Meeting quest type vars:
		public QuestC Target;
		public string ArrivalMessage;
		
		//Collecting/Elimination quest type vars:
		public string ItemName;
		public int AmountRequired = 5;
		[HideInInspector]
		public int Amount = 0;
		
		[HideInInspector]
		public bool  Achieved = false;
	}
	public Objective[] Goal;
	public bool ReturnToGiver = true;
	public bool ShowAfterFinished = false; //Please don't tick this if the quest giver has more quests for the player.
	[HideInInspector]
	public int Progress = 0; //Player's progress in the quest.
	
	
	//Meeting another quest giver:
	[HideInInspector]
	public int MeetingProgress; 
	[HideInInspector]
	public QuestC Sender;
	
	[HideInInspector]
	public Transform MyTransform;
	
	//Scripts:
	[HideInInspector]
	public QuestManagerC Manager;
	[HideInInspector]
	public ExperienceManagerC XPManager;
	[HideInInspector]
	public InventoryQuest InvQuest;
	[HideInInspector]
	public QuestUIManager QuestUI;

	public GameObject Item;
	
	
	void  Awake ()
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
		
		
		Manager = FindObjectOfType(typeof(QuestManagerC)) as QuestManagerC;
		XPManager = FindObjectOfType(typeof(ExperienceManagerC)) as ExperienceManagerC;
		InvQuest = FindObjectOfType(typeof(InventoryQuest)) as InventoryQuest;
		QuestUI = FindObjectOfType (typeof(QuestUIManager)) as QuestUIManager;
		
		if(Manager.SaveAndLoad == true)
		{
			if(PlayerPrefs.GetInt(QuestCode+QuestOrder.ToString()) == 1)
			{
				QuestFinished = true;
				
				//Make the next quest available for the player if it's not already finished.
				QuestC[] Quests = FindObjectsOfType(typeof(QuestC)) as QuestC[];
				foreach(QuestC NextQuest in Quests) 
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
	}
	
	
	void  OnMouseDown ()
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
		
		float Distance;
		Distance = Vector3.Distance(MyTransform.position, Manager.Player.transform.position); //Get the current distance between the player and the quest holder.
		if(Distance <= Manager.MinDistance && QuestOpen == false && Manager.ControlType == QuestManagerC.ControlTypes.Mouse || Manager.ControlType == QuestManagerC.ControlTypes.MouseAndKeyboard) //Check if that distance is below or equal to the minimum distance.
		{
			    //Open the quest and play an audio if it exists.
		    	QuestOpen = true;
			    QuestUI.ActiveQuest = this; QuestUI.OpenQuest();
		} 
	}
	
	void  Update ()
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

		float Distance2;
		Distance2 = Vector3.Distance(MyTransform.position, Manager.Player.transform.position); //Get the current distance between the player and the quest holder.
		if(QuestOpen == true) //If the quest is already open:
		{
			if(Distance2 > Manager.MinDistance) //Check if the distance is no longer below or equal to the minimum distance required to open the quest.
			{
				//The player moves away so we close the quest.
				QuestOpen = false;
				QuestUI.CloseQuest();
			}
		}
		if(Distance2 < Manager.MinDistance && Manager.ControlType == QuestManagerC.ControlTypes.Keyboard || Manager.ControlType == QuestManagerC.ControlTypes.MouseAndKeyboard)//Check if the player is in range of the quest giver.
		{
			if(Input.GetKeyDown(Manager.TogQuest)) //Checking if the player pressed the key used to open/close the quest.
			{
				//Open the quest or close it and play an audio if it exists.
				QuestOpen = !QuestOpen;
				if(QuestOpen == true) QuestUI.ActiveQuest = this; QuestUI.OpenQuest();
				if(QuestOpen == false) QuestUI.CloseQuest();
			}
		}
		
		
		if(QuestOpen == true) //If the quest is active.
		{
			if(Manager.ControlType == QuestManagerC.ControlTypes.Keyboard || Manager.ControlType == QuestManagerC.ControlTypes.MouseAndKeyboard) //If we are controlling the quest with the keyboard/joystick.
			{
				if(Meeting == true)
				{
					FinishMeetingQuest();
				}
				//If we are returning to the quest giver after finishing the quest.
				else if(ReturningToGiver == true)
				{
					FinishQuest();
				}
				//If the quest is already finished.
				else if(QuestFinished == true)
				{
					if(Input.GetKey(Manager.AcceptQuest) || Input.GetKey(Manager.AbandonQuest)) 
					{
						QuestOpen = false;
					}
				}
				else if(QuestActive == false) //If the quest is still active.
				{
					AcceptQuest();
				}  
				else if(QuestActive == true) //If the quest is inactive.
				{
					AbandonQuest();
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

	public void FinishMeetingQuest () //called when your meeting type quest has finished.
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
	
	public void FinishQuest () //called when the whole quest was finished.
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
	
	public void  AcceptQuest (){
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
	
	public void  AbandonQuest (){
		//Show the info message:
		//Abandon the quest.
		QuestActive = false; if(Manager.LogOpen == true && Manager.ShowQuestLog == true) Manager.LogUI.LoadActiveQuests();
		if(Manager.AbandonQuestSound) Manager.audio.PlayOneShot(Manager.AbandonQuestSound);
		QuestOpen = false;
		QuestUI.CloseQuest();
		Manager.Amount--;
	}
	
	//This function determines if two objects are in range of each other or not.
	bool  IsObjectInRangeOfPosition (Transform Obj, Vector3 Pos, float Range)
	{
		float Distance;
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
	public string  GetQuestString ()
	{
		string QuestString; 
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

	public void NextQuest ()
	{
		//Make the next quest available for the player if it's not already finished.
		QuestC[] Quests = FindObjectsOfType(typeof(QuestC)) as QuestC[];
		foreach(QuestC NextQuest in Quests) 
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
