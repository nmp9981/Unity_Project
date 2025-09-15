using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DropdownImageHandler : MonoBehaviour
{
    public TMP_Dropdown _dropdown;
    public Image _iconImage;
    private List<(Sprite,Color)> spriteList = new();

    private void Start()
    {
        foreach(var sprite in _dropdown.options)
        {
            Sprite newSP = sprite.image;
            spriteList.Add((sprite.image, sprite.color));
        }


        if(_dropdown != null)
        {
            _dropdown.onValueChanged.AddListener(delegate { DropdownValueImage(_dropdown); });
            //시작시 첫번째 이미지로 설정
            if (spriteList.Count > 0)
            {
                _iconImage.sprite = spriteList[0].Item1;
                _iconImage.color = spriteList[0].Item2;
            }

            // 드롭다운이 열릴 때 이벤트를 감지하여 색상을 설정합니다.
            SetItemColors();
            //_dropdown.onenable.AddListener(SetItemColors);
        }
    }

    private void DropdownValueImage(TMP_Dropdown change)
    {
        if (change.value < spriteList.Count)
        {
            _iconImage.sprite = spriteList[change.value].Item1;
            _iconImage.color = spriteList[change.value].Item2;
        }
    }

    private void SetItemColors()
    {
        // 드롭다운 템플릿의 Content를 찾습니다.
        Transform content = _dropdown.transform.Find("Template/Viewport/Content");

        if (content == null)
        {
            Debug.LogError("드롭다운 템플릿의 Content를 찾을 수 없습니다. 템플릿 구조를 확인해주세요.");
            return;
        }

        // 모든 드롭다운 항목을 순회하며 색상 설정
        for (int i = 0; i < content.childCount; i++)
        {
            // 각 항목(Item) 오브젝트를 가져옵니다.
            Transform item = content.GetChild(i);

            // 항목의 텍스트 컴포넌트를 찾습니다.
            TMP_Text itemLabel = item.Find("Item Label")?.GetComponent<TMP_Text>();

            if (itemLabel != null && i < _dropdown.options.Count)
            {
                // 옵션 데이터에서 색상 정보를 가져와 텍스트 컴포넌트에 할당합니다.
                Debug.Log(itemLabel.name);
                itemLabel.color = _dropdown.options[i].color; // 이전에 이미지에 색상을 설정했을 경우
            }
        }
    }
}
