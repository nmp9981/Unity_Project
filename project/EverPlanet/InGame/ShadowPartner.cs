using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class ShadowPartner : MonoBehaviour
{
    [SerializeField] GameObject player;
    [SerializeField] GameObject playerShadow;
    void Update()
    {
        ShadowPartnerSkill();
    }
    void ShadowPartnerSkill()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            playerShadow.SetActive(true);
            
            foreach (MeshRenderer mesh in playerShadow.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material.color = Color.black;
            }
            
        }
    }
}
