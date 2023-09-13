using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScrolling : MonoBehaviour
{
    [SerializeField] Transform[] m_tfBackgrounds;//씬이 재업로드되면서 파괴됨
    [SerializeField] Sprite[] useBackground;
    float viewHeight;//카메라 높이

    private void Awake()
    {
        viewHeight = Camera.main.orthographicSize*3;//카메라 높이
        //DontDestroyOnLoad(this.gameObject);//파괴방지
    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_tfBackgrounds.Length; i++)
        {
            //계속 내려감
            m_tfBackgrounds[i].position += (Vector3.down * GamaManager.Instance.PlayerSpeed * Time.deltaTime);

            //위로 배경을 붙인다.
            if (m_tfBackgrounds[i].position.y < -viewHeight)
            {
                Vector3 nextPos = m_tfBackgrounds[i].position;
                nextPos = new Vector3(nextPos.x, nextPos.y + 2 * viewHeight, nextPos.z);
                m_tfBackgrounds[i].position = nextPos;
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
