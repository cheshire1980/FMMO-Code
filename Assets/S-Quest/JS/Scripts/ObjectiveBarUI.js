import UnityEngine.UI;

var ObjectivesText : Text;

//Scripts:
@HideInInspector
var Manager : QuestManager;

function Awake () 
{
     Manager = FindObjectOfType(QuestManager);
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

function Update ()
{
     RefreshObjectives();
}

public function RefreshObjectives()
{
    ObjectivesText.text = "";
    var TempText : String = "";
    //Search for all the active quests of the player.
    var Quests : Quest[] = FindObjectsOfType(Quest) as Quest[];
	var ActiveQuests : int = 0; //This variable will hold the amount of the current active quests.
	
	for(var Quest : Quest in Quests) 
	{
		if(Quest.QuestActive == true) //Check if the current quest is active.
		{
		    ActiveQuests++;
		    //Classic objectives bar style: using text.
		    //Let's add the color yellow for the quest title. but keep the color you chose for the rest.
		    TempText += Quest.QuestTitle+":\n";
		    //Determine the current objective of this quest.
		    var Goal : String = Quest.Goal[Quest.Progress].Message;
		    
		        //Calculating the distance if we are returning to the quest giver, collecting or eliminating items.
		        var Distance: float;
		        var Target : Vector3;
		        if(Quest.ReturningToGiver == true)
		        {
		            Target = Quest.transform.position;
		            Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
		        }
		        else if(Quest.Goal[Quest.Progress].Type == QuestTypes.Goto)
		        {
		            Target = Quest.Goal[Quest.Progress].Destination;
		            Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
		            Distance -= Quest.Goal[Quest.Progress].Range;
		        }
		        else if(Quest.Goal[Quest.Progress].Type == QuestTypes.Meeting)
		        {
		            Target = Quest.Goal[Quest.Progress].Target.transform.position;
		            Distance = Vector3.Distance(Target, Manager.Player.transform.position); //Get the current distance between the player and the target.
		        }
                var Int : int;
                Int = Mathf.CeilToInt(Distance);
                
		    if(Quest.ReturningToGiver == true)
		    {
		        Goal = "Return to " + Quest.QuestGiverName + ":  "+Int.ToString()+ " m left.";
		    }
		    else if(Quest.Goal[Quest.Progress].Type == QuestTypes.Collection || Quest.Goal[Quest.Progress].Type == QuestTypes.Elimination)
		    {
		        Goal += ": "+Quest.Goal[Quest.Progress].Amount.ToString()+"/"+Quest.Goal[Quest.Progress].AmountRequired.ToString();
		    }
		    else if(Quest.Goal[Quest.Progress].Type == QuestTypes.Goto || Quest.Goal[Quest.Progress].Type == QuestTypes.Meeting)
		    {
		        Goal += ": "+Int.ToString()+ " m left."; 
		    }
		    TempText += Goal+"\n";
		}
	}
	
	ObjectivesText.text = "Objectives("+ActiveQuests.ToString()+"):\n"+TempText;
}