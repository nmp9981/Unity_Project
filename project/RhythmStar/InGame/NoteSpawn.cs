using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteSpawn : MonoBehaviour
{
    ObjectFulling _objectPulling;

    void Awake()
    {
        _objectPulling = GameObject.Find("ObjectFulling").GetComponent<ObjectFulling>();
        StartCoroutine(NoteCreate());
    }

    IEnumerator NoteCreate()
    {
        for(int i = 0; i < 100; i++)
        {
            yield return new WaitForSeconds(0.3f);
            int noteNum = Random.Range(0, 3);
            GameObject noteObj = _objectPulling.MakeObj(noteNum);

            switch (noteNum)
            {
                case 0:
                    noteObj.transform.position = new Vector2(-1, 6);
                    break;
                case 1:
                    noteObj.transform.position = new Vector2(0, 6);
                    break;
                case 2:
                    noteObj.transform.position = new Vector2(1, 6);
                    break;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
