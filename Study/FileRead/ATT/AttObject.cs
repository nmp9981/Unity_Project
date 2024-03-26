using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttObject : MonoBehaviour
{
    private string objectName;
    private List<Attribute> attributeList;

    public AttObject()
    {
        this.objectName = "";
        this.attributeList = new List<Attribute>();
    }
    public AttObject(string objectName)
    {
        this.objectName = objectName;
        this.attributeList = new List<Attribute>();
    }
    public string getObjectName() { return objectName; }
    public List<Attribute> getAttributeList() { return attributeList; }
    public void setObjectName(string objectName) { this.objectName = objectName; }
    public void setAttributeList(List<Attribute> attributeList)
    {
        this.attributeList = attributeList;
    }
}
