using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InGameUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _restCountFishText;
    void Start()
    {
        GameManager.Instance.RestCount = GameManager.Instance.StageNum * 10;
    }

    // Update is called once per frame
    void Update()
    {
        RestFishCountShow();
    }
    void RestFishCountShow()
    {
        if (GameManager.Instance.PlayMode != 0)//스테이지 모드일때만
        {
            _restCountFishText.text = "Rest Count : " + GameManager.Instance.RestCount.ToString();
        }
        else
        {
            _restCountFishText.text = "";
        }
    }
}
