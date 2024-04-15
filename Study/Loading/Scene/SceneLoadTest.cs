using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadTest : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadingSceneController.LoadScene("Scene2");
        }   
    }
}
