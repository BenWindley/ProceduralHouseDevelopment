using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundSlider : MonoBehaviour
{
    public int m_decimalPlaces;

    public void Round(UnityEngine.UI.Slider slider)
    {
        slider.value = Mathf.Round(slider.value * m_decimalPlaces * 10) / (m_decimalPlaces * 10);
    }
}
