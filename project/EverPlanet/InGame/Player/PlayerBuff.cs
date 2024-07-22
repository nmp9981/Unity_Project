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
    [SerializeField] GameObject accBuffImage;
    [SerializeField] TextMeshProUGUI accBuffTimeText;
    [SerializeField] GameObject avoidBuffImage;
    [SerializeField] TextMeshProUGUI avoidBuffTimeText;
    [SerializeField] GameObject boosterImage;
    [SerializeField] TextMeshProUGUI boosterTimeText;
    [SerializeField] GameObject mesoUpImage;
    [SerializeField] TextMeshProUGUI mesoUpTimeText;

    float hasteFullTime;
    float hasteTime;

    float attackbuffFullTime;
    float attackBuffTime;
    float accbuffFullTime;
    float accBuffTime;
    float avoidbuffFullTime;
    float avoidBuffTime;

    float boosterTime;
    float boosterFullTime;
    float mesoUpTime;
    float mesoUpFullTime;

    private void Awake()
    {
        hasteImage.SetActive(false);
        hasteTime = 0f;
       
        attackBuffTime = 0f;
        attackbuffFullTime = 300f;
        GameManager.Instance.AttackUPCount = 100;

        accBuffTime = 0f;
        accbuffFullTime = 300f;

        boosterImage.SetActive(false);
        boosterTime = 0f;

        mesoUpImage.SetActive(false);
        mesoUpTime = 0f;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && GameManager.Instance.AddJumpSpeed>0)
        {
            hasteFullTime = GameManager.Instance.HasteTime;
            StartCoroutine(Haste());
        }
        if (Input.GetKeyDown(KeyCode.W) && GameManager.Instance.AttackUPCount>=1)
        {
            StartCoroutine(AttackBuff());
        }
        if (Input.GetKeyDown(KeyCode.R) && GameManager.Instance.BoosterTime >= 10)
        {
            boosterFullTime = GameManager.Instance.BoosterTime;
            StartCoroutine(Booster());
        }
        if (Input.GetKeyDown(KeyCode.T) && GameManager.Instance.MesoUpTime >= 37)
        {
            mesoUpFullTime = GameManager.Instance.MesoUpTime;
            StartCoroutine(MesoUp());
        }
        if (Input.GetKeyDown(KeyCode.Y) && GameManager.Instance.AccUPCount >= 1)
        {
            StartCoroutine(ACCBuff());
        }
        if (Input.GetKeyDown(KeyCode.U) && GameManager.Instance.AvoidUPCount >= 1)
        {
            StartCoroutine(AvoidBuff());
        }
        ShowHasteBuffTime();
        ShowAttackBuffTime();
        ShowAccBuffTime();
        ShowAvoidBuffTime();
        ShowMesoUpBuffTime();
        ShowBoosterTime();

    }
    IEnumerator Haste()
    {
        hasteImage.SetActive(true);
        SoundManager._sound.PlaySfx(6);
        hasteTime = hasteFullTime;
        GameManager.Instance.PlayerMoveSpeed = 5*(float)(1+GameManager.Instance.AddMoveSpeed/100);
        GameManager.Instance.PlayerJumpSpeed = 7 * (float)(1+GameManager.Instance.AddJumpSpeed / 100);
        yield break;
    }
    IEnumerator AttackBuff()
    {
        attackBuffImage.SetActive(true);
        SoundManager._sound.PlaySfx(8);
        GameManager.Instance.AttackUPCount -= 1;
        attackBuffTime = attackbuffFullTime;
        GameManager.Instance.IsAtaackBuffOn = true;
        yield break;
    }
    IEnumerator ACCBuff()
    {
        accBuffImage.SetActive(true);
        SoundManager._sound.PlaySfx(8);
        GameManager.Instance.AccUPCount -= 1;
        accBuffTime = accbuffFullTime;
        GameManager.Instance.IsAccBuffOn = true;
        yield break;
    }
    IEnumerator AvoidBuff()
    {
        avoidBuffImage.SetActive(true);
        SoundManager._sound.PlaySfx(8);
        GameManager.Instance.AvoidUPCount -= 1;
        avoidBuffTime = avoidbuffFullTime;
        GameManager.Instance.IsAvoidBuffOn = true;
        yield break;
    }
    IEnumerator MesoUp()
    {
        mesoUpImage.SetActive(true);
        SoundManager._sound.PlaySfx(6);
        mesoUpTime = mesoUpFullTime;
        yield break;
    }
    IEnumerator Booster()
    {
        boosterImage.SetActive(true);
        SoundManager._sound.PlaySfx(6);
        boosterTime = boosterFullTime;
        GameManager.Instance.PlayerAttackSpeed = 0.38f;
        yield break;
    }
    void ShowHasteBuffTime()
    {
        if (hasteTime < 1)//원래대로
        {
            hasteTimeText.text = "";
            GameManager.Instance.PlayerMoveSpeed = 5f;
            GameManager.Instance.PlayerJumpSpeed = 7f;
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
    void ShowAccBuffTime()
    {
        if (accBuffTime < 1)//원래대로
        {
            accBuffTimeText.text = "";
            GameManager.Instance.IsAccBuffOn = false;
            accBuffImage.SetActive(false);
            return;
        }
        accBuffTime -= Time.deltaTime;
        accBuffTimeText.text = string.Format("{0:N0}", accBuffTime);
    }
    void ShowAvoidBuffTime()
    {
        if (avoidBuffTime < 1)//원래대로
        {
            avoidBuffTimeText.text = "";
            GameManager.Instance.IsAvoidBuffOn = false;
            avoidBuffImage.SetActive(false);
            return;
        }
        avoidBuffTime -= Time.deltaTime;
        avoidBuffTimeText.text = string.Format("{0:N0}", avoidBuffTime);
    }
    void ShowMesoUpBuffTime()
    {
        if (mesoUpTime < 1)//원래대로
        {
            mesoUpTimeText.text = "";
            mesoUpImage.SetActive(false);
            return;
        }
        mesoUpTime -= Time.deltaTime;
        mesoUpTimeText.text = string.Format("{0:N0}", mesoUpTime);
    }
    void ShowBoosterTime()
    {
        if (boosterTime < 1)//원래대로
        {
            boosterTimeText.text = "";
            GameManager.Instance.PlayerAttackSpeed = 0.5f;
            boosterImage.SetActive(false);
            return;
        }
        boosterTime -= Time.deltaTime;
        boosterTimeText.text = string.Format("{0:N0}", boosterTime);
    }
}
