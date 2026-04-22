using UnityEngine;

public class TouchManager : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Drag your Feeding Menu Panel here from the Hierarchy")]
    public GameObject feedingMenuPanel;

    void Update()
    {
        // Listen for mobile touch or mouse click
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Fire the raycast into the 3D world
            if (Physics.Raycast(ray, out hit))
            {
                // Check if we hit a Cattle
                Cattle clickedCattle = hit.collider.GetComponent<Cattle>();
                
                if (clickedCattle != null)
                {
                    if (clickedCattle.currentState == AnimalState.Hungry)
                    {
                        // Open the feeding UI Menu
                        if (feedingMenuPanel != null)
                        {
                            feedingMenuPanel.SetActive(true);
                            Debug.Log($"Opened feeding menu for {clickedCattle.gameObject.name}");
                        }
                        else
                        {
                            Debug.LogWarning("Feeding Menu Panel is missing! Please assign it in the Inspector.");
                        }
                    }
                    else if (clickedCattle.currentState == AnimalState.ReadyToCollect)
                    {
                        // If tapped while ready, collect the milk/meat!
                        clickedCattle.CollectProduct();
                    }
                    else
                    {
                        Debug.Log($"{clickedCattle.gameObject.name} is busy producing.");
                    }
                }
            }
        }
    }
}