using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    [SerializeField] Transform[] m_tfBackgrounds = null;//배경 배열
    [SerializeField] float m_speed = 0f;//스크롤링 속도

    float m_leftPosX = 0f;//사라질 위치
    float m_rightPosX = 0f;//다시 나올 위치
    // Start is called before the first frame update
    void Start()
    {
        float t_length = m_tfBackgrounds[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x;
        m_leftPosX = -t_length;
        m_rightPosX = t_length * m_tfBackgrounds.Length;//이미지 길이*개수 = 총 배경의 길이

    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < m_tfBackgrounds.Length; i++)
        {
            m_tfBackgrounds[i].position += new Vector3(m_speed, 0, 0) * Time.deltaTime;

            //왼쪽으로 넘어가면 오른쪽꺼를 다시 등장시킴
            if (m_tfBackgrounds[i].position.x < m_leftPosX)
            {
                Vector3 t_selfPos = m_tfBackgrounds[i].position;
                t_selfPos.Set(t_selfPos.x + m_rightPosX, t_selfPos.y, t_selfPos.z);
                m_tfBackgrounds[i].position = t_selfPos;
            }
        }
    }
}
