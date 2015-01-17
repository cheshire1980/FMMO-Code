using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class WaypointEditor : EditorWindow
{
    #region Variables

    public const string PrefabExt = ".prefab";
    public const string OnScreenPattern = "OnScreen";
    public const string OffScreenPattern = "OffScreen";

    private GUIStyle m_IntroStyle;
    private GUIStyle m_SystemButtonStyle;
    private GUIStyle m_WaypointButtonStyle;
    private GUIStyle m_SelectedWaypointButtonStyle;
    private GUIStyle m_FalloffRangeInfoStyle;

    private HashSet<Waypoint> m_Waypoints = new HashSet<Waypoint>();
    private Waypoint m_SelectedWaypoint;
    private int m_PrevWaypointID;
    private Vector2 m_ScrollPosition;
    private Texture2D m_Logo;
    private string[] m_OnScreenPrefabs;
    private string[] m_OnScreenDisplayNames;
    private string[] m_OffScreenPrefabs;
    private string[] m_OffScreenDisplayNames;

    private bool m_SystemFoldout = true;
    private bool m_WaypointFoldout = true;

    #endregion


    #region Methods

    [MenuItem("Tools/Waypoint Editor")]
    public static void ShowWindow()
    {
        //Show existing window instance. If one doesn't exist, make one.
        EditorWindow.GetWindow(typeof(WaypointEditor));
    }

    void Awake()
    {
        m_IntroStyle = new GUIStyle();
        m_IntroStyle.fontSize = 14;
        m_IntroStyle.wordWrap = true;
        m_IntroStyle.margin = new RectOffset(8, 8, 8, 8);

        m_SystemButtonStyle = new GUIStyle();
        m_SystemButtonStyle.fontSize = 18;
        m_SystemButtonStyle.wordWrap = true;
        m_SystemButtonStyle.alignment = TextAnchor.MiddleCenter;
        m_SystemButtonStyle.padding = new RectOffset(16, 16, 16, 16);

        m_WaypointButtonStyle = new GUIStyle();
        m_WaypointButtonStyle.fontSize = 12;
        m_WaypointButtonStyle.wordWrap = false;
        m_WaypointButtonStyle.alignment = TextAnchor.MiddleCenter;
        m_WaypointButtonStyle.margin = new RectOffset(8, 8, 0, 0);

        m_SelectedWaypointButtonStyle = new GUIStyle();
        m_SelectedWaypointButtonStyle.fontSize = 12;
        m_SelectedWaypointButtonStyle.wordWrap = false;
        m_SelectedWaypointButtonStyle.alignment = TextAnchor.MiddleCenter;
        m_SelectedWaypointButtonStyle.margin = new RectOffset(8, 8, 0, 0);

        m_FalloffRangeInfoStyle = new GUIStyle();
        m_FalloffRangeInfoStyle.alignment = TextAnchor.UpperRight;
        m_FalloffRangeInfoStyle.margin = new RectOffset(8, 8, 0, 0);
    }

    void OnEnable()
    {
        m_Logo = Resources.Load("WaypointsLogo", typeof(Texture2D)) as Texture2D;
        EditorApplication.playmodeStateChanged = OnEditorChangedPlayMode;
    }

    void OnEditorChangedPlayMode()
    {
        //Saving off the id and popping it back between plays allows the Waypoint Editor to edit in-game
        //but still not lose the reference of the old selection after coming out of the game.
        if (!EditorApplication.isPlaying)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (m_SelectedWaypoint != null)
                {
                    //Store the id to the selected waypoint
                    m_PrevWaypointID = m_SelectedWaypoint.GetInstanceID();
                }
            }
            else
            {
                if (m_SelectedWaypoint != null)
                {
                    //Find the previous selected waypoint by ID
                    m_SelectedWaypoint = null;
                    Waypoint[] waypoints = GameObject.FindObjectsOfType(typeof(Waypoint)) as Waypoint[];
                    m_Waypoints.Clear();
                    foreach (Waypoint waypoint in waypoints)
                    {
                        if (waypoint.GetInstanceID() == m_PrevWaypointID)
                        {
                            m_SelectedWaypoint = waypoint;
                            break;
                        }
                    }
                    m_PrevWaypointID = -1;
                    Repaint();
                }
            }
        }
    }

    void OnGUI()
    {
        //Update the GUI after an Undo incase the Undo changes a field here
        if (Event.current.type == EventType.ValidateCommand)
        {
            switch (Event.current.commandName)
            {
                case "UndoRedoPerformed":
                    Repaint();
                    break;
            }
        }

        m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);

        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(m_Logo);
        if (GUILayout.Button("Help"))
        {
            Application.OpenURL(Application.dataPath + WaypointSystem.WaypointPath + @"/Help/Help.htm");
        }
        EditorGUILayout.EndHorizontal();

        if (WaypointSystem.Instance != null)
        {
            WaypointSystem system = WaypointSystem.Instance ?? GameObject.FindObjectOfType(typeof(WaypointSystem)) as WaypointSystem;
            GUIStyle foldoutStyle = new GUIStyle("Foldout");
            foldoutStyle.fontSize = 12;
            foldoutStyle.fontStyle = FontStyle.Bold;

            /*----Begin System Layout----*/
            m_SystemFoldout = EditorGUILayout.Foldout(m_SystemFoldout, "System", foldoutStyle);
            if (m_SystemFoldout)
            {
                system.SetPlayerFromEditor(EditorGUILayout.ObjectField("Player", system.Player, typeof(GameObject), true) as GameObject);
                if (system.Player == null)
                {
                    EditorGUILayout.HelpBox("Set the player game object here for distance finding features. Will use the camera instead if unset.", MessageType.Warning);
                }
                system.SetCameraFromEditor(EditorGUILayout.ObjectField("Camera", system.Camera, typeof(Camera), true) as Camera);
                if (system.Camera == null)
                {
                    EditorGUILayout.HelpBox("Set the camera to render the hud elements. Defaults to Camera.main if unset.", MessageType.Warning);
                }
                system.SetStyleFromEditor((WaypointSystem.OffscreenStyle)EditorGUILayout.EnumPopup("OffScreen Style", system.Style));
                system.SetSafeMarginFromEditor(EditorGUILayout.Slider("Safe Margin", system.SafeMargin, 0f, 0.5f));
                system.SetCompassSizeFromEditor(EditorGUILayout.Slider("Compass Size", system.CompassSize, 0f, 1f));
                system.SetPulsePeriodFromEditor(EditorGUILayout.Slider("Pulse Period", system.PulsePeriod, 0.5f, 5f));
                system.SetFalloffFromEditor(EditorGUILayout.BeginToggleGroup("Falloff", system.Falloff));
                system.SetFalloffMaxFromEditor(EditorGUILayout.FloatField("Falloff Max", system.FalloffMax));
                float falloffMin = system.FalloffStart;
                float falloffMax = system.FalloffEnd;
                EditorGUILayout.MinMaxSlider(new GUIContent("Falloff Range"), ref falloffMin, ref falloffMax, 0, system.FalloffMax);
                system.SetFalloffStartFromEditor(falloffMin);
                system.SetFalloffEndFromEditor(falloffMax);
                EditorGUILayout.BeginHorizontal("Box");
                GUILayout.Box("Min: " + falloffMin + "\t\tMax: " + falloffMax, m_FalloffRangeInfoStyle);
                EditorGUILayout.EndHorizontal();
                system.SetFalloffAlphaFromEditor(EditorGUILayout.CurveField("Falloff Alpha", system.FalloffAlpha));
                EditorGUILayout.EndToggleGroup();
            }
            /*----End System Layout----*/

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            /*----Begin Waypoints Layout----*/
            m_WaypointFoldout = EditorGUILayout.Foldout(m_WaypointFoldout, "Waypoints", foldoutStyle);
            if (m_WaypointFoldout)
            {
                Waypoint[] waypoints = GameObject.FindObjectsOfType(typeof(Waypoint)) as Waypoint[];
                m_Waypoints.Clear();
                foreach (Waypoint waypoint in waypoints)
                {
                    m_Waypoints.Add(waypoint);
                }

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("New"))
                {
                    Undo.RegisterSceneUndo("New Waypoint");
                    GameObject gameObject = new GameObject();
                    gameObject.name = "Waypoint";
                    Waypoint waypoint = gameObject.AddComponent<Waypoint>();
                    m_SelectedWaypoint = waypoint;
                }
                if (GUILayout.Button("New at selection"))
                {
                    Undo.RegisterSceneUndo("New Waypoint at Selection");
                    GameObject gameObject = new GameObject();
                    gameObject.transform.parent = Selection.activeTransform;
                    gameObject.transform.localPosition = Vector3.zero;
                    gameObject.name = "Waypoint";
                    Waypoint waypoint = gameObject.AddComponent<Waypoint>();
                    m_SelectedWaypoint = waypoint;
                }
                if (GUILayout.Button("Clone"))
                {
                    Undo.RegisterSceneUndo("Clone Waypoint");
                    Waypoint waypoint = Instantiate(m_SelectedWaypoint) as Waypoint;
                    m_SelectedWaypoint = waypoint;
                }
                if (GUILayout.Button("Delete"))
                {
                    Undo.RegisterSceneUndo("Delete Waypoint");
                    m_Waypoints.Remove(m_SelectedWaypoint);
                    DestroyImmediate(m_SelectedWaypoint.gameObject);
                    m_SelectedWaypoint = null;
                }
                EditorGUILayout.EndHorizontal();
                foreach (Waypoint waypoint in m_Waypoints)
                {
                    if (waypoint == m_SelectedWaypoint)
                    {
                        Rect rect = EditorGUILayout.BeginHorizontal("Box");
                        GUI.Box(rect, GUIContent.none);
                        if (GUILayout.Button(waypoint.name, m_SelectedWaypointButtonStyle))
                        {
                            Selection.activeGameObject = waypoint.gameObject;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        if (GUILayout.Button(waypoint.name, m_WaypointButtonStyle))
                        {
                            m_SelectedWaypoint = waypoint;
                            GUI.FocusControl(null);
                        }
                    }
                }
                EditorGUILayout.Separator();
                if (m_SelectedWaypoint != null)
                {
                    RefreshPrefabs();
                    m_SelectedWaypoint.SetNameFromEditor(EditorGUILayout.TextField("Name", m_SelectedWaypoint.name));
                    int onScreenWidgetIndex = m_SelectedWaypoint.OnScreenWidget != null ? FindIndexInArray(m_OnScreenPrefabs, m_SelectedWaypoint.OnScreenWidget.name) : 0;
                    int newOnScreenWidgetIndex = EditorGUILayout.Popup("On Screen Widget", onScreenWidgetIndex, m_OnScreenDisplayNames);
                    if (onScreenWidgetIndex != newOnScreenWidgetIndex)
                    {
                        if (newOnScreenWidgetIndex == 0)
                        {
                            m_SelectedWaypoint.SetOnScreenWidgetFromEditor(null);
                        }
                        else
                        {
                            m_SelectedWaypoint.SetOnScreenWidgetFromEditor(
                                ((GameObject)AssetDatabase.LoadAssetAtPath(@"Assets" + WaypointSystem.WaypointPath + @"/Widgets/" +
                                m_OnScreenPrefabs[newOnScreenWidgetIndex] + ".prefab", typeof(GameObject))).GetComponent<OnScreenWidget>());
                        }
                    }
                    int offScreenWidgetIndex = m_SelectedWaypoint.OffScreenWidget != null ? FindIndexInArray(m_OffScreenPrefabs, m_SelectedWaypoint.OffScreenWidget.name) : 0;
                    int newOffScreenWidgetIndex = EditorGUILayout.Popup("Off Screen Widget", offScreenWidgetIndex, m_OffScreenDisplayNames);
                    if (offScreenWidgetIndex != newOffScreenWidgetIndex)
                    {
                        if (newOffScreenWidgetIndex == 0)
                        {
                            m_SelectedWaypoint.SetOffScreenWidgetFromEditor(null);
                        }
                        else
                        {
                            m_SelectedWaypoint.SetOffScreenWidgetFromEditor(
                                ((GameObject)AssetDatabase.LoadAssetAtPath(@"Assets" + WaypointSystem.WaypointPath + @"/Widgets/" +
                                m_OffScreenPrefabs[newOffScreenWidgetIndex] + ".prefab", typeof(GameObject))).GetComponent<OffScreenWidget>());
                        }
                    }
                    m_SelectedWaypoint.SetIconFromEditor(
                        EditorGUILayout.ObjectField("Icon",
                        m_SelectedWaypoint.Icon,
                        typeof(Texture), true) as Texture);
                    m_SelectedWaypoint.SetIconTintFromEditor(EditorGUILayout.ColorField("Icon Tint", m_SelectedWaypoint.IconTint));
                    m_SelectedWaypoint.SetLabelFromEditor(EditorGUILayout.TextField("Label", m_SelectedWaypoint.Label));
                    m_SelectedWaypoint.SetFontColorFromEditor(EditorGUILayout.ColorField("Font Color", m_SelectedWaypoint.FontColor));
                    m_SelectedWaypoint.SetDisplayStyleFromEditor((Waypoint.Display)EditorGUILayout.EnumPopup("Display Style", m_SelectedWaypoint.DisplayStyle));
                }
            }
            /*----End Waypoints Layout----*/
        }
        else
        {
            GUILayout.Label("Welcome to HUD Waypoints! No Waypoint System setup has been found. Click the following button to get started on one.", m_IntroStyle);
            Rect rect = EditorGUILayout.BeginHorizontal("Button");
            if (GUI.Button(rect, GUIContent.none))
            {
                Undo.RegisterSceneUndo("create Waypoint System");
                GameObject gameObject = new GameObject();
                gameObject.name = "Waypoint System";
                gameObject.AddComponent<WaypointSystem>();
            }
            GUILayout.Label("Create Waypoint System", m_SystemButtonStyle);
            EditorGUILayout.EndHorizontal();
        }
        EditorGUILayout.EndScrollView();
    }

    void RefreshPrefabs()
    {
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + WaypointSystem.WaypointPath + @"/Widgets");
        FileInfo[] info;
        //Prepend wildcard
        string pattern = "*" + OnScreenPattern + PrefabExt;
        info = dir.GetFiles(pattern);
        m_OnScreenPrefabs = new string[info.Length + 1];
        m_OnScreenDisplayNames = new string[info.Length + 1];
        m_OnScreenPrefabs[0] = "None";
        m_OnScreenDisplayNames[0] = "None";
        for (int i = 0; i < info.Length; ++i)
        {
            m_OnScreenPrefabs[i + 1] = info[i].Name.Substring(0, info[i].Name.Length - PrefabExt.Length);
            m_OnScreenDisplayNames[i + 1] = m_OnScreenPrefabs[i + 1].Substring(0, m_OnScreenPrefabs[i + 1].Length - OnScreenPattern.Length);
        }

        pattern = "*" + OffScreenPattern + PrefabExt;
        info = dir.GetFiles(pattern);
        m_OffScreenPrefabs = new string[info.Length + 1];
        m_OffScreenDisplayNames = new string[info.Length + 1];
        m_OffScreenPrefabs[0] = "None";
        m_OffScreenDisplayNames[0] = "None";
        for (int i = 0; i < info.Length; ++i)
        {
            m_OffScreenPrefabs[i + 1] = info[i].Name.Substring(0, info[i].Name.Length - PrefabExt.Length);
            m_OffScreenDisplayNames[i + 1] = m_OffScreenPrefabs[i + 1].Substring(0, m_OffScreenPrefabs[i + 1].Length - OffScreenPattern.Length);
        }
    }

    int FindIndexInArray(string[] array, string name)
    {
        for (int i = 0; i < array.Length; ++i)
        {
            if (array[i] == name)
            {
                return i;
            }
        }
        return 0;
    }

    #endregion
}