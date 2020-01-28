using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelSlideIn : MonoBehaviour
{
    public Vector3 m_disabledOffset;
    public float m_speed;
    public Vector3 m_disabledPosition;
    public Vector3 m_enabledPosition;

    public bool m_enabled = false;

    void Start()
    {
        m_enabledPosition = transform.position;
        m_disabledPosition = m_enabledPosition + m_disabledOffset;

        transform.position = m_disabledPosition;
    }

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, m_enabled ? m_enabledPosition : m_disabledPosition, Time.deltaTime * m_speed);
    }
}
