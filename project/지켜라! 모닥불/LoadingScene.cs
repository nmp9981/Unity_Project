using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    Image loadingBar;

    private void Start()
    {
        StartCoroutine(LoadAsyncScene());
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene("Loading");//바로 불러옴
    }

    IEnumerator LoadAsyncScene()
    {
        yield return null;
        AsyncOperation op = SceneManager.LoadSceneAsync("InGameScene");//완료 후 넘어감(텀을 로딩씬으로 채워줌)
        op.allowSceneActivation = false;//넘어가도되는가
        float timer = 0.0f;
        loadingBar.fillAmount = 0;
        while (!op.isDone)//완료가 될때까지
        {
            yield return null;
            timer += Time.deltaTime;
            if (op.progress < 0.9f)//진행도
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, op.progress, timer);
                if (loadingBar.fillAmount >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                loadingBar.fillAmount = Mathf.Lerp(loadingBar.fillAmount, 1f, timer);
                if (loadingBar.fillAmount == 1.0f)
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }
    }
}
