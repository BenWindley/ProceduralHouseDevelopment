using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public MapDrawer m_drawer;
    public EdgeSelector m_edgeSelector;

    public int m_currentStage = 0;

    public void NextSection()
    {
        m_currentStage++;

        switch(m_currentStage)
        {
            case 1:
                m_drawer.enabled = false;
                m_edgeSelector.enabled = true;
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
