using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadVisualiser : MonoBehaviour
{
    public List<Road> m_roads = new List<Road>();
    public List<House> m_houses = new List<House>();
    public GameObject m_roadPrefab;
    public GameObject m_housePrefab;

    public void Clear()
    {
        for(int i = 0; i < transform.childCount; ++i)
            Destroy(transform.GetChild(i).gameObject);

        m_roads.Clear();
        m_houses.Clear();
    }

    public void GenerateRoads()
    {
        foreach(Road r in m_roads)
        {
            GameObject g = Instantiate(m_roadPrefab, transform);
            g.name = "Road";
            LineRenderer gr = g.GetComponent<LineRenderer>();

            gr.SetPosition(0, r.m_start);
            gr.SetPosition(1, r.m_end);
        }

        foreach(House h in m_houses)
        {
            GameObject g = Instantiate(m_housePrefab, transform);
            g.name = "House";
            LineRenderer gr = g.GetComponent<LineRenderer>();

            gr.SetPosition(0, h.p[0]);
            gr.SetPosition(1, h.p[1]);
            gr.SetPosition(2, h.p[2]);
            gr.SetPosition(3, h.p[3]);

            //switch (h.m_housingType)
            //{
            //    case House.HouseType.Detatched:
            //        gr.startColor = new Color(0, 1, 1);
            //        gr.endColor = new Color(0, 1, 1);
            //        break;
            //    case House.HouseType.SemiDetatched:
            //        gr.startColor = new Color(.5f, 1, 1);
            //        gr.endColor = new Color(.5f, 1, 1);
            //        break;
            //    case House.HouseType.Terraced:
            //        gr.startColor = new Color(1, 1, 1);
            //        gr.endColor = new Color(1, 1, 1);
            //        break;
            //    default:
            //        gr.startColor = new Color(1, 1, 1);
            //        gr.endColor = new Color(1, 1, 1);
            //        break;
            //}
        }
    }

    void OnDrawGizmosSelected()
    {
        foreach (House h in m_houses)
        {
            Gizmos.DrawCube(h.m_checkPoint1, Vector3.one * 0.1f);
            Gizmos.DrawCube(h.m_checkPoint2, Vector3.one * 0.1f);
        }
    }
}
