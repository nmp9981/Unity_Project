using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillManager : MonoBehaviour
{
    BossHit bossHit;
    Manager manager;
    Upgrage upgrade;
    AttackMgr attackMgr;
    public Image tripleSkill;
    public Image doubleSkill;
    public Image avengerSkill;

    public bool isTriple;//트리플스로우 사용여부
    public bool isDouble;//럭키세븐 사용여부
    public bool isAvenger;//어벤져 사용 여부

    [SerializeField] public GameObject Dagger;//표창 객체
    [SerializeField] public GameObject DoubleDagger;//표창 객체
    [SerializeField] public GameObject TripleDagger;//표창 객체
    [SerializeField] public GameObject AvengerDragger;//표창 갹체
   
    public Transform draggerPos;//위치 지정
                                
    void Awake()
    {
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
        bossHit = GameObject.FindWithTag("User").GetComponent<BossHit>();//BossHit 스크립트에서 변수 가져오기
        isDouble = false;
        isTriple = false;
    }
    public void GeneralAttack()
    {
        Instantiate(Dagger, draggerPos.position, draggerPos.rotation);//발사되는 위치 초기화
        
    }
    public void LuckySeven()
    {
        if (doubleSkill.gameObject.GetComponent<Image>().color.a == 1.0f)
        {
            isDouble = true;
            //Instantiate(Dagger, draggerPos.position + new Vector3(0.75f, 0.75f, 0), draggerPos.rotation);//발사되는 위치 초기화
            Instantiate(DoubleDagger, draggerPos.position, draggerPos.rotation);//발사되는 위치 초기화
        }
       
        isDouble = false;
    }
    public void TripleThrow()
    {
        if (tripleSkill.gameObject.GetComponent<Image>().color.a == 1.0f)
        {
            isTriple = true;
            Instantiate(TripleDagger, draggerPos.position+new Vector3(0.2f,0.2f,0), draggerPos.rotation);//발사되는 위치 초기화
        }
        isTriple = false;
    }
    public void AvengerThrow()
    {
        if (avengerSkill.gameObject.GetComponent<Image>().color.a == 1.0f)
        {
            isAvenger = true;
            Instantiate(AvengerDragger, draggerPos.position, draggerPos.rotation);//발사되는 위치 초기화
        }
        isAvenger = false;
    }
    IEnumerator SkillDelay()
    {
        yield return new WaitForSecondsRealtime(0.5f);
    }
}
