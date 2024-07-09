using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SkillUIManager : MonoBehaviour
{
    [SerializeField] GameObject skillUI;
    [SerializeField] TextMeshProUGUI skillPointText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K)) SkillUIOnOff();
        SkillPointShow();
    }
    void SkillUIOnOff()
    {
        if (skillUI.activeSelf) skillUI.SetActive(false);
        else skillUI.SetActive(true);
    }
    void SkillPointShow()
    {
        skillPointText.text = $"{GameManager.Instance.SkillPoint}";
    }
}
