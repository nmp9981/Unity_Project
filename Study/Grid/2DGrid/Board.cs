using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class Board : MonoBehaviour
{
    public int width; //가로 길이 
    public int height;//세로 길이
    public GameObject tilePrefab;//타일 오브젝트
    private Tile[,] board;//보드판

    public int boardSize;//여백 크기
    void Start()
    {
        SetUPBoard();//보드판 크기 설정
        SetUpCamera();//카메라 세팅
        SetupTiles();//타일 배치
    }
    private void Update()
    {
        MouseClickFunction();
    }

    /// <summary>
    /// 보드판 크기설정
    /// </summary>
    void SetUPBoard()
    {
        board = new Tile[width, height];
    }
    /// <summary>
    /// 타일 배치
    /// </summary>
    void SetupTiles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //(i,j)에 타일 생성
                GameObject tile = Instantiate(tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;
                //오브젝트 명 설정
                tile.name = "Tile (" + i + "," + j + ")";
                //타일 오브젝트 관리를 위해 부모 오브젝트로 묶는다.
                tile.transform.parent = transform;
                //타일에 텍스트 넣기
                tile.GetComponent<Tile>().MappingTextTile(i, j);
            }
        }
    }
    /// <summary>
    /// 카메라 세팅
    /// </summary>
    void SetUpCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width - 1) / 2f, (float)(height - 1) / 2f, -10f);

        float aspecRatio = (float) Screen.width/ Screen.height;
        float verticalSize = (float)height * 0.5f + boardSize;
        float horizonSize = ((float)width * 0.5f + boardSize) / aspecRatio;

        Camera.main.orthographicSize = (verticalSize>horizonSize)? verticalSize : horizonSize;
    }

    /// <summary>
    /// 마우스 클릭시 작동하는 기능
    /// </summary>
    private void MouseClickFunction()
    {
        if(Input.GetMouseButtonDown(0)){
            Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // Raycast함수를 통해 부딪치는 collider를 hit에 리턴받습니다.
            RaycastHit2D hit = Physics2D.Raycast(pos, Vector2.zero);

            //선택 오브젝트 없음
            if (hit.collider == null)
            {
                return;
            }

            //타일 선택
            if (hit.collider.gameObject.tag == "Tile")
            {
                GameObject gm = hit.collider.gameObject;
                ChangeColor(gm);
            }
        }
       
    }

    /// <summary>
    /// 색상 변경
    /// </summary>
    /// <param name="gm">변경할 오브젝트</param>
    void ChangeColor(GameObject gm)
    {
        SpriteRenderer spriteRenderer = gm.GetComponent<SpriteRenderer>();
        Tile tile = gm.GetComponent<Tile>();
        tile.clickCount += 1;

        if (tile.clickCount % 2 == 1)
        {
            spriteRenderer.color = Color.yellow;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }

    }
}
