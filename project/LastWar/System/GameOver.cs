using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using TMPro;
using static LastWar.CharacterAuthoring;

namespace LastWar
{
    public class GameOver : MonoBehaviour
    {
        [SerializeField] private TMPro.TextMeshProUGUI resultLvText;
        [SerializeField] private TMPro.TextMeshProUGUI resultExpText;
        
        void Update()
        {
            float expRate = (float)UIManager.resultExp/UIManager.resultMaxExp*100f;
            resultLvText.text = string.Format("Lv : {0}", UIManager.resultLv);
            resultExpText.text = string.Format("Exp : {0} ({1:N2}%)", UIManager.resultExp, expRate);
        }
    }

}
