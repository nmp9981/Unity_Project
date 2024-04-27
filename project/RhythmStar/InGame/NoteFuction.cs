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

    NoteJudge noteJudge;
    void Awake()
    {
        noteJudge = GameObject.Find("JudgeLine").GetComponent<NoteJudge>();
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
            Debug.Log(NoteType);
            switch (NoteType)
            {
                case NoteColor.RED:
                    StartCoroutine(JudgeMissTextOn(0));
                    break;
                case NoteColor.GREEN:
                    StartCoroutine(JudgeMissTextOn(1));
                    break;
                case NoteColor.BLUE:
                    StartCoroutine(JudgeMissTextOn(2));
                    break;
            }
            this.gameObject.SetActive(false);
            GameManager.Instance.ComboCount = 0;
            GameManager.Instance.HealthPoint -= 4.0f;
        }
    }
    void NoteMove()
    {
        this.gameObject.transform.position += Vector3.down * GameManager.Instance.NoteSpeed * Time.deltaTime;
    }
    public IEnumerator JudgeMissTextOn(int num)
    {
        noteJudge.judgeText[num].text = "Miss";
        noteJudge.judgeText[num].color = Color.red;
        yield return new WaitForSeconds(0.3f);
        noteJudge.judgeText[num].text = "";
    }
}