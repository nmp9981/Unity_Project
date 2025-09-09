using UnityEngine;
using UnityEngine.UI;

public class FixedHeaderScroll : MonoBehaviour
{
    // 스크롤뷰의 뷰포트 (Viewport)
    public RectTransform viewport;

    private RectTransform content;

    // 고정할 헤더 오브젝트 (Content의 첫 번째 자식)
    private RectTransform header;

    // 초기 헤더의 위치
    private Vector2 initialHeaderPosition;

    void Awake() // Awake 함수로 변경
    {
        // 스크립트가 붙어 있는 GameObject의 RectTransform을 가져옵니다.
        content = GetComponent<RectTransform>();
    }

    void Start()
    {
        if (transform.childCount > 0)
        {
            header = transform.GetChild(0).GetComponent<RectTransform>();
            initialHeaderPosition = header.anchoredPosition;
        }
    }

    void Update()
    {
        if (header != null && viewport != null)
        {
            // 현재 스크롤 위치를 계산
            float currentScrollPosition = content.anchoredPosition.y;

            // 헤더가 뷰포트 상단에 도달했는지 확인
            if (currentScrollPosition > initialHeaderPosition.y)
            {
                // 헤더를 뷰포트 상단에 고정
                // Content의 위치 변화를 상쇄시켜서 header를 제자리로 돌려놓는 계산
                header.anchoredPosition = new Vector2(initialHeaderPosition.x, initialHeaderPosition.y + currentScrollPosition);
            }
            else
            {
                // 스크롤 위치가 초기 위치보다 낮으면 원래 위치로 되돌림
                header.anchoredPosition = initialHeaderPosition;
            }
        }
    }
}
