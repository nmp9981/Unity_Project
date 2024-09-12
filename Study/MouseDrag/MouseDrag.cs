using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragAndDrop : MonoBehaviour
{
    void OnMouseDrag()
    {
        float distance = Camera.main.WorldToScreenPoint(transform.position).z;

        Vector3 mousePos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, distance);
        Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);

        //objPos.x = 0;//특정 축 고정
        transform.position = objPos;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Object")
        {
            Debug.Log("충돌");
        }
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
