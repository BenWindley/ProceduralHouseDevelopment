using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public GameObject m_prompt1;
    public GameObject m_prompt2;
    public GameObject m_prompt3;

    public MapDrawer m_drawer;
    public EdgeSelector m_edgeSelector;
    public Generator m_generator;
    public Stats m_stats;
    public RoadVisualiser m_roadVisualiser;
    public RoadHouseModifier m_roadHouseModifier;

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

                m_prompt1.SetActive(false);
                m_prompt2.SetActive(true);
                m_prompt3.SetActive(false);
                break;

            case 2:
                m_edgeSelector.enabled = false;
                m_generator.enabled = true;

                m_generator.m_mainRoadIndex = m_edgeSelector.m_mainRoadIndex;
                m_mainRoad = m_edgeSelector.m_mainRoad;
                m_generatorSettings.m_enabled = true;

                m_prompt1.SetActive(false);
                m_prompt2.SetActive(false);
                m_prompt3.SetActive(true);
                break;

            case 3:
                m_edgeSelector.m_line.enabled = false;
                m_stats.GetComponent<PanelSlideIn>().m_enabled = true;
                m_roadHouseModifier.Init(m_roadVisualiser);

                m_prompt1.SetActive(false);
                m_prompt2.SetActive(false);
                m_prompt3.SetActive(false);
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
        m_prompt1.SetActive(true);
        m_prompt2.SetActive(false);
        m_prompt3.SetActive(false);
    }
    private void Update()
    {
#if UNITY_STANDALONE
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
#endif
    }

    public void Restart()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
