@CustomEditor (Quest)
class QuestEditor extends Editor 
{
    var Manager : QuestManager;
    
    //Quest custom editor: makes creating quests easy and simple.
    function OnInspectorGUI () 
    {
        Manager = FindObjectOfType(QuestManager);
        EditorGUIUtility.LookLikeInspector();
        EditorGUIUtility.LookLikeControls();
        EditorGUILayout.Space();
        
        target.QuestCode = EditorGUILayout.TextField("Quest Code:", target.QuestCode);
        target.QuestOrder = EditorGUILayout.IntField("Quest Order:", target.QuestOrder);
        target.ActivateOnStart = EditorGUILayout.Toggle("Activate On Start:", target.ActivateOnStart);
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        target.QuestTitle = EditorGUILayout.TextField("Quest Title:", target.QuestTitle);
        EditorGUILayout.LabelField("Quest Objective (Short):");
        target.QuestObjective = EditorGUILayout.TextArea(target.QuestObjective, GUILayout.Height(30));
        EditorGUILayout.LabelField("Quest Description/Story (Long):"); 
        target.Description = EditorGUILayout.TextArea(target.Description, GUILayout.Height(80));
        target.ShowAfterFinished = EditorGUILayout.Toggle("Show When Completed:", target.ShowAfterFinished);
        if(target.ShowAfterFinished == true)
        {
            EditorGUILayout.LabelField("Completed Quest Message (Short):");
            target.FinishedQuestMessage = EditorGUILayout.TextArea(target.FinishedQuestMessage, GUILayout.Height(30));
        }    
        EditorGUILayout.LabelField("Speech Audio:");
        EditorGUIUtility.LookLikeInspector();
        target.Speech = EditorGUILayout.ObjectField(target.Speech, typeof(AudioClip)) as AudioClip;
        EditorGUIUtility.LookLikeControls();        
         
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        target.QuestGiverName = EditorGUILayout.TextField("Quest Giver Name:", target.QuestGiverName);
        EditorGUILayout.LabelField("Quest Giver Image:");
        EditorGUIUtility.LookLikeInspector();
        target.QuestGiverSprite = EditorGUILayout.ObjectField(target.QuestGiverSprite, typeof(Sprite)) as Sprite;
        EditorGUIUtility.LookLikeControls();
        
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
         
        var Condition : SerializedProperty = serializedObject.FindProperty("Goal");
        Condition.arraySize = EditorGUILayout.IntField("Condition(s):", Condition.arraySize);
        serializedObject.ApplyModifiedProperties();
        if(Condition.arraySize < 1) Condition.arraySize = 1;
        
        if(Condition.arraySize >= 1)
        {
        
        for(var i : int = 0; i < Condition.arraySize; i++)
        {
            var x =  i + 1;
            EditorGUILayout.LabelField("Condition " + x.ToString() + ":");
            EditorGUILayout.LabelField("Condition Type:");
            target.Goal[i].Type = EditorGUILayout.EnumPopup(target.QuestTypes, target.Goal[i].Type);
            target.Goal[i].Message = EditorGUILayout.TextField("Condition Message: ", target.Goal[i].Message); 
            if(target.Goal[i].Type == 0)
            {
                target.Goal[i].Destination = EditorGUILayout.Vector3Field("Destination:",target.Goal[i].Destination);
                target.Goal[i].Range = EditorGUILayout.FloatField("Range:",target.Goal[i].Range);
                target.Goal[i].TimeToStay = EditorGUILayout.FloatField("Time To Stay:",target.Goal[i].TimeToStay);
            } 
            else if(target.Goal[i].Type == 1 || target.Goal[i].Type == 3)
            {
                target.Goal[i].ItemName = EditorGUILayout.TextField("Item/Target Name:", target.Goal[i].ItemName);
                target.Goal[i].AmountRequired = EditorGUILayout.IntField("Amount:", target.Goal[i].AmountRequired);
            } 
            else if(target.Goal[i].Type == 2)
            {
                EditorGUILayout.LabelField("Quest Giver To Meet:");
                EditorGUIUtility.LookLikeInspector();
                target.Goal[i].Target = EditorGUILayout.ObjectField(target.Goal[i].Target, typeof(Quest)) as Quest;
                EditorGUIUtility.LookLikeControls();
                EditorGUILayout.LabelField("Message On Arrival:");
                target.Goal[i].ArrivalMessage = EditorGUILayout.TextArea(target.Goal[i].ArrivalMessage, GUILayout.Height(30));
            }   
        }
        
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        target.ReturnToGiver = EditorGUILayout.Toggle("Return To Quest Giver:", target.ReturnToGiver);
        target.RewardPlayer = EditorGUILayout.Toggle("Reward Player:", target.RewardPlayer);
        if(target.RewardPlayer == true)
        {
            target.XP = EditorGUILayout.IntField("XP:", target.XP);
        }    
        EditorGUILayout.LabelField("Reward Item: Requires S-Inventory");
        EditorGUIUtility.LookLikeInspector();
        target.Item = EditorGUILayout.ObjectField(target.Item, typeof(GameObject)) as GameObject;
        EditorGUIUtility.LookLikeControls();
        
        if(GUILayout.Button("Reset Quest"))
        {
            PlayerPrefs.SetInt(target.QuestCode+target.QuestOrder.ToString(), 0);
        }
    }
}