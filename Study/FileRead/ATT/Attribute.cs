using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEditor.Build.Pipeline;
using UnityEngine;

public class Attribute : MonoBehaviour
{
    private string key;
    private string value;

    public Attribute()
    {
        this.key = "";
        this.value = "";
    }
    public Attribute(string key,string value)
    {
        this.key = key;
        this.value = value;
    }
    public string getKey()
    {
        return key;
    }
    public void setKey(string key)
    {
        this.key = key;
    }
}
