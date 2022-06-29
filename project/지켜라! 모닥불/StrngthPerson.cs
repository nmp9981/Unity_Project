using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StrngthPerson : MonoBehaviour
{
    public Sprite um2;
    SpriteRenderer thisimg;

    PersonMov personmov;
    UmbrellaMgr umbrellamgr;
    Strength strength;
    
    Image preimg;
    public Sprite speed_up;//속도 증가
    public Sprite umbrella_size_up;//우산 크기 증가

    public Text t;
    public int lv = 1;//강화 레벨
    private int num;//랜덤 번호
    Vector3 scale = new Vector3(0.1f, 0.1f, 0f);

    // Start is called before the first frame update
    void Start()
    {
        preimg = GetComponent<Image>();

        personmov = GameObject.FindWithTag("Player").GetComponent<PersonMov>();
        umbrellamgr = GameObject.FindWithTag("Umbrella").GetComponent<UmbrellaMgr>();
        strength = GameObject.FindWithTag("enchant").GetComponent<Strength>();
        num = Random.Range(1, 3);
        thisimg = umbrellamgr.gameObject.GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        if (num == 1)
        {
            if(!this.gameObject.GetComponent<Button>().interactable)
                this.gameObject.GetComponent<Button>().interactable = true;
            preimg.sprite = speed_up;
        }
        else if(num==2)
        {
            if (lv == 1)
            {
                if (!this.gameObject.GetComponent<Button>().interactable)//버튼 활성화 여부
                    this.gameObject.GetComponent<Button>().interactable = true;
                preimg.sprite = umbrella_size_up;
            }
            else
            {
                preimg.sprite = umbrella_size_up;
                this.gameObject.GetComponent<Button>().interactable = false;//버튼 비활성화
                ColorBlock colorblock= this.gameObject.GetComponent<Button>().colors; 
                colorblock.disabledColor = new Color(200.0f /255.0f, 200.0f/ 255.0f, 200.0f /255.0f, 150.0f/ 255.0f);

                this.gameObject.GetComponent<Button>().colors = colorblock;

                //t.GetComponent<Text>().text = "우산 최대 레벨";
            }
        }
    }
    //마우스 클릭
    public void onclick()
    {
        if (num == 1)
        {
            PersonSpeedIncrease();
            num = Random.Range(1, 3);
            strength.enchant.SetActive(false);
        }
        else if (num == 2)
        {
            lv++;
            if (lv <= 2)//최대 강화 레벨
            {
                UmbrellaScaleIncrease();
            }
            num = Random.Range(1, 3);
            strength.enchant.SetActive(false);
        }
    }
    //인간 속도 증가
    public void PersonSpeedIncrease()
    {
        personmov.speed += 0.5f;
    }
    //우산 크기 증가
    public void UmbrellaScaleIncrease()
    {
        umbrellamgr.gameObject.transform.localScale = new Vector3(0.25f, 0.2f, 0.0f);
        thisimg.sprite = um2;
    }
}
