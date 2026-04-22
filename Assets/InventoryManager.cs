using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    // Singleton instance
    public static InventoryManager Instance { get; private set; }

    // Dictionary to store Item ID as the key, and Quantity as the value
    private Dictionary<string, int> inventory = new Dictionary<string, int>();

    private void Awake()
    {
        // Enforce the Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject); // Keeps inventory alive across scene loads
        }
    }

    private void Start()
    {
        // Add some starting items for testing purposes
        AddItem("cow_feed", 5);
    }

    public void AddItem(string itemID, int amount)
    {
        if (inventory.ContainsKey(itemID))
        {
            inventory[itemID] += amount;
        }
        else
        {
            inventory.Add(itemID, amount);
        }
        Debug.Log($"Added {amount} of {itemID}. Total: {inventory[itemID]}");
        
        // TODO: Update UI text here
    }

    public bool RemoveItem(string itemID, int amount)
    {
        if (HasItem(itemID, amount))
        {
            inventory[itemID] -= amount;
            Debug.Log($"Removed {amount} of {itemID}. Remaining: {inventory[itemID]}");
            
            // TODO: Update UI text here
            return true;
        }
        
        Debug.LogWarning($"Not enough {itemID} in inventory!");
        return false;
    }

    public bool HasItem(string itemID, int requiredAmount)
    {
        return inventory.ContainsKey(itemID) && inventory[itemID] >= requiredAmount;
    }
}