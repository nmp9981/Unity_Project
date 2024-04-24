using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NoteColor
{
    RED, GREEN, BLUE
}
public class NoteFuction : MonoBehaviour
{
    [SerializeField] public NoteColor NoteType; 
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        NoteMove();
        RemoveNote();
    }
    void RemoveNote()
    {
        if (gameObject.transform.position.y < -3.3f)
        {
            this.gameObject.SetActive(false);
            GameManager.Instance.ComboCount = 0;
        }
    }
    void NoteMove()
    {
        this.gameObject.transform.position += Vector3.down * GameManager.Instance.NoteSpeed * Time.deltaTime;
    }
}
