/*

Quest Manager Script Created by Oussama Bouanani (SoumiDelRio).

*/

using UnityEngine;
using System.Collections;

public class QuestManagerC : MonoBehaviour 
{
    
    public GameObject Player; //Main player.

	//Quest controls:
	public enum ControlTypes {Mouse = 0, Keyboard = 1, MouseAndKeyboard = 2}
	public ControlTypes ControlType = ControlTypes.Mouse & ControlTypes.Keyboard & ControlTypes.MouseAndKeyboard;
	public KeyCode TogQuest = KeyCode.Q;
	public KeyCode AcceptQuest = KeyCode.A;
	public KeyCode AbandonQuest = KeyCode.D;
	public KeyCode CollectKey = KeyCode.C;
	public KeyCode EliminateKey = KeyCode.E;

	public float MinDistance = 4.0f;
	
	public bool  ProgressBar = true; //Use a progress bar in the objective bar UI?
	
	
	//Quest Log:
	public int MaxQuests = 10; //Maximum number of active quests at one time.
	[HideInInspector]
	public int Amount = 0; //Current amount of active quests.
	[HideInInspector]
	public bool  LogOpen = false;
	public KeyCode LogKey= KeyCode.L; //Key used to show the Quest log.
	[HideInInspector]
	public float Distance;
	
	public bool  ShowObjectiveBar = true; //Show the Objective bar?
	public bool  ShowQuestLog = true; //Show the quest log or not?
	[HideInInspector]
	public QuestC TargetQuest = null;
	//GUI:
	[HideInInspector]
	public bool  QuestOpen = false;
	
	//Sounds:
	public AudioClip CompleteQuestSound;
	public AudioClip AcceptQuestSound;
	public AudioClip AbandonQuestSound;
	
	public bool  SaveAndLoad = false;

	//Scripts:
	[HideInInspector]
	public QuestLogUI LogUI;
	
	void  Awake ()
	{
		LogUI = FindObjectOfType (typeof(QuestLogUI)) as QuestLogUI;
		LogOpen = false;
	}
	
	void  Update (){
		if(Input.GetKeyDown(LogKey) && ShowQuestLog == true) //Show or hide the quest log.
		{
			LogOpen = !LogOpen;
			if(LogOpen == true) LogUI.OpenQuestLog();
			if(LogOpen == false) LogUI.CloseQuestLog();
		}
	}
}