/*

Elimination Item Script Created by Oussama Bouanani (SoumiDelRio).
This is just an example. You can copy some of the code here then paste it in your own combat/elimination scripts.

*/

using UnityEngine;
using System.Collections;

public class EliminationItemC : MonoBehaviour {
	
	//This script is used to identify items that the player has to eliminate.
	
	public string Name; //Item name.
	QuestManagerC Manager;
	
	
	void  Awake (){
		Manager = FindObjectOfType(typeof(QuestManagerC)) as QuestManagerC;
	}
	
	
	void  OnMouseDown (){
		
		if(Manager.ControlType == QuestManagerC.ControlTypes.Mouse || Manager.ControlType == QuestManagerC.ControlTypes.MouseAndKeyboard) //Mouse controls.
		{
			//Elimination Quest:
			//Search for all quests.
			QuestC[] Quests;
			Quests = FindObjectsOfType(typeof(QuestC)) as QuestC[];
			//Loop through all the quests.
			foreach(QuestC i in Quests)
			{
				if(i.QuestActive == true && i.Goal.Length > 0) //Check if the current quest is active.
				{
					if(i.Goal[i.Progress].Achieved == false && i.Goal[i.Progress].Type == QuestC.QuestTypes.Elimination) //If the goal is not achieved and we are dealing with a elimination quest.
					{
						float Distance;
						Distance = Vector3.Distance(gameObject.transform.position, Manager.Player.transform.position); //Get the current distance between the player and the item.
						if(Distance > Manager.MinDistance) //Check if that distance is higher than the minimum distance required to eliminate the item.
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
			if(Distance < Manager.MinDistance) //Check if that distance is lower than the minimum distance required to eliminate the item.
			{
				if(Input.GetKeyDown(Manager.EliminateKey)) //Player presses the pick up key.
				{
					//Elimination Quest:
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