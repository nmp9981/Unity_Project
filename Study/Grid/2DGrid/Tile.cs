using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int xIdx;
    public int yIdx;
    public int clickCount;

    private Board board;

    [SerializeField]
    TextMeshProUGUI tileText;

    void Start() { }

    public void Init(int x, int y,int count, Board board_input)
    {
        xIdx = x;
        yIdx = y;
        board = board_input;
        clickCount = count;
    }
    /// <summary>
    /// 타일에 글자 넣기
    /// </summary>
    /// <param name="xIdx">x좌표</param>
    /// <param name="yIdx">y좌표</param>
    public void MappingTextTile(int xIdx, int yIdx)
    {
        Vector2 changeUIPos = Camera.main.WorldToScreenPoint(new Vector2(xIdx, yIdx));
        tileText.transform.position = changeUIPos;
        tileText.text = $"{xIdx}, {yIdx}";
    }
}
