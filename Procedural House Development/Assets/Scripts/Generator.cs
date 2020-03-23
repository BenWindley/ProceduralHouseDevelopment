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
    public enum HouseType
    {
        Detatched,
        SemiDetatched,
        Terraced
    }

    public bool m_leftSideHouse;
    public Vector2[] p = new Vector2[4];

    public Vector2 m_hor;
    public float m_spacing;
    public bool m_leftAttached = false;
    public bool m_rightAttached = false;

    public Vector2 m_checkPoint1;
    public Vector2 m_checkPoint2;

    public HouseType m_housingType;

    public List<House> m_adjacentHouses = new List<House>();

    public void JoinHouse(List<Vector3> edges)
    {
        if(!m_leftAttached)
        {
            bool temp = m_rightAttached;
            m_rightAttached = m_leftAttached;
            m_leftAttached = temp;
        }

        if(m_rightAttached)
        {
            p[0] -= m_hor * m_spacing;
            if (!CheckInsideArea(edges))
                p[0] += m_hor * m_spacing;
            p[1] -= m_hor * m_spacing;
            if (!CheckInsideArea(edges))
                p[1] += m_hor * m_spacing;
        }
        if (m_leftAttached)
        {
            p[2] += m_hor * m_spacing;
            if (!CheckInsideArea(edges))
                p[2] -= m_hor * m_spacing;
            p[3] += m_hor * m_spacing;
            if (!CheckInsideArea(edges))
                p[3] -= m_hor * m_spacing;
        }
    }

    public void CalculateCheckPoints(float checkDist)
    {
        Vector2 dir = MathUtility.Perpendicular(p[1] - p[0], m_leftSideHouse).normalized;
        m_checkPoint1 = (p[0] + p[1]) / 2.0f + dir * checkDist;
        m_checkPoint2 = (p[2] + p[3]) / 2.0f - dir * checkDist;
    }

    public bool CheckHouseIntersection(House house)
    {
        return  MathUtility.PointInQuad(house.p[0], p[0], p[1], p[2], p[3]) ||
                MathUtility.PointInQuad(house.p[1], p[0], p[1], p[2], p[3]) ||
                MathUtility.PointInQuad(house.p[2], p[0], p[1], p[2], p[3]) ||
                MathUtility.PointInQuad(house.p[3], p[0], p[1], p[2], p[3]) ||
                MathUtility.LineSegmentLineSegmentIntersection(house.p[0], house.p[1], p[0],p[1]) ||
                MathUtility.LineSegmentLineSegmentIntersection(house.p[2], house.p[3], p[2], p[3]);
    }

    public bool CheckRoadIntersection(Road road)
    {
        return  MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p[0], p[1]) ||
                MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p[1], p[2]) ||
                MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p[2], p[3]) ||
                MathUtility.LineSegmentLineSegmentIntersection(road.m_start, road.m_end, p[3], p[0]);
    }

    public bool CheckEdgeIntersection(List<Road> edges)
    {
        foreach (Road r in edges)
            if (CheckRoadIntersection(r))
                return true;

        return false;
    }

    public bool CheckInsideArea(List<Vector3> edges)
    {
        foreach (Vector2 point in p)
        {
            if (!MathUtility.PointInPolygon(edges.ToArray(), point))
                return false;
        }

        return true;
    }
}

[System.Serializable]
public class Node
{
    public Generator m_generator;

    public bool m_left = true;
    public float m_totalValue = 0.0f;
    public int m_depth = 0;

    public Node m_parent;
    public List<Node> m_children = new List<Node>();

    public List<Road> m_roads = new List<Road>();
    public List<Road> m_edges = new List<Road>();

    public List<House> m_houses = new List<House>();

    public void FillChildren(int maxDepth)
    {
        if (m_depth >= maxDepth)
            return;

        FillSide(true);
        FillSide(false);

        RemoveLowestBuildings(m_generator.m_branchChildren);

        foreach (Node c in m_children)
            c.FillChildren(maxDepth);
    }

    private void FillSide(bool left = true)
    {
        float roadIntervals = m_generator.m_houseHeight * 2 + m_generator.m_houseOffset * 3;

        for (int pi = 0; pi < m_roads.Count; ++pi)
        {
            //if (m_depth > 0 && pi == 0) continue;

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
                        //end -= Vector2.one * dir * (Vector2.Distance(start, end) % roadIntervals);

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
                         Vector2.one * o.m_end == end) ||
                        (Vector2.one * o.m_start == end &&
                         Vector2.one * o.m_end == start))
                    {
                        clear = false;
                    }

                    // Invalid if line is inside another line
                    if (MathUtility.PointOnLineSegment(o.m_start, o.m_end, start) &&
                        MathUtility.PointOnLineSegment(o.m_start, o.m_end, end))
                    {
                        //clear = false;
                    }
                }

                if (!MathUtility.PointInPolygon(m_generator.m_manager.m_edges.ToArray(), (end + start) / 2.0f))
                    clear = false;

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

                    float totalRoadLength = 0;

                    foreach (Road rl in c.m_roads)
                        totalRoadLength += rl.Length();

                    c.m_totalValue = m_houses.Count + totalRoadLength;

                    if(c.m_totalValue > m_totalValue)
                        m_children.Add(c);
                }
            }
        }
    }

    private void RemoveLowestBuildings(int keepCount)
    {
        while(m_children.Count > keepCount)
        {
            int lowestI = 0;

            for(int i = 0; i < m_children.Count; ++i)
            {
                if (m_children[i].m_totalValue < m_children[lowestI].m_totalValue)
                    lowestI = i;
            }

            m_children.RemoveAt(lowestI);
        }
    }

    public void FillBuildings()
    {
        m_houses.Clear();

        float startOffset = m_generator.m_houseHeight + m_generator.m_houseOffset * 2;

        for (int j = 0; j < m_roads.Count; ++j)
        {
            Road r = m_roads[j];

            int segments = Mathf.FloorToInt((r.Length() - startOffset) / (m_generator.m_houseWidth + m_generator.m_houseOffset));

            Vector2 ver = MathUtility.Perpendicular((r.m_end - r.m_start).normalized) * (m_generator.m_perpDirLeft ? 1 : -1);
            Vector2 hor = (r.m_end - r.m_start).normalized;

            for (int dir = 0; dir < 2; ++dir)
            {
                if (j == 0 && dir == 0)
                    continue;

                ver *= dir == 0 ? 1 : -1;

                for (int i = 0; i < segments; ++i)
                {
                    Vector2 start = Vector2.one * r.m_start + hor * startOffset + hor * (m_generator.m_houseWidth + m_generator.m_houseOffset) * i;

                    House house = new House();

                    house.m_leftSideHouse = dir == 0;

                    house.p[0] = start + ver * m_generator.m_houseOffset;
                    house.p[1] = house.p[0] + ver * m_generator.m_houseHeight;
                    house.p[2] = house.p[1] + hor * m_generator.m_houseWidth;
                    house.p[3] = house.p[0] + hor * m_generator.m_houseWidth;

                    house.m_hor = hor;
                    house.m_spacing = m_generator.m_houseOffset;

                    bool valid = true;

                    foreach (Road rTest in m_roads)
                        if (house.CheckRoadIntersection(rTest))
                            valid = false;
                    foreach (House hTest in m_houses)
                        if (house.CheckHouseIntersection(hTest))
                            valid = false;
                    if (!house.CheckInsideArea(m_generator.m_manager.m_edges))
                        valid = false;

                    if (valid)
                        m_houses.Add(house);
                }
            }
        }

        ClassifyHouses();
    }

    private void ClassifyHouses()
    {
        for (int i = 0; i < m_houses.Count; ++i)
        {
            m_houses[i].CalculateCheckPoints(m_generator.m_houseOffset + (m_generator.m_houseWidth / 2.0f));

            ClassifyHouse(m_houses[i], i);
        }

        for (int i = 0; i < m_houses.Count; ++i)
        {
            m_houses[i].JoinHouse(m_generator.m_manager.m_edges);
        }
    }

    private void ClassifyHouse(House house, int index)
    {
        int attachedSides = 0;

        for (int i = 0; i < m_houses.Count; ++i)
        {
            if (i == index) continue;

            House h = m_houses[i];

            bool left = MathUtility.PointInQuad(house.m_checkPoint1, h.p[0], h.p[1], h.p[2], h.p[3]);
            bool right = MathUtility.PointInQuad(house.m_checkPoint2, h.p[0], h.p[1], h.p[2], h.p[3]);

            if (left || right)
            {
                ++attachedSides;
                house.m_leftAttached = left;
                house.m_rightAttached = right;
            }
        }

        house.m_housingType = (House.HouseType) attachedSides;
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
    public Manager m_manager;
    public RoadVisualiser m_roadVisualiser;

    public int m_mainRoadIndex;

    public Slider m_houseWidthSlider;
    public float m_houseWidth;
    public Slider m_houseHeightSlider;
    public float m_houseHeight;
    public float m_houseOffset;

    public int m_maxDepth = 5;
    public Slider m_maxDepthSlider;

    public int m_branchChildren = 1;

    public bool m_perpDirLeft = true;

    public void Awake()
    {
        m_manager = Camera.main.GetComponent<Manager>();
    }

    public void Generate()
    {
        m_houseWidth = m_houseWidthSlider.value;
        m_houseHeight = m_houseHeightSlider.value;

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

        initialNode.FillChildren(m_maxDepth);

        Node longest = initialNode.GetMostValuableNode();
        m_roadVisualiser.m_roads.AddRange(longest.m_roads.ToArray());
        m_roadVisualiser.m_houses.AddRange(longest.m_houses.ToArray());
        m_roadVisualiser.GenerateRoads();
        m_manager.m_stats.InitStats(longest.m_houses, longest.m_roads);
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