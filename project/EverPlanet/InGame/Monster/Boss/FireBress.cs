using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBress : MonoBehaviour
{
    int timeCount;
    [SerializeField] GameObject[] bressPrice = new GameObject[8];

    int[] bressDirX = {1,1,1,-1,-1,-1,0,0};
    int[] bressDirZ = {0,1,-1,0,1,-1,1,-1 };
    private void OnEnable()
    {
        timeCount = 0;
        SettingBress();
        InvokeRepeating("Bress", 0.5f, 0.08f);
    }
    void SettingBress()
    {
        for(int idx = 0; idx < 8; idx++)
        {
            int rotateAngle = 45 * idx;
            bressPrice[idx].SetActive(true);
            bressPrice[idx].transform.rotation = Quaternion.Euler(0, rotateAngle, 0);
        }
    }
    void Bress()
    {
        if(timeCount < 50)
        {
            for (int idx = 0; idx < 8; idx++)
            {
                bressPrice[idx].transform.position += new Vector3(bressDirX[idx], 0, bressDirZ[idx]);

            }
            timeCount += 1;
        }
        else
        {
            for (int idx = 0; idx < 8; idx++)
            {
                bressPrice[idx].SetActive(false);
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && !GameManager.Instance.IsInvincibility)
        {
            int damage = UnityEngine.Random.Range(4700, 5400);
            GameManager.Instance.PlayerHP -= damage;
            if (GameManager.Instance.PlayerHP <= 0) GameManager.Instance.PlayerHP = 0;
            StartCoroutine(collision.gameObject.GetComponent<PlayerHit>().ShowDamage(damage));
        }
    }
}
