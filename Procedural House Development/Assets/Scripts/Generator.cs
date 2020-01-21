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

[System.Serializable]
public class Node
{
    public bool m_left = true;
    private float m_currentHighestValue = 0.0f;
    public float m_totalValue = 0.0f;
    public int m_depth = 0;

    public Node m_parent;
    public List<Node> m_children = new List<Node>();

    public List<Road> m_roads = new List<Road>();
    public List<Road> m_edges = new List<Road>();

    public void FillChildren(float stepSize, float minRoadLength, float roadEdgeAvoidance, int maxDepth)
    {
        if (m_depth >= maxDepth)
            return;

        FillSide(stepSize, minRoadLength, roadEdgeAvoidance, true);
        FillSide(stepSize, minRoadLength, roadEdgeAvoidance, false);

        foreach (Node c in m_children)
            c.FillChildren(stepSize, minRoadLength, roadEdgeAvoidance, maxDepth);
    }

    private void FillSide(float stepSize, float minRoadLength, float roadEdgeAvoidance, bool left = true)
    {
        for(int pi = 0; pi < m_roads.Count; ++pi)
        {
            if (m_depth > 0 && pi == 0) continue; 

            // Current parent road
            Road p = m_roads[pi];

            Vector3 dir = MathUtility.Perpendicular((p.m_end - p.m_start).normalized) * (left ? 1 : -1);

            for (int i = 0; i <= 1.0f / stepSize; ++i)
            {
                if (pi == 1 && i == 0) continue;

                Vector2 start = Vector3.Lerp(p.m_start, p.m_end, i * stepSize);
                Vector2 end = start + Vector2.one * dir * 100;
                Vector2 tempEnd = end;

                bool clear = false;

                // Check proposed road again edges
                foreach (Road o in m_edges)
                {
                    if (MathUtility.LineSegmentLineSegmentIntersection(
                    start, end, o.m_start, o.m_end, out tempEnd))
                    {
                        end = tempEnd;
                        end -= Vector2.one * dir * roadEdgeAvoidance;

                        clear = true;
                    }
                }

                if (!clear)
                    continue;

                // Check proposed road against other roads
                foreach (Road o in m_roads)
                {
                    // Don't check intersection with parent road
                    if (p.m_start == o.m_start && p.m_end == o.m_end)
                        continue;

                    //Check if intersecting other road
                    if (MathUtility.LineSegmentLineSegmentIntersection(
                    start, end, o.m_start, o.m_end, out tempEnd))
                    {
                        end = tempEnd;

                        clear = true;
                    }
                }

                // Check if propposed road is invalid
                foreach (Road o in m_roads)
                {
                    // Ignore checks on parent road
                    if (p.m_start == o.m_start && p.m_end == o.m_end)
                        continue;

                    // Invalid if both roads have the same start point
                    if (Vector2.one * o.m_start == start ||
                        Vector2.one * o.m_end == start ||
                        Vector2.one * o.m_start == end ||
                        Vector2.one * o.m_end == end)
                        clear = false;

                    // Invalid if line is inside another line
                    if (MathUtility.PointOnLineSegment(o.m_start, o.m_end, start) &&
                       MathUtility.PointOnLineSegment(o.m_start, o.m_end, end))
                        clear = false;
                }

                if (clear && Vector2.Distance(start, end) > minRoadLength)
                {
                    Node c = new Node();

                    c.m_parent = this;
                    c.m_roads = new List<Road>(m_roads);
                    c.m_roads.Add(new Road(start, end));
                    c.m_edges = m_edges;
                    c.m_depth = m_depth + 1;

                    // TODO Add value based on lower longest road length
                    c.m_totalValue = m_totalValue + new Road(start, end).Length();

                    if (c.m_totalValue > m_currentHighestValue)
                    {
                        m_currentHighestValue = c.m_totalValue;
                        m_children.Add(c);
                    }
                }
            }
        }
    }

    public Node GetMostValuableNode()
    {
        Node mostValuable;

        if (m_children.Count > 0)
        {
            mostValuable = m_children[0].GetMostValuableNode();

            foreach (Node n in m_children)
            {
                Node c = n.GetMostValuableNode();

                if (c.m_totalValue > mostValuable.m_totalValue)
                {
                    mostValuable = c;
                }
            }
        }
        else
        {
            mostValuable = this;
        }

        return mostValuable;
    }
}

public class Generator : MonoBehaviour
{
    private Manager m_manager;
    public RoadVisualiser m_roadVisualiser;

    // Buffer between the edges of the main road, between 0 for no buffer and 0.5 for only center
    public float m_roadEdgeAvoidance = 0.05f;
    public float m_minimumRoadLength = 0.1f;
    public float m_roadStepSize = 0.05f;

    public int m_mainRoadIndex;

    public Node m_initialNode = new Node();

    public int m_maxDepth = 5;

    public bool m_perpDirLeft = true;

    public void Awake()
    {
        m_manager = Camera.main.GetComponent<Manager>();
    }

    public void Generate()
    {
        List<Road> avoid = new List<Road>();

        for (int i = 0; i < m_manager.m_edges.Count; ++i)
        {
            if(i != m_mainRoadIndex)
                avoid.Add(new Road(m_manager.m_edges[i], m_manager.m_edges[(i + 1) % m_manager.m_edges.Count]));
        }

        m_perpDirLeft = CalculateDirectionOfPerpendicular(
            avoid, 
            Vector3.Lerp(m_manager.m_mainRoad[0], m_manager.m_mainRoad[1], 0.5f), 
            MathUtility.Perpendicular((m_manager.m_mainRoad[1] - m_manager.m_mainRoad[0]).normalized * 100, true)
            );
         
        m_initialNode.m_left = m_perpDirLeft;
        Vector3 mrDir = (m_manager.m_mainRoad[1] - m_manager.m_mainRoad[0]).normalized;

        m_initialNode.m_roads.Add(new Road(
            m_manager.m_mainRoad[0],
            m_manager.m_mainRoad[1]));
        m_initialNode.m_edges = avoid;

        m_initialNode.FillChildren(m_roadStepSize, m_minimumRoadLength, m_roadEdgeAvoidance, m_maxDepth);

        Node longest = m_initialNode.GetMostValuableNode();
        m_roadVisualiser.m_roads.AddRange(longest.m_roads.ToArray());
        m_roadVisualiser.GenerateRoads();
    }

    private bool CalculateDirectionOfPerpendicular(List<Road> edges, Vector2 point, Vector2 dir)
    {
        Vector2 end = Vector2.zero;

        foreach (Road r in edges)
            if (MathUtility.LineSegmentLineSegmentIntersection(point, dir, r.m_start, r.m_end, out end))
                return false;

        return true;
    }
}