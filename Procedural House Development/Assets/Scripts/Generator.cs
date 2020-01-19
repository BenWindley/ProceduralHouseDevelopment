using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Road
{
    public Vector3 m_start;
    public Vector3 m_end;

    public Road(Vector3 start, Vector3 end)
    {
        m_start = start;
        m_end = end;
    }

    public float Length()
    {
        return Vector3.Distance(m_start, m_end);
    }
}

public class Generator : MonoBehaviour
{
    private Manager m_manager;
    public RoadVisualiser m_roadVisualiser;

    // Buffer between the edges of the main road, between 0 for no buffer and 0.5 for only center
    public float m_mainRoadEdgeBuffer = 0.05f;
    public float m_roadEdgeAvoidance = 0.05f;
    public float m_stepSize = 0.01f;

    public List<Road> m_roads = new List<Road>();
    public Road m_initialRoad;

    public bool m_perpDir = true;

    public void Awake()
    {
        m_manager = Camera.main.GetComponent<Manager>();
    }

    public void Generate()
    {
        // Main road starting point
        Vector3 mrPoint = m_manager.m_mainRoad[0];
        // Main road direction
        Vector3 mrDir = (m_manager.m_mainRoad[1] - m_manager.m_mainRoad[0]).normalized;

        Road m_mainRoad = new Road(
            m_manager.m_mainRoad[0] + mrDir * m_mainRoadEdgeBuffer,
            m_manager.m_mainRoad[1] - mrDir * m_mainRoadEdgeBuffer);
        m_initialRoad = null;

        float index = 0.0f;

        // Generate inital road
        while(index < 1.0f)
        {
            Vector3 start = Vector3.Lerp(m_mainRoad.m_start, m_mainRoad.m_end, index);

            Vector3 end = Vector3.zero;

            // Check road with all edges
            for (int i = 0; i < m_manager.m_edges.Count; ++i)
            {
                // Don't check for collision with main road
                if (m_manager.m_edges[i] == m_manager.m_mainRoad[0]) 
                    continue;

                Vector2 tempEnd;

                if (MathUtility.LineSegmentLineSegmentIntersection(
                    start, 
                    start + MathUtility.Perpendicular(mrDir, m_perpDir) * 10, 
                    m_manager.m_edges[i], 
                    m_manager.m_edges[(i + 1) % m_manager.m_edges.Count], 
                    out tempEnd))
                {
                    tempEnd -= Vector2.one * MathUtility.Perpendicular(mrDir, m_perpDir) * m_roadEdgeAvoidance;
                    if (Vector2.Distance(start, end) > Vector2.Distance(start, tempEnd) || end == Vector3.zero)
                    {
                        end = tempEnd;
                    }
                }
            }

            if(m_initialRoad == null || m_initialRoad.Length() < Vector3.Distance(start, end))
                m_initialRoad = new Road(start, end);
            
            index += m_stepSize;
        }
        
        // If no initial road, redo with alternative perpendicular direction
        if(m_perpDir && m_initialRoad.m_end == Vector3.zero)
        {
            m_perpDir = false;
            Generate();
        }

        m_roadVisualiser.m_roads.Add(m_mainRoad);
        m_roadVisualiser.m_roads.Add(m_initialRoad);
        m_roadVisualiser.m_roads.AddRange(m_roads.ToArray());

        m_roadVisualiser.GenerateRoads();
    }
}
