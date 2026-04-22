using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TabGroup tabGroup;
    private RectTransform rectTransform;
    private Vector3 originalPosition;
    private bool positionInitialized = false;
    
    void Start()
    {
        if (tabGroup == null)
            tabGroup = GetComponentInParent<TabGroup>();
        tabGroup.Subscribe(this);
        rectTransform = GetComponent<RectTransform>();
        
        // Chờ LayoutGroup hoàn thành trong frame tiếp theo
        StartCoroutine(InitializePositionAfterLayout());
    }
    
    private System.Collections.IEnumerator InitializePositionAfterLayout()
    {
        // Chờ 2 frame để LayoutGroup hoàn toàn hoàn thành tính toán
        yield return null;
        yield return null;
        
        originalPosition = rectTransform.localPosition;
        positionInitialized = true;
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        tabGroup.OnTabEnter(this);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        tabGroup.OnTabExit(this);
    }
    
    /// <summary>
    /// Di chuyển button sang trái với smooth
    /// </summary>
    public void MoveLeft(float distance, float duration)
    {
        StopAllCoroutines();
        Vector3 targetPosition = originalPosition + Vector3.left * distance;
        StartCoroutine(SmoothMove(targetPosition, duration));
    }
    
    /// <summary>
    /// Trả button về vị trí cũ với smooth
    /// </summary>
    public void MoveToOriginal(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(SmoothMove(originalPosition, duration));
    }
    
    /// <summary>
    /// Coroutine di chuyển mượt
    /// </summary>
    private System.Collections.IEnumerator SmoothMove(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = rectTransform.localPosition;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            
            // Dùng Lerp cho smooth movement
            rectTransform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            
            yield return null;
        }
        
        // Đảm bảo vị trí cuối cùng chính xác
        rectTransform.localPosition = targetPosition;
    }
}
