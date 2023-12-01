using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUInstancingTest_ : MonoBehaviour
{
    public Transform prefab;
    public int instances = 5000;
    public float radius = 50f;

    private MaterialPropertyBlock properties;
    private Transform t;

    private List<Renderer> rendlist;

    private void Start()
    {
        rendlist = new List<Renderer>(); //render list생성 
        properties = new MaterialPropertyBlock(); //property 블록 생성 

        for (int i = 0; i < instances; i++)
        {
            //prefab 생성 
            t = Instantiate(prefab);
            //render 리스트 추가 
            Renderer rend = t.GetComponent<Renderer>();
            rendlist.Add(rend);

            t.localPosition = Random.insideUnitSphere * radius; //random 위치 
            t.SetParent(transform); //생성된 오브젝트 하위로 
        }
    }

    private void Update()
    {
        for (int i = 0; i < instances; i++)
        {
            //색상 지정하기 
            properties.SetColor(
               "_Color", new Color(Random.value, Random.value, Random.value)
           );
            //색상 적용하기 
            rendlist[i].SetPropertyBlock(properties);
        }
    }
}
