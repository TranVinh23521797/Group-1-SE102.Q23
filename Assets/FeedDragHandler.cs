using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(CanvasGroup))]
public class FeedDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("Feed Configuration")]
    public string feedID = "cow_feed"; // Must match the Cattle's required feed
    
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save the starting position so we can snap back if the drop fails
        originalPosition = rectTransform.anchoredPosition;

        // Make the UI semi-transparent while dragging (optional but looks good)
        canvasGroup.alpha = 0.6f;

        // CRITICAL: Stop this UI element from blocking raycasts so we can click through it to the 3D world
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Move the UI image with the pointer (mouse or touch)
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Cattle targetCattle = hit.collider.GetComponent<Cattle>();

            if (targetCattle != null && targetCattle.currentState == AnimalState.Hungry)
            {
                if (targetCattle.feedItemID == feedID)
                {
                    // --- THE NEW LINK ---
                    // Check if we have at least 1 of this feed in the inventory
                    if (InventoryManager.Instance.HasItem(feedID, 1))
                    {
                        // Deduct the feed from inventory
                        InventoryManager.Instance.RemoveItem(feedID, 1);
                        
                        // Feed the animal
                        targetCattle.FeedAnimal();
                        
                        // Close the menu
                        transform.parent.gameObject.SetActive(false); 
                    }
                    else
                    {
                        Debug.LogWarning("You don't have enough feed in your inventory!");
                        SnapBack();
                    }
                }
                else
                {
                    Debug.LogWarning("Wrong feed type!");
                    SnapBack();
                }
            }
            else
            {
                SnapBack();
            }
        }
        else
        {
            SnapBack();
        }
    }

    private void SnapBack()
    {
        // Return the feed icon to its original slot in the UI menu
        rectTransform.anchoredPosition = originalPosition;
    }
}