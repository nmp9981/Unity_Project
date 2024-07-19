using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class RightClickEvent : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] GameObject itemOptionUI;
    public string itemName;
    public ItemKind kind;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (!itemOptionUI.activeSelf)
            {
                Vector3 itemUISize = new Vector3(GetComponent<RectTransform>().rect.width / 2, -GetComponent<RectTransform>().rect.height / 2, 0);
                itemOptionUI.SetActive(true);
                itemOptionUI.transform.position = gameObject.transform.position + itemUISize;
                GameManager.Instance.SelectedItemName = itemName;
                GameManager.Instance.ItemKinds = kind;
            }
            else
            {
                itemOptionUI.SetActive(false);
                GameManager.Instance.SelectedItemName = string.Empty;
            }
        }
    }
}
