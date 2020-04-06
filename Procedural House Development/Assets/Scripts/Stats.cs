using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Stats : MonoBehaviour
{
    public TextMeshProUGUI m_detached;
    public TextMeshProUGUI m_semiDetached;
    public TextMeshProUGUI m_terraced;
    public TextMeshProUGUI m_road;

    public void Init(List<House> houses, List<Road> roads)
    {
        int detached = 0;
        int semiDetached = 0;
        int terraced = 0;
        float road = 0;

        foreach (House h in houses)
        {
            switch (h.m_housingType)
            {
                case House.HouseType.Detatched:
                    ++detached;
                    break;
                case House.HouseType.SemiDetatched:
                    ++semiDetached;
                    break;
                case House.HouseType.Terraced:
                    ++terraced;
                    break;
            }
        }
        foreach(Road r in roads)
        {
            road += r.Length();
        }

        m_detached.text = detached.ToString();
        m_semiDetached.text = semiDetached.ToString();
        m_terraced.text = terraced.ToString();
        m_road.text = road.ToString("F1") + "m";
    }
}
