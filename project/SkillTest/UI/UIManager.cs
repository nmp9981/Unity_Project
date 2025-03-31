using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI accumulateDamageText;

    public List<Sprite> damageImage = new List<Sprite>();
    public List<Sprite> criticalDamageImage = new List<Sprite>();
    //누적 데미지
    long accumulateDamage = 0;

    //데미지 보이기
    public void ShowDamage(long damage)
    {
        accumulateDamage += damage;
        accumulateDamageText.text = accumulateDamage.ToString();
    }
}
