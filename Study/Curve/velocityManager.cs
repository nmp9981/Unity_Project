using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using Unity.VisualScripting;

public class VelocityManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI velocityText;
    [SerializeField] float kartSpeed;
    [SerializeField] GameObject kart;
    Vector3 kartPos;
    float colTime = 0.11f;
    float timeCheck;

    // Start is called before the first frame update
    void Awake()
    {
        kart = GameObject.Find("Kart");
        velocityText.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(750, -350, 0);
        velocityText.text = $"<size=50>{0}km/h</size>";
        kartPos = kart.transform.position;
        timeCheck = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        KartSpeed();
        velocityText.text = $"<size=50>{Mathf.Abs(kartSpeed)}km/h</size>";
    }
    void KartSpeed()
    {
        if(colTime < timeCheck)
        {
            float distX = kart.transform.position.x - kartPos.x;
            float distY = kart.transform.position.y - kartPos.y;
            float distZ = kart.transform.position.z - kartPos.z;
            
            float dist = Mathf.Sqrt(distX * distX + distY * distY + distZ * distZ);
            kartSpeed = Mathf.Round( 22*dist/colTime);
            timeCheck = 0.0f;
            kartPos = kart.transform.position;
        }
        timeCheck += Time.deltaTime;
    }
}
