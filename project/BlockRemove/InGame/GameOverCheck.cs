using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverCheck : MonoBehaviour
{
    BlockClick _blockClick;
    UIManager _uiManager;
    bool[][] visitCheck;
    Queue<(int, int)> _unionObject = new Queue<(int, int)>();
    
    int[] dr = { -1, 1, 0, 0 };
    int[] dc = { 0, 0, -1, 1 };

    void Awake()
    {
        _blockClick = GameObject.Find("MouseClick").GetComponent<BlockClick>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        visitCheck = new bool[10][]
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
   
    void visitArrayInit()
    {
        for (int i = 0; i < GameManager.Instance.RowCount; i++)
        {
            for (int j = 0; j < GameManager.Instance.ColCount; j++)
            {
                visitCheck[i][j] = false;
            }
        }
    }
    public void GameOver()
    {
        if(!CheckContinueGame() || GameManager.Instance.RestBlockCount == 0)
        {
            _uiManager.GetComponent<UIManager>()._gameOverUI.SetActive(true);//게임 오버 UI 켜기
            _uiManager.GetComponent<UIManager>()._startButton.interactable = true;//시작 버튼 활성화
        }
    }
    bool CheckContinueGame()
    {
        visitArrayInit();
        for (int i = 0; i < GameManager.Instance.RowCount; i++)
        {
            for (int j = 0; j < GameManager.Instance.ColCount; j++)
            {
                if (visitCheck[i][j]) continue;//이미 방문함
                if (_blockClick.blockState[i][j] == -1) continue;//빈 공간
                if(BFSSearch(i, j, _blockClick.blockState[i][j])>=2) return true;//더 진행가능
            }
        }
        return false;//더 진행 불가능
    }
    int BFSSearch(int startRowpos, int startColpos, int colorState)
    {
        //초기화
        _unionObject = new Queue<(int, int)>();
        int _unionCount = 1;
       
        visitCheck[startRowpos][startColpos] = true;//-1은 방문 체크
        _unionObject.Enqueue((startRowpos, startColpos));
       
        //BFS 구현
        while (_unionObject.Count != 0)
        {
            (int r, int c) = _unionObject.Dequeue();

            for (int dir = 0; dir < 4; dir++)
            {
                int nr = r + dr[dir];
                int nc = c + dc[dir];

                if (nr < 0 || nr >= GameManager.Instance.RowCount || nc < 0 || nc >= GameManager.Instance.ColCount) continue;
                if (visitCheck[nr][nc]) continue;
                if (colorState != _blockClick.blockState[nr][nc]) continue;

                _unionObject.Enqueue((nr, nc));
                _unionCount++;
                 visitCheck[nr][nc] = true;
            }
        }
        return _unionCount;
    }
}
