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
        switch (nextPortalNum)
        {
            case int n when(n<=2):
                GameManager.Instance.PlayerCurrentMap = 0;
                SoundManager._sound.PlayBGM(0);
                break;
            case int n when (n>=3 && n <= 4):
                GameManager.Instance.PlayerCurrentMap = 1;
                SoundManager._sound.PlayBGM(0);
                break;
            case int n when (n >= 5 && n <= 7):
                GameManager.Instance.PlayerCurrentMap = 2;
                SoundManager._sound.PlayBGM(1);
                break;
            case int n when (n >= 8 && n <= 9):
                GameManager.Instance.PlayerCurrentMap = 3;
                SoundManager._sound.PlayBGM(2);
                break;
            case int n when (n >= 10 && n <= 11):
                GameManager.Instance.PlayerCurrentMap = 4;
                SoundManager._sound.PlayBGM(3);
                break;
            case int n when (n >= 12 && n <= 13):
                GameManager.Instance.PlayerCurrentMap = 5;
                SoundManager._sound.PlayBGM(4);
                break;
            case int n when (n >= 14 && n <= 15):
                GameManager.Instance.PlayerCurrentMap = 6;
                SoundManager._sound.PlayBGM(5);
                break;
            case 16:
                GameManager.Instance.PlayerCurrentMap = 7;
                SoundManager._sound.PlayBGM(6);
                break;
        }
    }
}
