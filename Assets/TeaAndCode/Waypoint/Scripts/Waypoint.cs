using UnityEngine;
using System.Collections;

public class Waypoint : MonoBehaviour
{
    #region Enumerations

    public enum Display
    {
        Off = 0,
        Normal = 1,
        Pulsing = 2,
    }

    #endregion


    #region Properties

    [SerializeField]
    private OnScreenWidget m_OnScreenPrefab;
    public OnScreenWidget OnScreenWidget
    {
        get { return m_OnScreenPrefab; }
        set { m_OnScreenPrefab = value; }
    }
    public void SetOnScreenWidgetFromEditor(OnScreenWidget value)
    {
        if (m_OnScreenPrefab != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "OnScreenWidget");
            m_OnScreenPrefab = value;
        }
    }

    [SerializeField]
    private OffScreenWidget m_OffScreenPrefab;
    public OffScreenWidget OffScreenWidget
    {
        get { return m_OffScreenPrefab; }
        set { m_OffScreenPrefab = value; }
    }
    public void SetOffScreenWidgetFromEditor(OffScreenWidget value)
    {
        if (m_OffScreenPrefab != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "OffScreenWidget");
            m_OffScreenPrefab = value;
        }
    }

    [SerializeField]
    private Texture m_Icon;
    public Texture Icon
    {
        get { return m_Icon; }
        set { m_Icon = value; }
    }
    public void SetIconFromEditor(Texture value)
    {
        if (m_Icon != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "Icon");
            m_Icon = value;
        }
    }

    [SerializeField]
    private Color m_IconTint = Color.white;
    public Color IconTint
    {
        get { return m_IconTint; }
        set { m_IconTint = value; }
    }
    public void SetIconTintFromEditor(Color value)
    {
        if (m_IconTint != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "IconTint");
            m_IconTint = value;
        }
    }

    [SerializeField]
    private string m_Label;
    public string Label
    {
        get { return m_Label; }
        set { m_Label = value; }
    }
    public void SetLabelFromEditor(string value)
    {
        if (m_Label != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "Label");
            m_Label = value;
        }
    }

    [SerializeField]
    private Color m_FontColor = Color.white;
    public Color FontColor
    {
        get { return m_FontColor; }
        set { m_FontColor = value; }
    }
    public void SetFontColorFromEditor(Color value)
    {
        if (m_FontColor != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "FontColor");
            m_FontColor = value;
        }
    }

    [SerializeField]
    private Display m_DisplayStyle = Display.Normal;
    private float m_DisplayStyleSetTime;
    public Display DisplayStyle
    {
        get { return m_DisplayStyle; }
        set 
        {
            m_DisplayStyle = value;
            m_DisplayStyleSetTime = Time.time;
        }
    }
    public void SetDisplayStyleFromEditor(Display value)
    {
        if (m_DisplayStyle != value)
        {
            WaypointSystem.OnModifiedByEditor(this, "DisplayStyle");
            m_DisplayStyle = value;
        }
    }

    public float Distance
    {
        get;
        set;
    }

    public float Timer
    {
        get;
        set;
    }

    #endregion


    #region Private

    private OnScreenWidget m_OnScreenWidget;
    private OffScreenWidget m_OffScreenWidget;

    #endregion


    #region Methods

    void Start()
    {
        if (WaypointSystem.Instance == null)
        {
            Debug.LogError("WaypointSystem not found. Please select Tools->Waypoint->Create System from the toolbar.");
        }

        if (m_OnScreenPrefab != null)
        {
            m_OnScreenWidget = (OnScreenWidget)Instantiate(m_OnScreenPrefab, WaypointSystem.Instance.transform.position, Quaternion.identity);
            m_OnScreenWidget.transform.parent = WaypointSystem.Instance.transform;
            m_OnScreenWidget.Init(this);
        }
        if (m_OffScreenPrefab != null)
        {
            m_OffScreenWidget = (OffScreenWidget)Instantiate(m_OffScreenPrefab, WaypointSystem.Instance.transform.position, Quaternion.identity);
            m_OffScreenWidget.transform.parent = WaypointSystem.Instance.transform;
            m_OffScreenWidget.Init(this);
        }
    }

    void Update()
    {
        if (WaypointSystem.Instance.Player)
        {
            Distance = Vector3.Distance(WaypointSystem.Instance.Player.transform.position, this.transform.position);
        }
        else
        {
            Distance = Vector3.Distance(WaypointSystem.Instance.Camera.transform.position, this.transform.position);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawIcon(transform.position, ".." + WaypointSystem.WaypointPath + @"/Gizmos/WaypointGizmo.png", true);
    }

    void OnDestroy()
    {
        if (m_OnScreenWidget != null)
        {
            DestroyObject(m_OnScreenWidget.gameObject);
        }
        if (m_OffScreenWidget != null)
        {
            DestroyObject(m_OffScreenWidget.gameObject);
        }
    }

    public float DisplayStyleAlpha()
    {
        switch (DisplayStyle)
        {
            case Waypoint.Display.Off:
                return 0f;
            case Waypoint.Display.Normal:
                return 1f;
            case Waypoint.Display.Pulsing:
                return Mathf.PingPong(Time.time - m_DisplayStyleSetTime, WaypointSystem.Instance.PulsePeriod / 2);
            default:
                //Unreachable
                throw new UnityException("Waypoint.DisplayStyle enumeration element not implemented in WaypointWidget::Update");
        }
    }

    public void SetNameFromEditor(string name)
    {
        if (this.name != name)
        {
            WaypointSystem.OnModifiedByEditor(this, "Name");
            this.name = name;
        }
    }

    #endregion
}