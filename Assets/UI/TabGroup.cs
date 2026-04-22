using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    
    [SerializeField] private float moveDistance = 20f;  // Khoảng cách di chuyển (pixel)
    [SerializeField] private float moveDuration = 0.3f; // Thời gian di chuyển (giây)
    
    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
            tabButtons = new List<TabButton>();
        tabButtons.Add(button);
    }
    
    public void OnTabEnter(TabButton button)
    {
        // Di chuyển button sang trái
        button.MoveLeft(moveDistance, moveDuration);
    }

    public void OnTabExit(TabButton button)
    {
        // Trả button về vị trí cũ
        button.MoveToOriginal(moveDuration);
    }

    public void ResetTabs()
    {
        foreach (TabButton button in tabButtons)
        {
            button.MoveToOriginal(moveDuration);
        }
    }
}
