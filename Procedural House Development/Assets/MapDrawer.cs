using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour
{
    public LineRenderer m_line;
    public List<Vector3> m_corners;
    public bool updateLine = true;

    void Update()
    {
        if (!updateLine)
            return;

        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (m_line.positionCount >= 3)
            {
                Debug.Log("Completed zone");

                updateLine = false;
                m_corners.Clear();
                m_line.positionCount--;

                for (int i = 0; i < Mathf.Max(m_line.positionCount, 4); ++i)
                {
                    m_corners.Add(m_line.GetPosition(i));
                }

                GetComponent<Manager>().NextSection();
                GetComponent<EdgeSelector>().m_areaPoints = m_corners;
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
