using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class FireBress : MonoBehaviour
{
    int timeCount;
    RhinoSkill rhinoSkill;
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
        //gameObject.transform.position = GameObject.Find("Rhino_PBR(Clone)").transform.position;
        for(int idx = 0; idx < 8; idx++)
        {
            int rotateAngle = 45 * idx;
            bressPrice[idx].SetActive(true);
            bressPrice[idx].transform.rotation = Quaternion.Euler(0, rotateAngle, 0);
            bressPrice[idx].transform.position = gameObject.transform.position;
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
}
