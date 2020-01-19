using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeSelector : MonoBehaviour
{
    private Manager m_manager;

    public LineRenderer m_line;
    public List<Vector3> m_mainRoad = new List<Vector3>();
    public float m_lineWidth = 0.1f;
    public bool m_updateLine = true;

    private void Start()
    {
        m_manager = Camera.main.GetComponent<Manager>();
        m_line.widthMultiplier = m_lineWidth;
    }

    void Update()
    {
        if (!m_updateLine)
            return;

        int closestIndex = 0;
        float distance = 0.0f;
        Vector3 p1;
        Vector3 p2;

        for (int i = 0; i < m_manager.m_edges.Count; ++i)
        {
            p1 = m_manager.m_edges[i];
            p2 = m_manager.m_edges[(i + 1) % m_manager.m_edges.Count];
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float d = Vector3.Distance(m, MathUtility.NearestPointToLine(p1, p2 - p1, m));

            if(i == 0)
            {
                distance = d;
                continue;
            }
            
            if(d < distance)
            {
                closestIndex = i;
                distance = d;
            }
        }

        m_line.SetPosition(0, m_manager.m_edges[closestIndex]);
        m_line.SetPosition(1, m_manager.m_edges[(closestIndex + 1) % m_manager.m_edges.Count]);

        if(Input.GetMouseButtonDown(0))
        {
            m_mainRoad.Clear();
            m_mainRoad.Add(m_manager.m_edges[closestIndex]);
            m_mainRoad.Add(m_manager.m_edges[(closestIndex + 1) % m_manager.m_edges.Count]);

            m_manager.NextSection();
        }
    }
}
