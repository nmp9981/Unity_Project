using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Move : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField]

    Slime_UI SU;
    public void OnPointerDown(PointerEventData ped)
    {
        SU.left_move();
    }
    public void OnPointerUp(PointerEventData ped)
    {
        SU.dont_move();
    }
}
