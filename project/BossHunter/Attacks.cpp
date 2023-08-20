using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Attacks : MonoBehaviour
{
    BossHit bossHit;
    Manager manager;
    SkillManager skillManager;

    private float delayTime = 0.5f;
    float speed = 20.0f;
    private void Awake()
    {
        bossHit = GameObject.FindWithTag("User").GetComponent<BossHit>();//BossHit 스크립트에서 변수 가져오기
        manager = GameObject.FindWithTag("Manager").GetComponent<Manager>();//Manager 스크립트에서 변수 가져오기
        skillManager = GameObject.Find("SkillManager").GetComponent<SkillManager>();
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(new Vector3(-speed*Time.deltaTime, 0, 0));
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss")
        {
            Destroy(this.gameObject);
        }
    }
    
}
