using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum NoteColor
{
    RED, GREEN, BLUE
}
public enum NoteType
{
    General,Long
}
public class NoteFuction : MonoBehaviour
{
    [SerializeField] public NoteColor NoteColor;
    public NoteType noteType;
    public int longNoteLength = 1;

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
        LongNoteChange();
    }
    void RemoveNote()
    {
        if (gameObject.transform.position.y < -3.3f)
        {
            switch (NoteColor)
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
            GameManager.Instance.MissCount += 1;
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
    void LongNoteChange()
    {
        if(gameObject.transform.position.y < -2)
        {
            if(noteType == NoteType.Long) LongNoteChange((int)NoteColor);
            else GerneralNoteChange((int)NoteColor);
        }
    }
    void LongNoteChange(int noteNum)
    {
        switch (noteNum)
        {
            case 0:
                GameManager.Instance.IsLongNote1 = true;
                break;
            case 1:
                GameManager.Instance.IsLongNote2 = true;
                break;
            case 2:
                GameManager.Instance.IsLongNote3 = true;
                break;
        }
    }
    void GerneralNoteChange(int noteNum)
    {
        switch (noteNum)
        {
            case 0:
                GameManager.Instance.IsLongNote1 = false;
                break;
            case 1:
                GameManager.Instance.IsLongNote2 = false;
                break;
            case 2:
                GameManager.Instance.IsLongNote3 = false;
                break;
        }
    }
}
