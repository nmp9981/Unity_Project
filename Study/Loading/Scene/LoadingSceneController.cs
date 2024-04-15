using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController : MonoBehaviour
{
    static string nextScene;
    [SerializeField] Image progressBar;
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        SceneManager.LoadScene("LoadingScene");
    }

    private void Start()
    {
        StartCoroutine(LoadSceneProcess());
    }
    IEnumerator LoadSceneProcess()
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(nextScene);//비동기방식, 불러오는 중에도 다른 작업 가능
        op.allowSceneActivation = false;//씬을 90%까지만 로드한 채로 다음으로 넘어가지 않는다.(리소스 불러올 시간 번다.)
       
        float timer = 0f;
        while (!op.isDone)//씬이 모두 로드되지 않았다면
        {
            yield return null;

            if (op.progress < 0.9f)
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = op.progress;
            }
            else//fake 로딩
            {
                timer += Time.unscaledDeltaTime;
                progressBar.fillAmount = Mathf.Lerp(0.9f, 1.0f, timer);
                if (progressBar.fillAmount >= 1f)//다 채움
                {
                    op.allowSceneActivation = true;
                    yield break;
                }
            }
        }


    }
}
