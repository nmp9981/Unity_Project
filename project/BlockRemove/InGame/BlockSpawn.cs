using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum BlockColor
{
    NULL,Red,Orange,Yellow,Green,Sky,Blue,Pupple,Black
}
public class BlockSpawn : MonoBehaviour
{
    ObjectPulling _objectPulling;
    BlockClick _blockClick;
    GameOverCheck _gameOverCheck;
    UIManager _uiManager;
    private void Awake()
    {
        _blockClick = GameObject.Find("MouseClick").GetComponent<BlockClick>();
        _gameOverCheck = GameObject.Find("GameCheck").GetComponent<GameOverCheck>();
        _uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        _objectPulling = GameObject.Find("ObjectPulling").GetComponent<ObjectPulling>();
    }
    
    public void BlockInitSetting()
    {
        _objectPulling.OffObj();
        SoundManager._sound.PlaySfx((int)SFXSound.Start);//효과음 재생
        _uiManager.GetComponent<UIManager>()._gameOverUI.SetActive(false);//게임 오버 UI 끄기
        _uiManager.GetComponent<UIManager>()._getReadyUI.SetActive(false);//게임 준비 UI 끄기
        _uiManager.GetComponent<UIManager>()._startButton.interactable = false;//시작 버튼 비활성화
        for (int i= -GameManager.Instance.RowCount / 2; i < GameManager.Instance.RowCount / 2; i++)
        {
            for(int j = -GameManager.Instance.ColCount / 2; j < GameManager.Instance.ColCount / 2; j++)
            {
                int blockNum = Random.Range(0, GameManager.Instance.BlockKinds);
                GameObject gm = _objectPulling.MakeObj(blockNum);
                gm.transform.position = new Vector3(j,i,0);
                _blockClick.blockState[(int)(4 - i)][(int)(j + 10)] = BlockState(gm.gameObject.name);
            }
        }
        GameManager.Instance.RestBlockCount = 200;//블록 남은 개수 초기화
        GameManager.Instance.Score = 0;//점수 초기화
    }
    public void BlockBlockSetting()
    {
        _objectPulling.OffObj();
        for (int i = -GameManager.Instance.RowCount / 2; i < GameManager.Instance.RowCount / 2; i++)
        {
            for (int j = -GameManager.Instance.ColCount / 2; j < GameManager.Instance.ColCount / 2; j++)
            {
                int blockNum = _blockClick.blockState[4-i][j+10];//여기만 수정(4/4)
                if (blockNum == -1) continue;//빈 공간

                GameObject gm = _objectPulling.MakeObj(blockNum-1);//blockNum : 1~8
                gm.transform.position = new Vector3(j, i, 0);
            }
        }
        _gameOverCheck.GameOver();//게임 오버 판정
    }
    public static int BlockState(string name)
    {
        switch (name.Substring(0,name.Length-12))
        {
            case "Red":
                return 1;
            case "Select":
                return 6;
            case "Yellow":
                return 2;
            case "Green":
                return 3;
            case "Sky":
                return 8;
            case "Blue":
                return 4;
            case "Pupple":
                return 5;
            case "Black":
                return 7;
            case null:
                return 0;
        }
        return 0;
    }
}
