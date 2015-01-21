// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ObjectiveBarUI : MonoBehaviour {

	public Text ObjectivesText;
	
	//Scripts:
	QuestManagerC Manager;
	
	void  Awake (){
		Manager = FindObjectOfType(typeof(QuestManagerC)) as QuestManagerC;
		if(Manager.ShowObjectiveBar == true)
		{
			ObjectivesText.gameObject.SetActive(true);
			RefreshObjectives();
		}
		else
		{
			ObjectivesText.gameObject.SetActive(false);
		}
	}

	void Update ()
	{
		RefreshObjectives ();
	}

	public void  RefreshObjectives (){
		ObjectivesText.text = "";
		string TempText = "";
		//Search for all the active quests of the player.
		QuestC[] Quests = FindObjectsOfType(typeof(QuestC)) as QuestC[];
		int ActiveQuests = 0; //This variable will hold the amount of the current active quests.
		
		foreach(QuestC Quest in Quests) 
		{
			if(Quest.QuestActive == true) //Check if the current quest is active.
			{
				ActiveQuests++;
				//Classic objectives bar style: using text.
				//Let's add the color yellow for the quest title. but keep the color you chose for the rest.
				TempText += Quest.QuestTitle+":\n";
				//Determine the current objective of this quest.
				string Goal = Quest.Goal[Quest.Progress].Message;
				
				//Calculating the distance if we are returning to the quest giver, collecting or eliminating items.
				float Distance = 0.0f;
				Vector3 Target;
				if(Quest.ReturningToGiver == true)
				{
					Target = Quest.transform.position;
					Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
				}
				else if(Quest.Goal[Quest.Progress].Type == QuestC.QuestTypes.Goto)
				{
					Target = Quest.Goal[Quest.Progress].Destination;
					Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
					Distance -= Quest.Goal[Quest.Progress].Range;
				}
				else if(Quest.Goal[Quest.Progress].Type == QuestC.QuestTypes.Meeting)
				{
					Target = Quest.Goal[Quest.Progress].Target.transform.position;
					Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
				}
				int Int;
				Int = Mathf.CeilToInt(Distance);
				
				if(Quest.ReturningToGiver == true)
				{
					Goal = "Return to " + Quest.QuestGiverName + ":  "+Int.ToString()+ " m left.";
				}
				else if(Quest.Goal[Quest.Progress].Type == QuestC.QuestTypes.Collection || Quest.Goal[Quest.Progress].Type == QuestC.QuestTypes.Elimination)
				{
					Goal += ": "+Quest.Goal[Quest.Progress].Amount.ToString()+"/"+Quest.Goal[Quest.Progress].AmountRequired.ToString();
				}
				else if(Quest.Goal[Quest.Progress].Type == QuestC.QuestTypes.Goto || Quest.Goal[Quest.Progress].Type == QuestC.QuestTypes.Meeting)
				{
					Goal += ": "+Int.ToString()+ " m left."; 
				}
				TempText += Goal+"\n";
			}
		}
		
		ObjectivesText.text = "Objectives("+ActiveQuests.ToString()+"):\n"+TempText;
	}
}