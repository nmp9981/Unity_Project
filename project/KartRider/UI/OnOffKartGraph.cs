using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 마우스 포인터를 올리면 카트 정보가 띄워짐
/// </summary>
public class KartManager : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerUpHandler
{
    [SerializeField]
    GameObject kartGraphObject;

    [SerializeField]
    int kartIndexNumber;

    public void OnPointerEnter(PointerEventData eventData)
    {
        kartGraphObject.SetActive(true);
        GameManager.Instance.MouseUpKartNum = kartIndexNumber;
        kartGraphObject.transform.position = eventData.position+new Vector2(200,0);
    }
    //마우스 커서 올라가 있는 동안
    public void OnPointerUp(PointerEventData eventData)
    {
        kartGraphObject.SetActive(true);
        kartGraphObject.transform.position = eventData.position + new Vector2(200, 0);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        kartGraphObject.SetActive(false);
    }
}
