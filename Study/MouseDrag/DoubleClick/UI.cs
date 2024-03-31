using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DoubleClickButton : MonoBehaviour, IPointerClickHandler
{
    float clickTime = 0;
    float interval = 0.25f;

    public void OnPointerClick(PointerEventData eventData)
    {
        if ((Time.time - clickTime) < interval)
        {
            Debug.Log("Double Click!");
            clickTime = 0;
        }
        else
        {
            clickTime = Time.time;
        }
    }
}
