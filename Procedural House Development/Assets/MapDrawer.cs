using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDrawer : MonoBehaviour
{
    public LineRenderer m_line;
    public List<Vector3> m_corners;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            if (m_line.positionCount >= 3)
            {
                Debug.Log("Completed zone");

                for(int i = 0; i < Mathf.Max(m_line.positionCount, 4) - 1; ++i)
                {
                    m_corners.Add(m_line.GetPosition(i));
                }
            }
        }
        else if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit info;

            if (Physics.Raycast(ray, out info))
            {
                m_line.positionCount += 1;

                m_line.SetPosition(m_line.positionCount - 1, info.point);
            }
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
