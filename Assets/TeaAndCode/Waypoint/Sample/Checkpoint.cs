using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour
{
    #region Variables

    [SerializeField]
    private Waypoint m_Waypoint;
    [SerializeField]
    private Checkpoint m_NextCheckpoint;
    [SerializeField]
    private bool m_StartEnabled;

    #endregion


    #region Methods

    private void Start()
    {
        Enable(m_StartEnabled);
        gameObject.SetActive(m_StartEnabled);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_NextCheckpoint != null)
        {
            m_NextCheckpoint.Enable(true);
            m_NextCheckpoint.gameObject.SetActive(true);
        }
        this.Enable(false);
        gameObject.SetActive(false);
    }

    public void Enable(bool value)
    {
        if (value)
        {
            m_Waypoint.DisplayStyle = Waypoint.Display.Normal;
        }
        else
        {
            m_Waypoint.DisplayStyle = Waypoint.Display.Off;
        }
    }

    #endregion
}
