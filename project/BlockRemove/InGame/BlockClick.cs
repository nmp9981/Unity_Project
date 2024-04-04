using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
                //Debug.Log(hit.transform.gameObject.name);
                //Debug.Log(hit.transform.position.x+"  "+ hit.transform.position.y);

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
        if (_unionObjectPosition.Count >= 2)
        {
            RemoveBlock();
            BlockReSetting();
        }
    }
    //블록 제거
    void RemoveBlock()
    {
        foreach(var removePos in _unionObjectPosition)
        {
            blockState[removePos.Item1][removePos.Item2] = 0;
        }
        GameManager.Instance.RestBlockCount -= _unionObjectPosition.Count;//남은 블록 개수
    }
    //블록 재세팅
    void BlockReSetting()
    {
        List<Stack<int>> restBlockState = new List<Stack<int>>();//남은 블록의 상태
        //각 열별로
        for (int c = 0; c < GameManager.Instance.ColCount; c++)
        {
            Stack<int> colBlock = new Stack<int>();//각 열별로 남아있는 블럭 상태
            for (int r = 0; r < GameManager.Instance.RowCount; r++)
            {
                if (blockState[r][c] >= 1) colBlock.Push(blockState[r][c]);
            }
            if (colBlock.Count > 0) restBlockState.Add(colBlock);
        }
        //재배치
        for (int c = 0; c < restBlockState.Count; c++)
        {
            for (int r = GameManager.Instance.RowCount - 1; r >= 0; r--)
            {
                if (restBlockState[c].Count == 0) blockState[r][c] = -1;
                else
                {
                    blockState[r][c] = restBlockState[c].Peek();
                    restBlockState[c].Pop();
                }
            }
        }
        //나머지 빈칸은 모두 -1로
        for (int c = restBlockState.Count; c < GameManager.Instance.ColCount; c++)
        {
            for (int r = GameManager.Instance.RowCount - 1; r >= 0; r--) blockState[r][c] = -1;
        }
        /*
        List<int> colCount = new List<int>();//각 열별로 남은 블럭 개수
        //각 열별로
        for (int c = 0; c < GameManager.Instance.ColCount; c++)
        {
            Stack<int> colBlock = new Stack<int>();//각 열별로 남아있는 블럭 상태
            for(int r = 0; r < GameManager.Instance.RowCount; r++)
            {
                if (blockState[r][c] >= 1) colBlock.Push(blockState[r][c]);
            }
            colCount.Add(colBlock.Count);
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

        //빈 열이 있으면 왼쪽으로
        int emptyCount = 0;
        int emptyColCount = 0;//빈 열의 총 개수
        for (int idx = 0; idx < colCount.Count; idx++)
        {
            if (colCount[idx] == 0)
            {
                emptyCount++;
                emptyColCount++;
            }
            else
            {
                for (int c = idx; c < GameManager.Instance.ColCount; c++)
                {
                    for (int r = GameManager.Instance.RowCount - 1; r >= 0; r--)
                    {
                        blockState[r][c - emptyCount] = blockState[r][c];
                    }
                }
                emptyCount = 0;
            }
        }
        //나머지 빈칸은 모두 -1로
        for (int c = GameManager.Instance.ColCount-1;c>= GameManager.Instance.ColCount-emptyColCount; c--)
        {
            for (int r = GameManager.Instance.RowCount - 1; r >= 0; r--) blockState[r][c] = -1;
        }
        */
        _blockSpawn.BlockBlockSetting();
    }
}
