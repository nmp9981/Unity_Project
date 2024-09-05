using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog
{
    //virtual을 부모에
    virtual public void Bark()
    {
        Debug.Log("멍멍");
    }
}
public class FireDog : Dog
{
    //상속받은게 오버라이딩 대상
    public override void Bark()
    {
        Debug.Log("완완");
    }
}
public class Program : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        FireDog pd = new FireDog();
        pd.Bark();

        Dog dog = new Dog();
        dog.Bark();

        dog = new FireDog();
        dog.Bark();
    }
}
