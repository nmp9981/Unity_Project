using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] int nextPortalNum;
    [SerializeField] GameObject player;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && PortalManager.PortalInstance.Curtime>= PortalManager.PortalInstance.Cooltime)
        {
            float dist = Vector3.Magnitude(player.transform.position - gameObject.transform.position);
            if (dist < 2)
            {
                SoundManager._sound.PlaySfx(0);
                PortalManager.PortalInstance.Curtime = 0;
                ChangeMap(nextPortalNum);
                player.transform.position = PortalManager.PortalInstance.portalist[nextPortalNum].transform.position;
            }
        }
    }
    void ChangeMap(int nextPortalNum)
    {
        int beforeBGMNymber = GameManager.Instance.PlayerBGMNumber;//이전 bgm 번호
        switch (nextPortalNum)
        {
            case 0:
                GameManager.Instance.PlayerCurrentMap = 0;
                GameManager.Instance.PlayerBGMNumber = 0;
                break;
            case int n when(n>=1 && n<=2):
                GameManager.Instance.PlayerCurrentMap = 1;
                GameManager.Instance.PlayerBGMNumber = 0;
                break;
            case int n when (n>=3 && n <= 4):
                GameManager.Instance.PlayerCurrentMap = 2;
                GameManager.Instance.PlayerBGMNumber = 0;
                break;
            case int n when (n >= 5 && n <= 7):
                GameManager.Instance.PlayerCurrentMap = 3;
                GameManager.Instance.PlayerBGMNumber = 1;
                break;
            case int n when (n >= 8 && n <= 9):
                GameManager.Instance.PlayerCurrentMap = 4;
                GameManager.Instance.PlayerBGMNumber = 2;
                break;
            case int n when (n >= 10 && n <= 11):
                GameManager.Instance.PlayerCurrentMap = 5;
                GameManager.Instance.PlayerBGMNumber = 3;
                break;
            case int n when (n >= 12 && n <= 13):
                GameManager.Instance.PlayerCurrentMap = 6;
                GameManager.Instance.PlayerBGMNumber = 4;
                break;
            case int n when (n >= 14 && n <= 15):
                GameManager.Instance.PlayerCurrentMap = 7;
                GameManager.Instance.PlayerBGMNumber = 5;
                break;
            case 16:
                GameManager.Instance.PlayerCurrentMap = 8;
                GameManager.Instance.PlayerBGMNumber = 6;
                break;
        }
        //새로운 BGM 재생
        if(beforeBGMNymber != GameManager.Instance.PlayerBGMNumber)
        {
            SoundManager._sound.StopBGM(beforeBGMNymber);
            SoundManager._sound.PlayBGM(GameManager.Instance.PlayerBGMNumber);
        }
    }
}
