/*

Collection Item Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

public class CollectionItemC : MonoBehaviour {
	
	//This script is used to identify items that the player has to collect.
	
	public string Name; //Item name.
	QuestManagerC Manager;
	
	
	void  Awake (){
		Manager = FindObjectOfType(typeof(QuestManagerC)) as QuestManagerC;
	}
	
	
	void  OnMouseDown (){

		if(Manager.ControlType == QuestManagerC.ControlTypes.Mouse || Manager.ControlType == QuestManagerC.ControlTypes.MouseAndKeyboard) //Mouse controls.
		{
			//Collection Quest:
			//Search for all quests.
			QuestC[] Quests;
			Quests = FindObjectsOfType(typeof(QuestC)) as QuestC[];
			//Loop through all the quests.
			foreach(QuestC i in Quests)
			{
				if(i.QuestActive == true && i.Goal.Length > 0) //Check if the current quest is active.
				{
					if(i.Goal[i.Progress].Achieved == false && i.Goal[i.Progress].Type == QuestC.QuestTypes.Collection) //If the goal is not achieved and we are dealing with a collection quest.
					{
						float Distance;
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
	}

	void Update ()
	{
		if(Manager.ControlType == QuestManagerC.ControlTypes.Keyboard || Manager.ControlType == QuestManagerC.ControlTypes.MouseAndKeyboard) //Keyboard controls:
		{
			float Distance;
			Distance = Vector3.Distance(gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
			if(Distance < Manager.MinDistance) //Check if that distance is lower than the minimum distance required to collect the item.
			{
				if(Input.GetKeyDown(Manager.CollectKey)) //Player presses the pick up key.
				{
					//Collection Quest:
					//Search for all quests.
					QuestC[] Quests;
					Quests = FindObjectsOfType(typeof(QuestC)) as QuestC[];
					//Loop through all the quests.
					foreach(QuestC i in Quests)
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
}