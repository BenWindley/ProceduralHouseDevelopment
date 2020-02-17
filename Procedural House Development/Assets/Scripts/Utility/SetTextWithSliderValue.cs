using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SetTextWithSliderValue : MonoBehaviour
{
    public TextMeshProUGUI m_text;
    public UnityEngine.UI.Slider m_slider;

    void Start()
    {
        m_text.text = (Mathf.RoundToInt(m_slider.value * 10) / 10.0f).ToString();
    }

    void Update()
    {
        m_text.text = (Mathf.RoundToInt(m_slider.value * 10) / 10.0f).ToString();
    }
}
