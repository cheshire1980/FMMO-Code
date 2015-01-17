using UnityEngine;
using System.Collections;

public class OnScreenWidget : WaypointWidget
{
    protected override void Update()
    {
        base.Update();

        Vector3 screenPos = GetScreenPos();
        Enable(OnScreen(screenPos));
        Position(screenPos);
    }

    protected override void Position(Vector3 screenPos)
    {
        if (m_Waypoint == null)
        {
            transform.localPosition = screenPos;
            return;
        }

        m_CachedTransform.localPosition = screenPos;
    }
}