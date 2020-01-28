using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualiser : MonoBehaviour
{
    public List<Road> m_roads;
    public GameObject m_roadPrefab;

    public void Clear()
    {
        for(int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);

        m_roads.Clear();
    }

    public void GenerateRoads()
    {
        foreach(Road r in m_roads)
        {
            GameObject g = Instantiate(m_roadPrefab, transform);
            LineRenderer gr = g.GetComponent<LineRenderer>();

            gr.SetPosition(0, r.m_start);
            gr.SetPosition(1, r.m_end);
        }
    }
}
