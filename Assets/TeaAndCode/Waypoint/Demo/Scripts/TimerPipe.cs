using UnityEngine;
using System.Collections;

public class TimerPipe : MonoBehaviour
{
    public float m_Duration = 120f;
    public float m_CriticalTime = 10f;
    public Waypoint m_Waypoint;

    private float m_TimeLeft = -1f;

	void Start()
    {
        m_TimeLeft = m_Duration;
	}
	
	void Update()
    {
        if (m_TimeLeft < 0f)
        {
            DestroyImmediate(m_Waypoint);
        }
        m_TimeLeft -= Time.deltaTime;
        m_Waypoint.Timer = m_TimeLeft;
        if (m_TimeLeft <= m_CriticalTime)
        {
            m_Waypoint.FontColor = Color.red;
        }
	}
}
