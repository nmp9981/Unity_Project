using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BlockClick : MonoBehaviour
{
    [SerializeField] GameObject searchCube;//탐색용 큐브

    bool[][] visit; 
   
    void Awake()
    {
        VisitArrayInstiate();
    }

    // Update is called once per frame
    void Update()
    {
        MouseClick();
    }
    void VisitArrayInstiate()
    {
        visit = new bool[10][]
        {
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
             new bool[20],
        };
    }

    void visitInit()
    {
        for (int i = 0; i < GameManager.Instance.RowCount; i++)
        {
            for (int j = 0; j < GameManager.Instance.ColCount; j++)
            {
                visit[i][j] = false;
            }
        }
    }
    void MouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);//마우스로 클릭한 좌표값 가져오기

            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero,0f);
            if (hit.collider != null)
            {
                Debug.Log(hit.transform.gameObject.name);
                Debug.Log(hit.transform.position.x+"  "+ hit.transform.position.y);

                visitInit();
                NearObjectSearch(hit.transform.position.x, hit.transform.position.y,hit.transform.gameObject.name);
            }
        }
    }
    void NearObjectSearch(float startXpos,float startYpos, string objName)
    {
        GameObject pivot = Instantiate(searchCube);
        pivot.transform.position = new Vector3(startXpos, startYpos, 0f);
        
        visit[(int)(4-startYpos)][(int)startXpos+10] = true;//y x순


        Destroy(pivot);
    }
}
