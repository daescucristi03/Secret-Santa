using UnityEngine;
using UnityEngine.UI;

public class GiftCounter : MonoBehaviour
{
    [Header("UI Settings")]
    public Text giftCountText; // UI Text to display the gift count

    private int giftCount = 0; // Keeps track of how many gifts the player has found

    void Start()
    {
        // Initialize the UI text
        UpdateGiftCountUI();
    }

    public void AddGift()
    {
        // Increment the gift count
        giftCount++;

        // Update the UI
        UpdateGiftCountUI();
    }

    private void UpdateGiftCountUI()
    {
        // Update the text to display the current gift count
        if (giftCountText != null)
        {
            giftCountText.text = $"Gifts Found: {giftCount} / 10";
        }
        else
        {
            Debug.LogWarning("GiftCountText is not assigned!");
        }
    }
}
