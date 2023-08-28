using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    GamaManager gamaManager;
    [SerializeField] GameObject HpSlider;
    [SerializeField] GameObject HPBar;
    [SerializeField] Text HPInfo;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HpSlider.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1.0f, 0));
        HPInfo.transform.position = Camera.main.WorldToScreenPoint(this.gameObject.transform.position + new Vector3(0, 1.0f, 0));
        HPInfo.text = GamaManager.Instance.HP.ToString()+" / " + GamaManager.Instance.FullHP.ToString();
        HPBar.GetComponent<Image>().fillAmount = (float)GamaManager.Instance.HP / (float)GamaManager.Instance.FullHP;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //적 총알에 맞음
        if(collision.tag == "EnemyBullet")
        {
            GamaManager.Instance.HP -= 1;
        }

        if(GamaManager.Instance.HP <= 0)
        {
            GamaManager.Instance.GameOver();
        }
    }
}
