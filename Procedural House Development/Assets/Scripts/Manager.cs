using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public MapDrawer m_drawer;
    public EdgeSelector m_edgeSelector;
    public Generator m_generator;

    public PanelSlideIn m_generatorSettings;

    public int m_currentStage = 0;

    public List<Vector3> m_edges;
    public List<Vector3> m_mainRoad;

    public void NextSection()
    {
        m_currentStage++;

        switch(m_currentStage)
        {
            case 1:
                m_drawer.enabled = false;
                m_edgeSelector.enabled = true;

                m_edges = m_drawer.m_edges;
                break;

            case 2:
                m_edgeSelector.enabled = false;
                m_generator.enabled = true;

                m_generator.m_mainRoadIndex = m_edgeSelector.m_mainRoadIndex;
                m_mainRoad = m_edgeSelector.m_mainRoad;
                m_generatorSettings.m_enabled = true;
                break;

            case 3:
                m_edgeSelector.m_line.enabled = false;
                break;
        }
    }

    public void PreviousSection()
    {
        m_currentStage = Mathf.Max(0, --m_currentStage);
    }

    void Start()
    {
        m_drawer.enabled = true;
    }
}
