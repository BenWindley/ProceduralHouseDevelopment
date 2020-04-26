using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeMaterial : MonoBehaviour
{
    public float m_progress;
    public float m_target;
    public float m_speed = 1.0f;
    public Material m_mat;

    void Start()
    {
        m_mat = GetComponent<MeshRenderer>().sharedMaterial;
    }

    void Update()
    {
        m_progress = Mathf.MoveTowards(m_progress, m_target, Time.deltaTime * m_speed);
        m_mat.color = new Color(1, 1, 1, m_progress);
    }
}
