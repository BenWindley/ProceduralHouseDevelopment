using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour
{
    private Manager m_manager;

    public LineRenderer m_line;
    public float m_lineWidth = 0.1f;
    public List<Vector3> m_edges;
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

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (m_line.positionCount >= 3)
            {
                m_updateLine = false;
                m_edges.Clear();
                m_line.positionCount--;

                for (int i = 0; i < Mathf.Max(m_line.positionCount, 4); ++i)
                {
                    m_edges.Add(m_line.GetPosition(i) * Vector2.one);
                }

                GetComponent<Manager>().NextSection();
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;

            if (Physics.Raycast(ray, out info))
            {
                m_line.positionCount++;

                m_line.SetPosition(m_line.positionCount - 1, info.point);
            }
        }
        else if(Input.GetMouseButtonDown(1))
        {
            if(m_line.positionCount > 1)
                m_line.positionCount--;
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;

            if (Physics.Raycast(ray, out info))
            {
                m_line.SetPosition(m_line.positionCount - 1, info.point);
            }
        }
    }
}
