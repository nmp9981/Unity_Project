using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Program : MonoBehaviour
{
    delegate void CalcDelegate(int x, int y);

    static void Callback(int x,int y, CalcDelegate dele)
    {
        dele(x, y);
    }
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

        CalcDelegate plus = Plus;
        CalcDelegate minus = Minus;
        CalcDelegate mul = multi;
        CalcDelegate div = Div;

        Callback(90, 80, mul);
        Callback(90, 90, plus);
    }
}
