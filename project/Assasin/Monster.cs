using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour
{
    public Text AttackDamage;//데미지
    public Text Sum_Dam;//누적 데미지

    public int AttackStat = 3000;//공격력
    public int Sum_Damage;//누적 데미지
    public int Damage;//데미지

    public int Max_HP = 300000;//몹 hp
    public int Current_HP;//현재 체력

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "ThrowWeapon")
        {
            Damage = Random.Range(AttackStat, AttackStat * 9 / 10);//데미지 입히기
            Sum_Damage += Damage;
            AttackDamage.text = "" + Damage;//데미지 텍스트 보이게
            Sum_Dam.text = "" + Sum_Damage;
            Current_HP -= Damage;//피격
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        Current_HP = Max_HP;
        Sum_Damage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
