using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackGroundScrolling : MonoBehaviour
{
    GameObject[] m_tfBackgrounds = new GameObject[3];
    Sprite[] useBackground = new Sprite[6];
    float viewHeight;//카메라 높이

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize*3;//카메라 높이

        m_tfBackgrounds[0] = GameObject.Find("BackGround0");
        m_tfBackgrounds[1] = GameObject.Find("BackGround1");
        m_tfBackgrounds[2] = GameObject.Find("BackGround2");

        useBackground = Resources.LoadAll<Sprite>("BackGroundMap");
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_tfBackgrounds.Length; i++)
        {
            //계속 내려감
            m_tfBackgrounds[i].gameObject.transform.position += (Vector3.down * GamaManager.Instance.PlayerSpeed * Time.deltaTime);

            //위로 배경을 붙인다.
            if (m_tfBackgrounds[i].gameObject.transform.position.y < -viewHeight)
            {
                Vector3 nextPos = m_tfBackgrounds[i].gameObject.transform.position;
                nextPos = new Vector3(nextPos.x, nextPos.y + 2 * viewHeight, nextPos.z);
                m_tfBackgrounds[i].gameObject.transform.position = nextPos;
            }
        }
    }
    public void BackGroundChange(int stageNum)
    {
        if (stageNum < 6)
        {
            for(int i = 0; i < m_tfBackgrounds.Length; i++)
            {
                int backNum = stageNum-1;
                m_tfBackgrounds[i].GetComponent<SpriteRenderer>().sprite = useBackground[backNum];
            }
        }
        else
        {
            for (int i = 0; i < m_tfBackgrounds.Length; i++)
            {
                m_tfBackgrounds[i].GetComponent<SpriteRenderer>().sprite = useBackground[5];
            }
        }
    }
}
