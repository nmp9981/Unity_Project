using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Unity.Collections.AllocatorManager;

public class BlockClick : MonoBehaviour
{
    BlockSpawn _blockSpawn;

    public int[][] blockState;
    bool[][] visitState;
    Queue<(int,int)> _unionObject = new Queue<(int,int)>();
    public List<(int, int)> _unionObjectPosition = new List<(int, int)>();

    int[] dr = { -1, 1, 0, 0 };
    int[] dc = { 0, 0, -1, 1 };

    void Awake()
    {
        _blockSpawn = GameObject.Find("BlockSpawner").GetComponent<BlockSpawn>();
        ArrayInstiate();
    }

    void Update()
    {
        MouseClick();
    }
    void ArrayInstiate()
    {
        blockState = new int[10][]
        {
             new int[20],
             new int[20],
             new int[20],
             new int[20],
             new int[20],
             new int[20],
             new int[20],
             new int[20],
             new int[20],
             new int[20],
        };
        visitState = new bool[10][]
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
                visitState[i][j] = false;
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

                NearObjectSearch((int)(4-hit.transform.position.y), (int)(hit.transform.position.x+10),BlockSpawn.BlockState(hit.transform.gameObject.name));
            }
        }
    }
    void NearObjectSearch(int startRowpos,int startColpos, int objColor)
    {
        //초기화
        _unionObject = new Queue<(int, int)>();
        _unionObjectPosition = new List<(int, int)>();
        visitInit();
        
        visitState[startRowpos][startColpos] = true;//y x순, -1은 방문 체크
        _unionObject.Enqueue((startRowpos,startColpos));
        _unionObjectPosition.Add((startRowpos, startColpos));
       
        //BFS 구현
        while (_unionObject.Count != 0)
        {
            (int r, int c) = _unionObject.Dequeue();

            for(int dir = 0; dir < 4; dir++)
            {
                int nr = r + dr[dir];
                int nc = c + dc[dir];

                if (nr < 0 || nr >= GameManager.Instance.RowCount || nc < 0 || nc >= GameManager.Instance.ColCount) continue;
                if (visitState[nr][nc]) continue;
                if (objColor != blockState[nr][nc]) continue;

                _unionObject.Enqueue((nr, nc));
                _unionObjectPosition.Add((nr, nc));
                visitState[nr][nc] = true;
            }
        }
        Debug.Log(_unionObjectPosition.Count+"개수");
        if (_unionObjectPosition.Count >= 2)
        {
            RemoveBlock();
            BlockReSetting();
        }
    }
    void RemoveBlock()
    {
        foreach(var removePos in _unionObjectPosition)
        {
            blockState[removePos.Item1][removePos.Item2] = 0;
        }
    }
    void BlockReSetting()
    {
        //각 열별로
        for(int c = 0; c < GameManager.Instance.ColCount; c++)
        {
            //각 열별로 남아있는 블럭 상태
            Stack<int> colBlock = new Stack<int>();
            for(int r = 0; r < GameManager.Instance.RowCount; r++)
            {
                if (blockState[r][c] >= 1) colBlock.Push(blockState[r][c]);
            }
            //재배치
            for (int r = GameManager.Instance.RowCount-1; r>=0; r--)
            {
                if (colBlock.Count == 0) blockState[r][c] = -1;
                else
                {
                    blockState[r][c] = colBlock.Peek();
                    colBlock.Pop();
                }
            }
        }
        _blockSpawn.BlockBlockSetting();
    }
}
