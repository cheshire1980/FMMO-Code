#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WaypointSystem : MonoBehaviour
{
    //Change to where you moved the plugin
    public const string WaypointPath = @"/TeaAndCode/Waypoint";

    #region Enumerations

    public enum OffscreenStyle
    {
        Compass = 0,
        Clamp = 1
    }

    #endregion


    #region Singleton

    public static WaypointSystem Instance
    {
        get;
        set;
    }

    void OnEnable()
    {
        Instance = this;
    }

    #endregion


    #region Properties

    [SerializeField]
    private GameObject m_Player;
    public GameObject Player
    {
        get { return m_Player; }
        set { m_Player = value; }
    }
    public void SetPlayerFromEditor(GameObject value)
    {
        if (m_Player != value)
        {
            OnModifiedByEditor(this, "Player");
            m_Player = value;
        }
    }

    [SerializeField]
    private Camera m_Camera;
    public Camera Camera
    {
        get { return m_Camera; }
        set { m_Camera = value; }
    }
    public void SetCameraFromEditor(Camera value)
    {
        if (m_Camera != value)
        {
            OnModifiedByEditor(this, "Camera");
            m_Camera = value;
        }
    }

    [SerializeField]
    private OffscreenStyle m_Style = OffscreenStyle.Compass;
    public OffscreenStyle Style
    {
        get { return m_Style; }
        set { m_Style = value; }
    }
    public void SetStyleFromEditor(OffscreenStyle value)
    {
        if (m_Style != value)
        {
            OnModifiedByEditor(this, "Style");
            m_Style = value;
        }
    }

    [SerializeField]
    [Range(0f, 0.5f)]
    private float m_SafeMargin = 0.1f;
    public float SafeMargin
    {
        get { return m_SafeMargin; }
        set { m_SafeMargin = value; }
    }
    public void SetSafeMarginFromEditor(float value)
    {
        if (m_SafeMargin != value)
        {
            OnModifiedByEditor(this, "SafeMargin");
            m_SafeMargin = value;
        }
    }

    [SerializeField]
    [Range(0f, 1f)]
    private float m_CompassSize = 0.75f;
    public float CompassSize
    {
        get { return m_CompassSize; }
        set { m_CompassSize = value; }
    }
    public void SetCompassSizeFromEditor(float value)
    {
        if (m_CompassSize != value)
        {
            OnModifiedByEditor(this, "CompassSize");
            m_CompassSize = value;
        }
    }

    [SerializeField]
    [Range(0.5f, 5f)]
    private float m_PulsePeriod = 2f;
    public float PulsePeriod
    {
        get { return m_PulsePeriod; }
        set { m_PulsePeriod = value; }
    }
    public void SetPulsePeriodFromEditor(float value)
    {
        if (m_PulsePeriod != value)
        {
            OnModifiedByEditor(this, "PulsePeriod");
            m_PulsePeriod = value;
        }
    }

    [SerializeField]
    private bool m_Falloff = false;
    public bool Falloff
    {
        get { return m_Falloff; }
        set { m_Falloff = value; }
    }
    public void SetFalloffFromEditor(bool value)
    {
        if (m_Falloff != value)
        {
            OnModifiedByEditor(this, "Falloff");
            m_Falloff = value;
        }
    }

    [SerializeField]
    private float m_FalloffMax = 50f;
    public float FalloffMax
    {
        get { return m_FalloffMax; }
        set { m_FalloffMax = value; }
    }
    public void SetFalloffMaxFromEditor(float value)
    {
        if (m_FalloffMax != value)
        {
            OnModifiedByEditor(this, "FalloffMax");
            m_FalloffMax = value;
        }
    }

    [SerializeField]
    private float m_FalloffStart = 10f;
    public float FalloffStart
    {
        get { return m_FalloffStart; }
        set { m_FalloffStart = Mathf.Clamp(value, 0, m_FalloffMax); }
    }
    public void SetFalloffStartFromEditor(float value)
    {
        if (m_FalloffStart != value)
        {
            OnModifiedByEditor(this, "FalloffStart");
            m_FalloffStart = value;
        }
    }

    [SerializeField]
    private float m_FalloffEnd = 25f;
    public float FalloffEnd
    {
        get { return m_FalloffEnd; }
        set { m_FalloffEnd = Mathf.Clamp(value, 0, m_FalloffMax); }
    }
    public void SetFalloffEndFromEditor(float value)
    {
        if (m_FalloffEnd != value)
        {
            OnModifiedByEditor(this, "FalloffEnd");
            m_FalloffEnd = value;
        }
    }

    [SerializeField]
    private AnimationCurve m_FalloffAlpha = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
    public AnimationCurve FalloffAlpha
    {
        get { return m_FalloffAlpha; }
        set { m_FalloffAlpha = value; }
    }
    public void SetFalloffAlphaFromEditor(AnimationCurve value)
    {
        if (m_FalloffAlpha != value)
        {
            OnModifiedByEditor(this, "FalloffAlpha");
            m_FalloffAlpha = value;
        }
    }

    #endregion


    #region Methods

    public void Start()
    {
        if (m_Camera == null)
        {
            m_Camera = Camera.main;
        }
    }

    public float DistanceFalloffAlpha(float distance)
    {
        if (!m_Falloff)
        {
            return 1f;
        }
        else
        {
            return m_FalloffAlpha.Evaluate((distance - m_FalloffStart) / (m_FalloffEnd - m_FalloffStart));
        }
    }

    //passing in name to avoid reflections
    public static void OnModifiedByEditor(Object instance, string fieldName)
    {
#if UNITY_EDITOR
        Undo.RegisterUndo(instance, fieldName + " Change");
        EditorUtility.SetDirty(instance);
#endif
    }

    #endregion
}