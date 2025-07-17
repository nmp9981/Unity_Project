using UnityEngine;
using UnityEngine.EventSystems;

public class ReorderableListItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Transform originalParent;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 1. 드래그를 시작할 때, 현재 부모(Vertical Layout Group)를 저장
        originalParent = transform.parent;

        // 2. 드래그하는 아이템을 Vertical Layout Group의 영향권 밖으로 이동
        // Canvas의 최상위 부모로 변경하여 자유롭게 움직일 수 있게 함
        transform.SetParent(transform.root);
        transform.SetAsLastSibling(); // 다른 UI보다 위에 표시되도록 순서 변경

        // 3. 드래그 중에는 레이캐스트를 무시하여 아래에 있는 다른 아이템들을 감지할 수 있게 함
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0.6f; // 반투명하게 만들어 드래그 효과 주기
            canvasGroup.blocksRaycasts = false;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 드래그하는 동안 마우스 위치에 따라 아이템의 위치를 업데이트
        rectTransform.anchoredPosition += eventData.delta / transform.root.GetComponent<Canvas>().scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 1. 드래그가 끝나면 원래 부모(Vertical Layout Group)로 다시 돌아옴
        transform.SetParent(originalParent);

        // 2. 새로운 순서를 계산
        // 드래그된 아이템의 현재 y 위치를 기준으로, 리스트에 있는 다른 아이템들과 비교
        int newSiblingIndex = 0;
        float currentYPosition = rectTransform.position.y;

        for (int i = 0; i < originalParent.childCount; i++)
        {
            // 드래그된 아이템 자신은 건너뛰기
            if (originalParent.GetChild(i) == transform)
            {
                continue;
            }

            // 다른 아이템이 드래그된 아이템보다 아래에 있으면 (Y 좌표가 더 작으면)
            if (originalParent.GetChild(i).position.y < currentYPosition)
            {
                newSiblingIndex++;
            }
        }
        
        // 3. 계산된 순서대로 Hierarchy의 위치를 변경
        transform.SetSiblingIndex(newSiblingIndex);

        // 4. 원래 상태로 복구
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }
}
