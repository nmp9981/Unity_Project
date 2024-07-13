using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowPartner : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerShadow;
    [SerializeField] GameObject shadowPartnerImage;
    [SerializeField] TextMeshProUGUI shadowPartnerTimeText;

    float shadowPartnerFullTime;
    float shadowPartnerTime;
    private void Awake()
    {
        shadowPartnerImage.SetActive(false);
        shadowPartnerTime = 0f;
    }
    void Update()
    {
        ShadowPartnerSkill();
        ShowBuffTime();
    }
    void ShadowPartnerSkill()
    {
        shadowPartnerFullTime = GameManager.Instance.ShadowTime;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerShadow.SetActive(true);
            shadowPartnerImage.SetActive(true);
            SoundManager._sound.PlaySfx(7);
            shadowPartnerTime = shadowPartnerFullTime;
            GameManager.Instance.PlayerMP -= 50;
            foreach (MeshRenderer mesh in playerShadow.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material.color = Color.black;
            }
        }
    }
    void ShowBuffTime()
    {
        if (shadowPartnerTime < 1)//원래대로
        {
            shadowPartnerTimeText.text = "";
            shadowPartnerImage.SetActive(false);
            playerShadow.SetActive(false);
            return;
        }
        shadowPartnerTime -= Time.deltaTime;
        shadowPartnerTimeText.text = string.Format("{0:N0}", shadowPartnerTime);
    }
}
