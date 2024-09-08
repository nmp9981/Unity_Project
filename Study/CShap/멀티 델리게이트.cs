using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    delegate void CalcDelegate(int x, int y);

    static void Plus(int x, int y)
    {
        Debug.Log(x + y);
    }
    static void Minus(int x, int y)
    {
        Debug.Log(x - y);
    }
    static void multi(int x, int y)
    {
        Debug.Log(x*y);
    }
    static void Div(int x, int y)
    {
        Debug.Log(x / y);
    }
    void Start()
    {

        CalcDelegate del1 = Plus;
        CalcDelegate del2 = Minus;
        CalcDelegate del3 = multi;
        CalcDelegate del4 = Div;

        del1 += del2;
        del1 += del3;
        del1 += del4;

        del1(37, 24);

        del1 -= del2;
        del1 -= del4;
        del1(37, 24);
    }
}
