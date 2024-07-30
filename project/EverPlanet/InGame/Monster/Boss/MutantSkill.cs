using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MutantSkill : MonoBehaviour
{
    GameObject player;
    GameObject mistObject;
    GameObject prickle;
    [SerializeField] GameObject whipLeft;
    [SerializeField] GameObject whipRight;
    
    private void Awake()
    {
        GetComponent<BearBossFunction>().enabled = true;
        mistObject = GameObject.Find("Mist");
        player = GameObject.Find("Player");
        prickle = GameObject.Find("Needle");
    }
    void OnEnable()
    {
        mistObject.SetActive(false);
        prickle.SetActive(false);   
        whipLeft.SetActive(false);
        whipRight.SetActive(false);
        InvokeRepeating("MutantAttack", 4f, 7f);
    }
    void MutantAttack()
    {
        float dist = (player.transform.position - gameObject.transform.position).sqrMagnitude;
        if (dist < 1500 && gameObject.activeSelf)
        {
            int ran = Random.Range(0, 9);
            switch (ran % 3)
            {
                case 0:
                    if (!mistObject.activeSelf) StartCoroutine(Mist());
                    break;
                case 1:
                    StartCoroutine(Prickle());
                    break;
                case 2:
                    StartCoroutine(Whip());
                    break;
            }
        }
    }
    IEnumerator Mist()
    {
        mistObject.SetActive(true);
        mistObject.transform.position = gameObject.transform.position;
        yield return new WaitForSeconds(10.0f);
        mistObject.SetActive(false);
    }
    IEnumerator Prickle()
    {
        prickle.SetActive(true);
        prickle.transform.position = player.transform.position + new Vector3(0, -5, 0);
        yield return new WaitForSeconds(3f);
        prickle.SetActive(false);
    }
    IEnumerator Whip()
    {
        whipLeft.transform.localScale = new Vector3(4,2,2);
        whipRight.transform.localScale = new Vector3(4,2,2);
        yield return new WaitForSeconds(0.7f);
        whipLeft.transform.localScale = new Vector3(1, 1, 1);
        whipRight.transform.localScale = new Vector3(1, 1, 1);
    }
}
