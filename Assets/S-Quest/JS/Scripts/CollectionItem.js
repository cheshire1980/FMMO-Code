/*

Collection Item Script Created by Oussama Bouanani (SoumiDelRio).

*/

//This script is used to identify items that the player has to collect.

var Name : String; //Item name.
var Manager : QuestManager;


function Awake ()
{
    Manager = FindObjectOfType(QuestManager);
}


function OnMouseDown ()
{
    //Collection Quest:
    //Search for all quests.
    var Quests : Quest[];
    Quests = FindObjectsOfType(Quest);
    //Loop through all the quests.
    for(var i : Quest in Quests)
    {
        if(i.QuestActive == true && i.Goal.Length > 0) //Check if the current quest is active.
        {
            if(i.Goal[i.Progress].Achieved == false && i.Goal[i.Progress].Type == QuestTypes.Collection) //If the goal is not achieved and we are dealing with a collection quest.
            {
                var Distance: float;
                Distance = Vector3.Distance(gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
                if(Distance > Manager.MinDistance) //Check if that distance is higher than the minimum distance required to collect the item.
	            {
	                return; //Stop here.
	            }
                else if(i.Goal[i.Progress].Amount < i.Goal[i.Progress].AmountRequired) //If we still didn't achieve the required amount of this item.
                {
                    //Destroy the object.
                    Destroy(this.gameObject);
                    i.Goal[i.Progress].Amount++;
                }
            }
        }
    }
}

function Update ()
{
	if(Manager.ControlType == ControlTypes.Keyboard || Manager.ControlType == ControlTypes.MouseAndKeyboard) //Keyboard controls:
	{
		var Distance : float;
		Distance = Vector3.Distance(gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
		if(Distance < Manager.MinDistance) //Check if that distance is lower than the minimum distance required to collect the item.
		{
			if(Input.GetKeyDown(Manager.CollectKey)) //Player presses the pick up key.
			{
				//Collection Quest:
				//Search for all quests.
				var Quests : Quest[] ;
				//Loop through all the quests.
				for(var i : Quest in Quests)
				{
					if(i.QuestActive == true && i.Goal.Length > 0) //Check if the current quest is active.
					{
						if(i.Goal[i.Progress].Amount < i.Goal[i.Progress].AmountRequired) //If we still didn't achieve the required amount of this item.
						{
							//Destroy the object.	
							Destroy(this.gameObject);
							i.Goal[i.Progress].Amount++;
						}
					}
				}
			}
		}
	}
}