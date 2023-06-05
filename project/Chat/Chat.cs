using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Chat : MonoBehaviour
{
    public static Chat instance;
    private void Awake() => instance = this;
    
    public InputField SendInput;
    public RectTransform ChatContent;
    public Text ChatText;
    public ScrollRect ChatScrollRect;

    public void ShowMessage(string data)
    {
        ChatText.text += ChatText.text == "" ? data : "\n" + data;

        Fit(ChatText.GetComponent<RectTransform>());
        Fit(ChatContent);//부모까지 Fit
        Invoke("ScrollDelay", 0.03f);
    }
    void Fit(RectTransform Rect) => LayoutRebuilder.ForceRebuildLayoutImmediate(Rect);

    //맨 아래로 항상 위치
    void ScrollDelay() => ChatScrollRect.verticalScrollbar.value = 0;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowMessage("hello world!" + Random.Range(0, 100));
        }
    }
}
