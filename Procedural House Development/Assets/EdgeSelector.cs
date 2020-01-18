using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeSelector : MonoBehaviour
{
    public List<Vector3> m_areaPoints = new List<Vector3>();
    public LineRenderer m_line;

    void Update()
    {
        int closestIndex = 0;
        float distance = 0.0f;
        Vector3 p1;
        Vector3 p2;

        for (int i = 0; i < m_areaPoints.Count; ++i)
        {
            p1 = m_areaPoints[i];
            p2 = m_areaPoints[(i + 1) % m_areaPoints.Count];
            Vector3 m = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float d = Vector3.Distance(m, NearestPointToLine(p1, p2 - p1, m));

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

        m_line.SetPosition(0, m_areaPoints[closestIndex]);
        m_line.SetPosition(1, m_areaPoints[(closestIndex + 1) % m_areaPoints.Count]);
    }

    public static Vector3 NearestPointToLine(Vector3 linePoint, Vector3 lineDir, Vector3 point)
    {
        lineDir.Normalize();
        Vector3 v = point - linePoint;
        float d = Vector3.Dot(v, lineDir);
        return linePoint + lineDir * d;
    }
}
