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
    [SerializeField] DropItems dropType;
    [SerializeField] TextMeshProUGUI getMesoText;

    int getMeso;
    public int monsterMeso;
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
                GameManager.Instance.PlayerMeso += getMeso;
            }
            gameObject.SetActive(false);
        }
    }
}
