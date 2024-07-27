using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum DropItems
{
    Meso,Item
}
public class MonsterDrop : MonoBehaviour
{
    InGameUI inGameUI;

    [SerializeField] DropItems dropType;
    [SerializeField] TextMeshProUGUI getMesoText;

    int getMeso;
    public int monsterMeso;

    private void Awake()
    {
        inGameUI = GameObject.Find("UIManager").GetComponent<InGameUI>();
    }
    private void OnEnable()
    {
       
    }
    void Update()
    {
        transform.Rotate(new Vector3(Time.deltaTime*45f,0 ,0));
    }
    void MesoSetting()
    {
        getMeso = monsterMeso * Random.Range(80, 120) / 100;
        //getMesoText.transform.position = this.gameObject.transform.position + new Vector3(0, 1f, 0);
        //getMesoText.text = $"{getMeso}";
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if(dropType == DropItems.Meso)
            {
                MesoSetting();
                SoundManager._sound.PlaySfx(2);
                GameManager.Instance.PlayerMeso += getMeso;
                inGameUI.ShowGetText("Meso", getMeso);
            }
            gameObject.SetActive(false);
        }
    }
}
