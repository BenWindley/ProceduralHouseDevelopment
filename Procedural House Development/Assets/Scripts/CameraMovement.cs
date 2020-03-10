using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float m_moveSpeed;
    public float m_zoomSpeed;

    void Update()
    {
        if(Input.GetAxis("Horizontal") != 0 ||
            Input.GetAxis("Vertical") != 0)
        {
            Vector2 move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            transform.Translate(move * Time.deltaTime * m_moveSpeed);
        }

        if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float zoom = Input.GetAxis("Mouse ScrollWheel");

            Camera.main.orthographicSize += zoom * Time.deltaTime * m_zoomSpeed;
        }
    }
}
