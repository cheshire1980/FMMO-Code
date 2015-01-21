using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestManagerC))] 
public class QuestManagerCEditor : Editor {

		//Quest Manager custom editor:
	    public override void OnInspectorGUI() 
	    {
		    QuestManagerC MyTarget = (QuestManagerC) target;

			EditorGUIUtility.LookLikeInspector();
			EditorGUIUtility.LookLikeControls();
			EditorGUILayout.Space();
			
			//Quests settings:
			EditorGUILayout.LabelField("Main Player Object:");
	    	EditorGUIUtility.LookLikeInspector();
	    	MyTarget.Player = EditorGUILayout.ObjectField(MyTarget.Player, typeof(GameObject)) as GameObject;
	    	EditorGUIUtility.LookLikeControls();

		    //Control settings:
	    	MyTarget.ControlType = (QuestManagerC.ControlTypes)EditorGUILayout.EnumPopup("Quest Control Type:", MyTarget.ControlType);

		    
	     	EditorGUILayout.LabelField("Tog (Open/Close) Quest Key:");
	    	MyTarget.TogQuest = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.TogQuest);
		    EditorGUILayout.LabelField("Accept/Finish Quest Key:");
		    MyTarget.AcceptQuest = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.AcceptQuest);
	    	EditorGUILayout.LabelField("Abandon Quest Key:");
	    	MyTarget.AbandonQuest = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.AbandonQuest);
	    	EditorGUILayout.LabelField("Collect Item Key:");
	    	MyTarget.CollectKey = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.CollectKey);
		    EditorGUILayout.LabelField("Eliminate Item Key:");
	     	MyTarget.EliminateKey = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.EliminateKey);

			EditorGUILayout.LabelField("Minimum Distance Between Player And Quest Giver:");
			MyTarget.MinDistance = EditorGUILayout.FloatField("Distance:", MyTarget.MinDistance);
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			//Quest Log:
			EditorGUILayout.LabelField("Quest Log:");
			MyTarget.ShowQuestLog = EditorGUILayout.Toggle("Show Quest Log:", MyTarget.ShowQuestLog);
			if(MyTarget.ShowQuestLog == true)
			{
				MyTarget.MaxQuests = EditorGUILayout.IntField("Max Quests:", MyTarget.MaxQuests);
				EditorGUILayout.LabelField("Quest Log Key:");
			MyTarget.LogKey = (KeyCode)EditorGUILayout.EnumPopup("",MyTarget.LogKey);
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			//Objective Bar:
			EditorGUILayout.LabelField("Objectives Bar:");
			MyTarget.ShowObjectiveBar = EditorGUILayout.Toggle("Show Objective Bar:", MyTarget.ShowObjectiveBar);
	    	/*if(MyTarget.ShowObjectiveBar == true)
	    	{
			MyTarget.ProgressBar = EditorGUILayout.Toggle("Show Progress Bar:", MyTarget.ProgressBar);
		    }*/
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			//Audio:
			EditorGUILayout.LabelField("Sound When Player Accepts Quest:");
			EditorGUIUtility.LookLikeInspector();
			MyTarget.AcceptQuestSound = EditorGUILayout.ObjectField(MyTarget.AcceptQuestSound, typeof(AudioClip)) as AudioClip;
			EditorGUIUtility.LookLikeControls(); 
			
			EditorGUILayout.LabelField("Sound When Player Abandons Quest:");
			EditorGUIUtility.LookLikeInspector();
			MyTarget.AbandonQuestSound = EditorGUILayout.ObjectField(MyTarget.AbandonQuestSound, typeof(AudioClip)) as AudioClip;
			EditorGUIUtility.LookLikeControls(); 
			
			EditorGUILayout.LabelField("Sound When Player Completes Quest:");
			EditorGUIUtility.LookLikeInspector();
			MyTarget.CompleteQuestSound = EditorGUILayout.ObjectField(MyTarget.CompleteQuestSound, typeof(AudioClip)) as AudioClip;
			EditorGUIUtility.LookLikeControls(); 
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			MyTarget.SaveAndLoad = EditorGUILayout.Toggle("Save And Load:", MyTarget.SaveAndLoad);

		    if(GUILayout.Button("Reset All Quests (WARNING: will remove all player prefs)."))
		    {
			    PlayerPrefs.DeleteAll();
		    }
	}
}