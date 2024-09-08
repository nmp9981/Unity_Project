using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    delegate int CalcDelegate(int x, int y);

    static int Plus(int x, int y)
    {
        return x + y;
    }
    static int multi(int x, int y)
    {
        return x * y;
    }
    void Start()
    {

        CalcDelegate del1 = new CalcDelegate(Plus);
        int result = del1(40, 30);
        Debug.Log(result);

        CalcDelegate del2 = new CalcDelegate(multi);
        int result2 = del2(40, 30);
        Debug.Log(result2);
    }
}
