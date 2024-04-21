using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteFuction : MonoBehaviour
{
    // Start is called before the first frame update
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
        if (gameObject.transform.position.y < -4) this.gameObject.SetActive(false);
    }
    void NoteMove()
    {
        this.gameObject.transform.position += Vector3.down * GameManager.Instance.NoteSpeed * Time.deltaTime;
    }
}
