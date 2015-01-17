using UnityEngine;

[ExecuteInEditMode()]
public class RotatableGUITexture : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private Texture2D m_Texture = null;
    public Texture2D Texture
    {
        get { return m_Texture; }
        set { m_Texture = value; }
    }

    [SerializeField]
    private Color m_Color = Color.white;
    public Color Color
    {
        get { return m_Color; }
        set { m_Color = value; }
    }

    [SerializeField]
    private float m_Rotation = 0;
    public float Rotation
    {
        get { return m_Rotation; }
        set { m_Rotation = value; }
    }

    [SerializeField]
    private Vector2 m_Size = new Vector2(128, 128);
    public Vector2 Size
    {
        get { return m_Size; }
        set { m_Size = value; }
    }

    [SerializeField]
    private Vector2 m_Pivot = new Vector2(0, 0);
    public Vector2 Pivot
    {
        get { return m_Pivot; }
        set { m_Pivot = value; }
    }

    private Camera m_Camera;
    public Camera Camera
    {
        get
        {
            if (m_Camera == null)
            {
                m_Camera = WaypointSystem.Instance.Camera ?? Camera.main;
            }
            return m_Camera;
        }
    }

    #endregion


    #region Variables

    private Vector2 m_Position = new Vector2(0, 0);
    private Vector2 m_ScreenPivot;
    private Rect m_Rect;

    #endregion


    #region Methods

    void Update()
    {
        if (Texture == null || Camera == null)
        {
            return;
        }

        m_Position = new Vector2(transform.position.x, transform.position.y);
        Vector2 pixelPos = new Vector2(Camera.pixelWidth * m_Position.x, Camera.pixelHeight * (1f - m_Position.y));
        m_ScreenPivot = pixelPos;
        m_Rect = new Rect(pixelPos.x - m_Size.x * 0.5f + m_Pivot.x, pixelPos.y - m_Size.y * 0.5f + m_Pivot.y, m_Size.x, m_Size.y);
    }

    void OnGUI()
    {
        if (Texture == null || Camera == null)
        {
            return;
        }

        if (Application.isEditor)
        {
            Update();
        }
        Matrix4x4 matrixBackup = GUI.matrix;
        GUIUtility.RotateAroundPivot(m_Rotation, m_ScreenPivot);
        GUI.color = m_Color;
        GUI.DrawTexture(m_Rect, m_Texture);
        GUI.matrix = matrixBackup;
    }

    #endregion
}