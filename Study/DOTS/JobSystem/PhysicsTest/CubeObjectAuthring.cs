using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.VisualScripting;
using UnityEngine;


namespace CollisionTest
{
    public class CubeObjectAuthring : MonoBehaviour
    {
        class Baker : Baker<CubeObjectAuthring>
        {
            public override void Bake(CubeObjectAuthring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new CubeObj());
            }
        }
       
        public struct CubeObj: IComponentData
        {
           
        }
        public void OnMouseDrag()
        {
            float distance = 10;
            Vector3 mousePosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance); 
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            Debug.Log(objPosition);
            transform.position = objPosition;
       
        }
    }

}
