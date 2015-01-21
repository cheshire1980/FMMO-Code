import UnityEngine.UI;

@CustomEditor (QuestManager)
class QuestManagerEditor extends Editor 
{
    //Quest Manager custom editor:
    function OnInspectorGUI () 
    {
        EditorGUIUtility.LookLikeInspector();
        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.Space();
        
        //Quests settings:
        EditorGUILayout.LabelField("Main Player Object:");
        EditorGUIUtility.LookLikeInspector();
		target.Player = EditorGUILayout.ObjectField(target.Player, typeof(GameObject)) as GameObject;
		EditorGUIUtility.LookLikeControls();
        
        //Control settings:
	    target.ControlType = EditorGUILayout.EnumPopup("Quest Control Type:", target.ControlType);
        
	    EditorGUILayout.LabelField("Tog (Open/Close) Quest Key:");
	    target.TogQuest = EditorGUILayout.EnumPopup("",target.TogQuest);
	    EditorGUILayout.LabelField("Accept/Finish Quest Key:");
        target.AcceptQuest = EditorGUILayout.EnumPopup("",target.AcceptQuest);
      	EditorGUILayout.LabelField("Abandon Quest Key:");
	    target.AbandonQuest = EditorGUILayout.EnumPopup("",target.AbandonQuest);
	    EditorGUILayout.LabelField("Collect Item Key:");
	    target.CollectKey = EditorGUILayout.EnumPopup("",target.CollectKey);
	    EditorGUILayout.LabelField("Eliminate Key:");
	    target.EliminateKey = EditorGUILayout.EnumPopup("",target.EliminateKey);
	    
        EditorGUILayout.LabelField("Minimum Distance Between Player And Quest Giver:");
        target.MinDistance = EditorGUILayout.FloatField("Distance:", target.MinDistance);
   
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //Quest Log:
        EditorGUILayout.LabelField("Quest Log:");
        target.ShowQuestLog = EditorGUILayout.Toggle("Show Quest Log:", target.ShowQuestLog);
        if(target.ShowQuestLog == true)
        {
            target.MaxQuests = EditorGUILayout.IntField("Max Quests:", target.MaxQuests);
            EditorGUILayout.LabelField("Quest Log Key:");
            target.LogKey = EditorGUILayout.EnumPopup(target.LogKey);
        }
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        //Objective Bar:
        EditorGUILayout.LabelField("Objectives Bar:");
        target.ShowObjectiveBar = EditorGUILayout.Toggle("Show Objective Bar:", target.ShowObjectiveBar);
        /*if(target.ShowObjectiveBar == true)
        {
            target.ProgressBar = EditorGUILayout.Toggle("Show Progress Bar:", target.ProgressBar);
        }*/
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //Audio:
        EditorGUILayout.LabelField("Sound When Player Accepts Quest:");
        EditorGUIUtility.LookLikeInspector();
        target.AcceptQuestSound = EditorGUILayout.ObjectField(target.AcceptQuestSound, typeof(AudioClip)) as AudioClip;
        EditorGUIUtility.LookLikeControls(); 
        
        EditorGUILayout.LabelField("Sound When Player Abandons Quest:");
        EditorGUIUtility.LookLikeInspector();
        target.AbandonQuestSound = EditorGUILayout.ObjectField(target.AbandonQuestSound, typeof(AudioClip)) as AudioClip;
        EditorGUIUtility.LookLikeControls(); 
        
        EditorGUILayout.LabelField("Sound When Player Completes Quest:");
        EditorGUIUtility.LookLikeInspector();
        target.CompleteQuestSound = EditorGUILayout.ObjectField(target.CompleteQuestSound, typeof(AudioClip)) as AudioClip;
        EditorGUIUtility.LookLikeControls(); 
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        target.SaveAndLoad = EditorGUILayout.Toggle("Save And Load:", target.SaveAndLoad);
         

        if(GUILayout.Button("Reset All Quests (WARNING: will remove all player prefs)."))
        {
            PlayerPrefs.DeleteAll();
        }
        
         
    }
}