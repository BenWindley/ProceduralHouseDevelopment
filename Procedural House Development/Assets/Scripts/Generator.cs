using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
public class House
{
    public Vector2 p1, p2, p3, p4;

    public bool CheckHouseIntersection(House house)
    {
        return  MathUtility.PointInQuad(house.p1, p1, p2, p3, p4) ||
                MathUtility.PointInQuad(house.p2, p1, p2, p3, p4) ||
                MathUtility.PointInQuad(house.p3, p1, p2, p3, p4) ||
                MathUtility.PointInQuad(house.p4, p1, p2, p3, p4);
    }

    public bool CheckRoadIntersection(Road road)
    {
        return  MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p1, p2) ||
                MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p2, p3) ||
                MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p3, p4) ||
                MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p4, p1);
    }
}

[System.Serializable]
public class Node
{
    public Generator m_generator;

    public bool m_left = true;
    private float m_currentHighestValue = 0.0f;
    public float m_totalValue = 0.0f;
    public int m_depth = 0;

    public Node m_parent;
    public List<Node> m_children = new List<Node>();

    public List<Road> m_roads = new List<Road>();
    public List<Road> m_edges = new List<Road>();

    public List<House> m_houses = new List<House>();

    public void FillChildren(float roadIntervals, int maxDepth)
    {
        if (m_depth >= maxDepth)
            return;

        FillSide(roadIntervals, true);
        FillSide(roadIntervals, false);

        foreach (Node c in m_children)
            c.FillChildren(roadIntervals, maxDepth);
    }

    private void FillSide(float roadIntervals, bool left = true)
    {
        for(int pi = 0; pi < m_roads.Count; ++pi)
        {
            if (m_depth > 0 && pi == 0) continue; 

            // Current parent road
            Road p = m_roads[pi];

            Vector3 dir = MathUtility.Perpendicular((p.m_end - p.m_start).normalized) * (left ? 1 : -1);

            int segments = Mathf.RoundToInt(p.Length() / roadIntervals);

            for (int i = 0; i <= segments; ++i)
            {
                if (pi == 1 && i == 0) continue;

                Vector2 start = p.m_start + (p.m_end - p.m_start).normalized * roadIntervals * i;
                Vector2 end = start + Vector2.one * dir * 100;
                Vector2 tempEnd;

                bool clear = false;

                // Check proposed road again edges
                foreach (Road o in m_edges)
                {
                    if (MathUtility.LineSegmentLineSegmentIntersection(
                    start, end, o.m_start, o.m_end, out tempEnd))
                    {
                        end = tempEnd;
                        end -= Vector2.one * dir * (Vector2.Distance(start, end) % roadIntervals);

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
                    if ((Vector2.one * o.m_start == start &&
                         Vector2.one * o.m_end == end   ) ||
                        (Vector2.one * o.m_start == end &&
                         Vector2.one * o.m_end == start   ))
                    {
                        clear = false;
                    }

                    // Invalid if line is inside another line
                    if (MathUtility.PointOnLineSegment(o.m_start, o.m_end, start) &&
                       MathUtility.PointOnLineSegment(o.m_start, o.m_end, end))
                        clear = false;
                }

                if (clear && Vector2.Distance(start, end) > roadIntervals)
                {
                    Node c = new Node();

                    c.m_generator = m_generator;
                    c.m_parent = this;
                    c.m_roads = new List<Road>(m_roads);
                    c.m_roads.Add(new Road(start, end));
                    c.m_edges = m_edges;
                    c.FillBuildings();
                    c.m_depth = m_depth + 1;

                    // TODO Add value based on lower longest road length
                    c.m_totalValue = m_totalValue + new Road(start, end).Length();

                    if (c.m_totalValue > m_currentHighestValue || m_depth == 1)
                    {
                        m_currentHighestValue = c.m_totalValue;
                        m_children.Add(c);
                    }
                }
            }
        }
    }

    public void FillBuildings()
    {
        m_houses.Clear();

        // Remove new 
        foreach(House h in m_houses)
            foreach(Road r in m_roads)
                if (h.CheckRoadIntersection(r))
                    m_houses.Remove(h);

        for (int j = 1; j < m_roads.Count; ++j)
        {
            Road r = m_roads[j];

            int segments = Mathf.RoundToInt((r.Length() - m_generator.m_houseHeight) / (m_generator.m_houseWidth + m_generator.m_houseOffset));

            Vector2 ver = MathUtility.Perpendicular((r.m_end - r.m_start).normalized) * (m_generator.m_perpDirLeft ? 1 : -1);
            Vector2 hor = (r.m_end - r.m_start).normalized;

            for (int dir = 0; dir < 2; ++dir)
            {
                ver *= dir == 0 ? 1 : -1;

                for (int i = 0; i < segments; ++i)
                {
                    Vector2 start = Vector2.one * r.m_start + hor * m_generator.m_houseHeight + hor * (m_generator.m_houseWidth + m_generator.m_houseOffset) * i;

                    House house = new House();

                    house.p1 = start + ver * m_generator.m_houseOffset;
                    house.p2 = house.p1 + ver * m_generator.m_houseHeight;
                    house.p3 = house.p2 + hor * m_generator.m_houseWidth;
                    house.p4 = house.p1 + hor * m_generator.m_houseWidth;

                    bool valid = true;

                    //foreach (Road rTest in m_roads)
                    //    if (!house.CheckRoadIntersection(rTest))
                    //        valid = false;
                    //foreach (House hTest in m_houses)
                    //    if (!house.CheckHouseIntersection(hTest))
                    //        valid = false;

                    if (valid)
                        m_houses.Add(house);
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
    public float m_roadIntervals = 0.5f;
    public Slider m_roadIntervalsSlider;

    public int m_mainRoadIndex;

    public float m_houseWidth;
    public float m_houseHeight;
    public float m_houseOffset;

    public int m_maxDepth = 5;
    public Slider m_maxDepthSlider;

    public bool m_perpDirLeft = true;

    public void Awake()
    {
        m_manager = Camera.main.GetComponent<Manager>();
    }

    public void Generate()
    {
        m_roadIntervals = m_roadIntervalsSlider.value;
        m_maxDepth = Mathf.RoundToInt(m_maxDepthSlider.value);

        m_roadVisualiser.Clear();
        Node initialNode = new Node();

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
         
        initialNode.m_left = m_perpDirLeft;

        initialNode.m_roads.Add(new Road(
            m_manager.m_mainRoad[0],
            m_manager.m_mainRoad[1]));
        initialNode.m_edges = avoid;
        initialNode.m_generator = this;

        initialNode.FillChildren(m_roadIntervals, m_maxDepth);

        Node longest = initialNode.GetMostValuableNode();
        m_roadVisualiser.m_roads.AddRange(longest.m_roads.ToArray());
        m_roadVisualiser.m_houses.AddRange(longest.m_houses.ToArray());
        m_roadVisualiser.GenerateRoads();
        m_manager.NextSection();
    }

    private bool CalculateDirectionOfPerpendicular(List<Road> edges, Vector2 point, Vector2 dir)
    {
        Vector2 end;

        foreach (Road r in edges)
            if (MathUtility.LineSegmentLineSegmentIntersection(point, dir, r.m_start, r.m_end, out end))
                return false;

        return true;
    }
}