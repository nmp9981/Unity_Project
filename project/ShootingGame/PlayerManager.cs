using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    GamaManager gamaManager;
    ItemManager itemManager;
    
    [SerializeField] GameObject HpSlider;
    [SerializeField] GameObject HPBar;
    [SerializeField] Text HPInfo;
    
    // Start is called before the first frame update
    void Awake()
    {
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
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
        //적 or 적 총알에 맞음
        if(collision.tag == "EnemyBullet" || collision.tag == "Enemy")
        {
            if (collision.tag == "Enemy") GamaManager.Instance.HP -= 1;//적 몸체에 맞으면 추가 데미지

            GamaManager.Instance.HP -= 1;
            this.gameObject.SetActive(false);//피격하면 잠시 무적
            Invoke("ChangeImage", 0.3f);
        }
        //득템
        if(collision.tag == "Item")
        {
            itemManager.GetItem(collision.gameObject.name);
            collision.gameObject.SetActive(false);//충돌한 오브젝트 비활성화
        }
        if(GamaManager.Instance.HP <= 0)
        {
            GamaManager.Instance.GameOver();
        }
    }
    void ChangeImage()
    {
        this.gameObject.SetActive(true);//원래대로
    }
}
