using System;
using System.Collections;
using UnityEngine;

public enum AnimalState 
{ 
    Hungry, 
    Producing, 
    ReadyToCollect 
}

public class Cattle : MonoBehaviour
{
    [Header("Cattle Configuration")]
    public string animalID = "cow_01"; // Unique ID for saving/loading
    public float productionDuration = 120f; // Time in seconds (e.g., 2 minutes)
    public string feedItemID = "cow_feed";
    public string productItemID = "milk";

    [Header("Current State")]
    public AnimalState currentState = AnimalState.Hungry;
    
    // To track offline progress
    private DateTime productionEndTime; 

    private void Start()
    {
        // On startup, check if the animal was producing while the game was closed
        LoadState();
    }

    /// <summary>
    /// Called when the player successfully drops feed onto the cattle.
    /// </summary>
    public void FeedAnimal()
    {
        if (currentState == AnimalState.Hungry)
        {
            currentState = AnimalState.Producing;
            
            // Calculate when the product will be ready
            productionEndTime = DateTime.Now.AddSeconds(productionDuration);
            SaveState();

            // TODO: Trigger Eating Animation here
            // GetComponent<Animator>().SetTrigger("Eat");

            Debug.Log($"{gameObject.name} is eating. Product ready in {productionDuration} seconds.");
            StartCoroutine(ProduceGoodsCoroutine(productionDuration));
        }
        else
        {
            Debug.LogWarning($"{gameObject.name} is not hungry right now!");
        }
    }

    /// <summary>
    /// Handles the real-time timer while the game is open.
    /// </summary>
    private IEnumerator ProduceGoodsCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        CompleteProduction();
    }

    /// <summary>
    /// Transitions the cattle to the Ready state.
    /// </summary>
    private void CompleteProduction()
    {
        currentState = AnimalState.ReadyToCollect;
        
        // TODO: Show UI indicator (e.g., a milk bucket icon above the cow)
        // milkBubbleVisual.SetActive(true);

        Debug.Log($"{gameObject.name} has finished producing {productItemID}!");
    }

    /// <summary>
    /// Called when the player taps the cattle to collect the finished product.
    /// </summary>
    public void CollectProduct()
    {
        if (currentState == AnimalState.ReadyToCollect)
        {
            currentState = AnimalState.Hungry;
            SaveState();

            // Hide the UI milk bubble (if you have one)
            // milkBubbleVisual.SetActive(false);

            // --- THE NEW LINK ---
            // Add the product (e.g., "milk") to the inventory
            InventoryManager.Instance.AddItem(productItemID, 1);
            
            Debug.Log($"Harvested {productItemID}! Cow is hungry again.");
        }
    }

    #region Save/Load System (Mockup)
    
    // In a real project, replace PlayerPrefs with your JSON/Database save system.
    private void SaveState()
    {
        PlayerPrefs.SetString($"{animalID}_State", currentState.ToString());
        PlayerPrefs.SetString($"{animalID}_EndTime", productionEndTime.ToString());
        PlayerPrefs.Save();
    }

    private void LoadState()
    {
        if (PlayerPrefs.HasKey($"{animalID}_State"))
        {
            string savedState = PlayerPrefs.GetString($"{animalID}_State");
            if (Enum.TryParse(savedState, out AnimalState parsedState))
            {
                currentState = parsedState;
            }

            if (currentState == AnimalState.Producing)
            {
                string savedTime = PlayerPrefs.GetString($"{animalID}_EndTime");
                if (DateTime.TryParse(savedTime, out DateTime parsedTime))
                {
                    productionEndTime = parsedTime;
                    CheckOfflineProgress();
                }
            }
        }
    }

    private void CheckOfflineProgress()
    {
        TimeSpan timeRemaining = productionEndTime - DateTime.Now;

        if (timeRemaining.TotalSeconds <= 0)
        {
            // Time passed while the game was closed
            CompleteProduction();
        }
        else
        {
            // Still producing, resume the coroutine
            StartCoroutine(ProduceGoodsCoroutine((float)timeRemaining.TotalSeconds));
        }
    }

    #endregion
}