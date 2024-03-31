using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickButton : MonoBehaviour
{
    float interval = 0.3f;
    float doubleClickedTime = 0.0f;
    bool isDoubleClicked = false;

    private void OnMouseUp()
    {
        if ((Time.time - doubleClickedTime) < interval)//더블 클릭 인정 시간
        {
            isDoubleClicked = true;
            doubleClickedTime = 0.0f;
        }
        else
        {
            isDoubleClicked = false;
            doubleClickedTime = Time.time;
        }
    }

    void Update()
    {
        if (isDoubleClicked)
        {
            Debug.Log("double click");
            isDoubleClicked = false;
        }
    }

}
