using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(QuestC))] 
public class QuestCEditor : Editor {

	public SerializedProperty Condition;
	public Rect MyRect;
	public bool showChildren;

		//Quest custom editor: makes creating quests easy and simple.
	    public override void OnInspectorGUI() 
	    {
		    QuestC MyTarget = (QuestC) target;

			EditorGUIUtility.LookLikeInspector();
			EditorGUIUtility.LookLikeControls();
			EditorGUILayout.Space();
			
			MyTarget.QuestCode = EditorGUILayout.TextField("Quest Code:", MyTarget.QuestCode);
			MyTarget.QuestOrder = EditorGUILayout.IntField("Quest Order:", MyTarget.QuestOrder);
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			MyTarget.QuestTitle = EditorGUILayout.TextField("Quest Title:", MyTarget.QuestTitle);
			EditorGUILayout.LabelField("Quest Objective (Short):");
			MyTarget.QuestObjective = EditorGUILayout.TextArea(MyTarget.QuestObjective, GUILayout.Height(30));
			EditorGUILayout.LabelField("Quest Description/Story (Long):"); 
			MyTarget.Description = EditorGUILayout.TextArea(MyTarget.Description, GUILayout.Height(80));
			MyTarget.ShowAfterFinished = EditorGUILayout.Toggle("Show When Completed:", MyTarget.ShowAfterFinished);
			if(MyTarget.ShowAfterFinished == true)
			{
				EditorGUILayout.LabelField("Completed Quest Message (Short):");
				MyTarget.FinishedQuestMessage = EditorGUILayout.TextArea(MyTarget.FinishedQuestMessage, GUILayout.Height(30));
			}    
			EditorGUILayout.LabelField("Speech Audio:");
			EditorGUIUtility.LookLikeInspector();
			MyTarget.Speech = EditorGUILayout.ObjectField(MyTarget.Speech, typeof(AudioClip)) as AudioClip;
			EditorGUIUtility.LookLikeControls();        
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			MyTarget.QuestGiverName = EditorGUILayout.TextField("Quest Giver Name:", MyTarget.QuestGiverName);
			EditorGUILayout.LabelField("Quest Giver Image:");
			EditorGUIUtility.LookLikeInspector();
			MyTarget.QuestGiverSprite = EditorGUILayout.ObjectField(MyTarget.QuestGiverSprite, typeof(Sprite)) as Sprite;
			EditorGUIUtility.LookLikeControls();
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			
		    Condition = serializedObject.FindProperty("Goal");
		 
		    Condition.arraySize = EditorGUILayout.IntField("Condition(s):", Condition.arraySize);
		    serializedObject.ApplyModifiedProperties();
            if(Condition.arraySize < 1) Condition.arraySize = 1; serializedObject.ApplyModifiedProperties();
			
			if(Condition.arraySize >= 1)
			{
				
				for(int i = 0; i < Condition.arraySize; i++)
				{
					int x=  i + 1;
					EditorGUILayout.LabelField("Condition " + x.ToString() + ":");
					EditorGUILayout.LabelField("Condition Type:");
					MyTarget.Goal[i].Type = (QuestC.QuestTypes)EditorGUILayout.EnumPopup("", MyTarget.Goal[i].Type);
					MyTarget.Goal[i].Message = EditorGUILayout.TextField("Condition Message: ", MyTarget.Goal[i].Message); 
			    	if(MyTarget.Goal[i].Type == QuestC.QuestTypes.Goto)
					{
						MyTarget.Goal[i].Destination = EditorGUILayout.Vector3Field("Destination:",MyTarget.Goal[i].Destination);
				    	MyTarget.Goal[i].Range = EditorGUILayout.FloatField("Range:",MyTarget.Goal[i].Range);
						MyTarget.Goal[i].TimeToStay = EditorGUILayout.FloatField("Time To Stay:",MyTarget.Goal[i].TimeToStay);
					} 
			    	else if(MyTarget.Goal[i].Type == QuestC.QuestTypes.Collection || MyTarget.Goal[i].Type == QuestC.QuestTypes.Elimination)
					{
						MyTarget.Goal[i].ItemName = EditorGUILayout.TextField("Item/Target Name:", MyTarget.Goal[i].ItemName);
						MyTarget.Goal[i].AmountRequired = EditorGUILayout.IntField("Amount:", MyTarget.Goal[i].AmountRequired);
					} 
				    else if(MyTarget.Goal[i].Type == QuestC.QuestTypes.Meeting)
					{
						EditorGUILayout.LabelField("Quest Giver To Meet:");
						EditorGUIUtility.LookLikeInspector();
						MyTarget.Goal[i].Target = EditorGUILayout.ObjectField(MyTarget.Goal[i].Target, typeof(QuestC)) as QuestC;
						EditorGUIUtility.LookLikeControls();
						EditorGUILayout.LabelField("Message On Arrival:");
						MyTarget.Goal[i].ArrivalMessage = EditorGUILayout.TextArea(MyTarget.Goal[i].ArrivalMessage, GUILayout.Height(30));
					}   
				}
				
			}
			
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			EditorGUILayout.Space();
			
			MyTarget.ReturnToGiver = EditorGUILayout.Toggle("Return To Quest Giver:", MyTarget.ReturnToGiver);
			MyTarget.RewardPlayer = EditorGUILayout.Toggle("Reward Player:", MyTarget.RewardPlayer);
			if(MyTarget.RewardPlayer == true)
			{
				MyTarget.XP = EditorGUILayout.IntField("XP:", MyTarget.XP);
			} 

		    EditorGUILayout.LabelField("Reward Item: Requires S-Inventory");
	    	EditorGUIUtility.LookLikeInspector();
	     	MyTarget.Item = EditorGUILayout.ObjectField(MyTarget.Item, typeof(GameObject)) as GameObject;
	    	EditorGUIUtility.LookLikeControls();

	    	if(GUILayout.Button("Reset Quest"))
	    	{
			    PlayerPrefs.SetInt(MyTarget.QuestCode+MyTarget.QuestOrder.ToString(), 0);
		    }
		}
}