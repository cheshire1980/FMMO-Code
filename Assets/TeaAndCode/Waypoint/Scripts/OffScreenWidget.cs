using UnityEngine;
using System.Collections;

public class OffScreenWidget : WaypointWidget
{
    [SerializeField]
    private RotatableGUITexture m_Arrow;

    protected override void Update()
    {
        base.Update();

        Vector3 screenPos = GetScreenPos();
        Position(screenPos);

        if (m_Waypoint == null)
        {
            return;
        }

        Enable(!OnScreen(screenPos));

        if (m_Arrow != null)
        {
            Color newColor = m_Waypoint.IconTint;
            newColor.a *= m_AlphaFactor;
            m_Arrow.Color = newColor;
            Vector2 direction = new Vector2(m_CachedTransform.localPosition.x - 0.5f, m_CachedTransform.localPosition.y - 0.5f);
            m_Arrow.Rotation = Vector2.Angle(Vector2.up, direction.normalized);
            if (direction.x < 0)
            {
                m_Arrow.Rotation *= -1f;
            }
        }
    }

    protected override void Position(Vector3 screenPos)
    {
        if (m_Waypoint == null || WaypointSystem.Instance.Camera == null)
        {
            transform.localPosition = screenPos;
            return;
        }

        WaypointSystem.OffscreenStyle offScreenStyle = WaypointSystem.Instance.Style;
        switch (offScreenStyle)
        {
            case WaypointSystem.OffscreenStyle.Compass:
                Vector3 fromCamera = m_Waypoint.transform.position - WaypointSystem.Instance.Camera.transform.position;
                Vector3 cameraRelativeDir = WaypointSystem.Instance.Camera.transform.InverseTransformDirection(fromCamera);
                cameraRelativeDir.z = 0;
                cameraRelativeDir = cameraRelativeDir.normalized / 2;
                screenPos.x = 0.5f + cameraRelativeDir.x * WaypointSystem.Instance.CompassSize / WaypointSystem.Instance.Camera.aspect;
                screenPos.y = 0.5f + cameraRelativeDir.y * WaypointSystem.Instance.CompassSize;
                m_CachedTransform.localPosition = screenPos;
                break;
            case WaypointSystem.OffscreenStyle.Clamp:
                float safeMargin = WaypointSystem.Instance.SafeMargin;
                screenPos.x = Mathf.Clamp(screenPos.x, safeMargin, 1 - safeMargin);
                screenPos.y = Mathf.Clamp(screenPos.y, safeMargin, 1 - safeMargin);
                bool planarTest = Vector3.Dot(WaypointSystem.Instance.Camera.transform.forward, WaypointSystem.Instance.Camera.transform.position - m_Waypoint.transform.position) <= 0;
                screenPos.x = planarTest ? screenPos.x : 1 - screenPos.x;
                screenPos.y = planarTest ? screenPos.y : 1 - screenPos.y;
                m_CachedTransform.localPosition = screenPos;
                break;
            default:
                //Unreachable
                throw new UnityException("WaypointSystem.OffscreenStyle enumeration element not implemented in OffScreenWidget::Widget");
        }
    }

    protected override void Enable(bool enable)
    {
        base.Enable(enable);

        if (m_Arrow != null)
        {
            m_Arrow.gameObject.SetActive(enable);
        }
    }
}