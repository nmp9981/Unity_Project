using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBuff : MonoBehaviour
{
    [SerializeField] GameObject hasteImage;
    [SerializeField] TextMeshProUGUI hasteTimeText;
    [SerializeField] GameObject attackBuffImage;
    [SerializeField] TextMeshProUGUI attackBuffTimeText;

    float hasteFullTime;
    float hasteTime;
    float attackbuffFullTime;
    float attackBuffTime;

    private void Awake()
    {
        hasteImage.SetActive(false);
        hasteTime = 0f;
        hasteFullTime = 180f;

        attackBuffTime = 0f;
        attackbuffFullTime = 300f;
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(Haste());
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            StartCoroutine(AttackBuff());
        }
        ShowHasteBuffTime();
        ShowAttackBuffTime();
    }
    IEnumerator Haste()
    {
        hasteImage.SetActive(true);
        hasteTime = hasteFullTime;
        GameManager.Instance.PlayerMoveSpeed = 6f;
        GameManager.Instance.PlayerJumpSpeed = 8f;
        yield break;
    }
    IEnumerator AttackBuff()
    {
        attackBuffImage.SetActive(true);
        attackBuffTime = attackbuffFullTime;
        GameManager.Instance.IsAtaackBuffOn = true;
        yield break;
    }
    void ShowHasteBuffTime()
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
    void ShowAttackBuffTime()
    {
        if (attackBuffTime < 1)//원래대로
        {
            attackBuffTimeText.text = "";
            GameManager.Instance.IsAtaackBuffOn = false;
            attackBuffImage.SetActive(false);
            return;
        }
        attackBuffTime -= Time.deltaTime;
        attackBuffTimeText.text = string.Format("{0:N0}", attackBuffTime);
    }
}
