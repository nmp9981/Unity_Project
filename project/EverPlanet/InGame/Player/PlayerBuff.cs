using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBuff : MonoBehaviour
{
    [SerializeField] GameObject hasteImage;
    [SerializeField] TextMeshProUGUI hasteTimeText;

    float hasteFullTime;
    float hasteTime;
    private void Awake()
    {
        hasteImage.SetActive(false);
        hasteTime = 0f;
        hasteFullTime = 180f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Haste());
        }
        ShowBuffTime();
    }
    IEnumerator Haste()
    {
        hasteImage.SetActive(true);
        hasteTime = hasteFullTime;
        GameManager.Instance.PlayerMoveSpeed = 6f;
        GameManager.Instance.PlayerJumpSpeed = 8f;
        yield break;
    }
    void ShowBuffTime()
    {
        if (hasteTime < 1)//원래대로
        {
            hasteTimeText.text = "";
            GameManager.Instance.PlayerMoveSpeed = 4f;
            GameManager.Instance.PlayerJumpSpeed = 6f;
            hasteImage.SetActive(false);
            return;
        }
        hasteTime -= Time.deltaTime;
        hasteTimeText.text = string.Format("{0:N0}", hasteTime);
    }
}
