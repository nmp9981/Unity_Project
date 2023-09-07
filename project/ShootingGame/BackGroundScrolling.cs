using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    [SerializeField] Transform[] m_tfBackgrounds = null;
    float m_speed = 3f;

    float m_leftPosY = 0f;
    float m_rightPosY = 0f;
    // Start is called before the first frame update
    void Start()
    {
        float t_length = m_tfBackgrounds[0].GetComponent<SpriteRenderer>().sprite.bounds.size.y;
        m_leftPosY = -t_length;
        m_rightPosY = t_length;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < m_tfBackgrounds.Length; i++)
        {
            m_tfBackgrounds[i].position += new Vector3(0, m_speed, 0) * Time.deltaTime;

            if (m_tfBackgrounds[i].position.y < m_leftPosY)
            {
                Vector3 t_selfPos = m_tfBackgrounds[i].position;
                t_selfPos.Set(t_selfPos.x, t_selfPos.y + m_rightPosY, t_selfPos.z);
                m_tfBackgrounds[i].position = t_selfPos;
            }
            else m_tfBackgrounds[i].position = new Vector3(0,0, 0);
        }
    }
}
