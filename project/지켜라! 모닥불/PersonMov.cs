using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonMov : MonoBehaviour
{
    private GameObject Person; // 불 막아주는 사람
    public float speed = 4.0f; // 사람 속도
    public float End_Position_x = 4.0f;//끝 위치
    private float Position_y;//캐릭터의 y축 위치
    public bool left, right; // 왼쪽, 오른쪽 이동 버튼

    // Start is called before the first frame update
    void Start()
    {
        Person = GameObject.Find("Person");
        Position_y = Person.transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (left && !right)
        {
            Person.GetComponent<Animator>().speed = 1.0f;
            Person.GetComponent<Transform>().position += Vector3.left * speed * Time.deltaTime;
            Person.transform.localScale = new Vector3(-0.2f, 0.2f, 0.2f);
        } 
        
        if (right && !left)
        {
            Person.GetComponent<Animator>().speed = 1.0f;
            Person.GetComponent<Transform>().position += Vector3.right * speed * Time.deltaTime;
            Person.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        if(!left && !right)
        {
            Person.GetComponent<Animator>().speed = 0.0f;
        }
        //범위초과
        if (Person.transform.position.x > End_Position_x)
        {
            Person.transform.position = new Vector3(-End_Position_x,Position_y,0);
        }
        else if(Person.transform.position.x < -End_Position_x)
        {
            Person.transform.position = new Vector3(End_Position_x, Position_y, 0);
        }
    }
    public void goleft() //왼쪽 이동
    {
        left = true;
    }
    public void stopleft() //왼쪽 이동 멈춤
    {
        left = false;
    }
    public void goright() //오른쪽 이동
    {
        
        right = true;
    }
    public void stopright() //오른쪽 이동 멈춤
    {
        right = false;
    }
}
