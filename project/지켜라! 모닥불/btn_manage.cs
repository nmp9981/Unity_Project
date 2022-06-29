using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class btn_manage : MonoBehaviour
{
    public void start_game()
    {
        SceneManager.LoadScene("Loading");
        Time.timeScale = 1;//인게임신에서 옵션창으로 넘어왔을때 시간이 멈춘것을 다시 흐르게 해준다.
    }

    public void go_game()
    {
        SceneManager.LoadScene("StartScene");
    }
    public void quit_game()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }
}
