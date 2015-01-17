using UnityEngine;
using System;
using System.Collections;

[ExecuteInEditMode]
public abstract class WaypointWidget : MonoBehaviour
{
    #region Properties

    [SerializeField]
    private GUITexture m_Icon;
    [SerializeField]
    private GUIText m_Label;
    [SerializeField]
    private GUIText m_Distance;
    [SerializeField]
    private GUIText m_Timer;

    #endregion


    #region Variables

    protected Waypoint m_Waypoint;
    protected Waypoint.Display m_Display;
    protected Transform m_CachedTransform;
    protected float m_AlphaFactor;

    #endregion


    #region Methods

    public virtual void Init(Waypoint waypoint)
    {
        m_Waypoint = waypoint;
        m_CachedTransform = this.transform;
    }

    protected virtual void Update()
    {
        if (m_Waypoint == null || WaypointSystem.Instance == null)
        {
            return;
        }

        m_AlphaFactor = WaypointSystem.Instance.DistanceFalloffAlpha(m_Waypoint.Distance);
        m_AlphaFactor *= m_Waypoint.DisplayStyleAlpha();
        
        if (m_Icon != null)
        {
            m_Icon.texture = m_Waypoint.Icon;
            Color newColor = m_Waypoint.IconTint * 0.5f;
            newColor.a *= m_AlphaFactor;
            m_Icon.color = newColor;
        }
        if (m_Label != null)
        {
            m_Label.text = m_Waypoint.Label;
            Color newColor = m_Waypoint.FontColor;
            newColor.a *= m_AlphaFactor;
            m_Label.material.color = newColor;

        }
        if (m_Distance != null)
        {
            m_Distance.text = string.Format("{0:N0}m", m_Waypoint.Distance);
            Color newColor = m_Waypoint.FontColor;
            newColor.a *= m_AlphaFactor;
            m_Distance.material.color = newColor;
        }
        if (m_Timer != null)
        {
            int minutes = Mathf.FloorToInt(m_Waypoint.Timer / 60F);
            int seconds = Mathf.FloorToInt(m_Waypoint.Timer - minutes * 60);

            m_Timer.text = string.Format("{0:0}:{1:00}", minutes, seconds);
            Color newColor = m_Waypoint.FontColor;
            newColor.a *= m_AlphaFactor;
            m_Timer.material.color = newColor;
        }
    }

    protected Vector3 GetScreenPos()
    {
        Vector3 screenPos;
        if (m_Waypoint != null && WaypointSystem.Instance.Camera != null)
        {
            screenPos = WaypointSystem.Instance.Camera.WorldToScreenPoint(m_Waypoint.transform.position);
            screenPos.x = screenPos.x / WaypointSystem.Instance.Camera.pixelWidth;
            screenPos.y = screenPos.y / WaypointSystem.Instance.Camera.pixelHeight;
            screenPos.z = m_CachedTransform.position.z;
        }
        else
        {
            screenPos = Vector3.one / 2f;
        }

        return screenPos;
    }

    protected abstract void Position(Vector3 screenPos);

    protected virtual void Enable(bool enable)
    {
        if (m_Icon != null)
        {
            m_Icon.gameObject.SetActive(enable);
        }
        if (m_Label != null)
        {
            m_Label.gameObject.SetActive(enable);
        }
        if (m_Distance != null)
        {
            m_Distance.gameObject.SetActive(enable);
        }
        if (m_Timer != null)
        {
            m_Timer.gameObject.SetActive(enable);
        }
    }

    protected bool OnScreen(Vector3 screenPos)
    {
        if (m_Waypoint != null && WaypointSystem.Instance.Camera != null)
        {
            Vector3 waypointDirection = m_Waypoint.transform.position - WaypointSystem.Instance.Camera.transform.position;
            if (Vector3.Dot(waypointDirection, WaypointSystem.Instance.Camera.transform.forward) <= 0)
            {
                return false;
            }
        }

        float safeMargin = WaypointSystem.Instance != null ? WaypointSystem.Instance.SafeMargin : 0.1f;

        if (screenPos.x < safeMargin ||
            screenPos.x > 1 - safeMargin ||
            screenPos.y < safeMargin ||
            screenPos.y > 1 - safeMargin)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    #endregion
}