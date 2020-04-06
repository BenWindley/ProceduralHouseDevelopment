using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadHouseModifier : MonoBehaviour
{
    public List<House> m_houses = new List<House>();
    public List<GameObject> m_houseObjects = new List<GameObject>();
    public List<Road> m_roads = new List<Road>();
    public List<GameObject> m_roadObjects = new List<GameObject>();

    public int m_selectedHouse;
    public int m_selectedRoad;

    public Vector3 m_previousMousePosition;

    public void Init(RoadVisualiser roadVisualiser)
    {
        m_houses = roadVisualiser.m_houses;
        m_houseObjects = roadVisualiser.m_houseObjects;
        m_roads = roadVisualiser.m_roads;
        m_roadObjects = roadVisualiser.m_roadObjects;
    }

    void Update()
    {
        m_selectedHouse = -1;
        m_selectedRoad = -1;

        for (int h = 0; h < m_houses.Count; ++h)
        {
            if (MathUtility.PointInPolygon(m_houses[h].p, Camera.main.ScreenToWorldPoint(Input.mousePosition)))
            {
                m_selectedHouse = h;
            }
        }

        if (Input.GetMouseButton(0) && m_selectedHouse != -1)
        {
            m_houses.RemoveAt(m_selectedHouse);
            Destroy(m_houseObjects[m_selectedHouse]);
            m_houseObjects.RemoveAt(m_selectedHouse);

            FindObjectOfType<Stats>().Init(m_houses, m_roads);
        }

        for (int r = 0; r < m_roads.Count; ++r)
        {
            if (MathUtility.LineSegmentLineSegmentIntersection(Camera.main.ScreenToWorldPoint(m_previousMousePosition), Camera.main.ScreenToWorldPoint(Input.mousePosition), m_roads[r].m_start, m_roads[r].m_end))
            {
                m_selectedRoad = r;
            }
        }

        if (Input.GetMouseButton(0) && m_selectedRoad != -1)
        {
            m_roads.RemoveAt(m_selectedRoad);
            Destroy(m_roadObjects[m_selectedRoad]);
            m_roadObjects.RemoveAt(m_selectedRoad);

            FindObjectOfType<Stats>().Init(m_houses, m_roads);
        }

        m_previousMousePosition = Input.mousePosition;
    }
}
