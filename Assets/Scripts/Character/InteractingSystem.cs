using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InteractingSystem : MonoBehaviour
{
    [Header("Interaction Settings")]
    public float interactionRange = 3f; // Max distance for interaction
    public KeyCode interactKey = KeyCode.E; // Key to interact
    public KeyCode controllerInteractKey = KeyCode.JoystickButton0;

    [Header("UI Elements")]
    public GameObject interactPrompt; // UI element for interaction prompt
    public Text giftFoundText; // UI text to display "Found a hidden gift"
    public Text giftCountText; // UI text to display the gift count
    public GameObject winUI; // UI for winning message

    [Header("Player Camera")]
    public Camera playerCamera;
    public GameObject playerCharacter;

    [Header("UI Settings")]
    public Image fadeScreen;
    public float fadeDuration = 1.5f;

    [Header("Hiding Gift Locations")]
    public Transform[] hiddenGiftLocations; // All possible hiding spots for gifts

    private GameObject currentInteractable; // Tracks the currently interactable object
    private int giftCount = 0; // Number of gifts collected
    private int winThreshold = 10; // Number of gifts needed to win
    private List<GameObject> collectedGifts = new List<GameObject>(); // Tracks collected gifts
    private bool hasWon = false; // Tracks if the player has won

    void Start()
    {
        if (playerCamera == null)
        {
            Debug.LogError("No camera found. Ensure the player has a camera.");
        }

        // Initialize UI
        if (interactPrompt != null) interactPrompt.SetActive(false);
        if (giftFoundText != null) giftFoundText.gameObject.SetActive(false);
        if (giftCountText != null) UpdateGiftCountUI();
        if (winUI != null) winUI.SetActive(false);
    }

    void Update()
    {
        if (hasWon) return; // Stop interactions after winning

        CheckForInteractable();

        if ((Input.GetKeyDown(interactKey) || Input.GetKeyDown(controllerInteractKey)) && currentInteractable != null)
        {
            Interact();
        }
    }

    void CheckForInteractable()
    {
        // Raycast from the camera forward
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            GameObject hitObject = hit.collider.gameObject;

            // Check if the object is interactable
            if (hitObject.CompareTag("Door") || hitObject.CompareTag("hiddenGift"))
            {
                currentInteractable = hitObject;

                // Show the interaction prompt
                if (interactPrompt != null) interactPrompt.SetActive(true);
                return;
            }
        }

        // Hide the interaction prompt if no interactable is found
        currentInteractable = null;
        if (interactPrompt != null) interactPrompt.SetActive(false);
    }

    void Interact()
    {
        if (currentInteractable == null) return;

        // Handle doors
        if (currentInteractable.CompareTag("Door"))
        {
            Door door = currentInteractable.GetComponent<Door>();
            if (door != null) door.ToggleDoor();
        }

        // Handle hidden gifts
        else if (currentInteractable.CompareTag("hiddenGift"))
        {
            CollectGift(currentInteractable);
        }
    }

    void CollectGift(GameObject gift)
    {
        // Increment gift count
        giftCount++;
        UpdateGiftCountUI();

        // Track collected gift
        collectedGifts.Add(gift);

        // Destroy the gift object
        Destroy(gift);

        // Show "Found a hidden gift" message
        if (giftFoundText != null) StartCoroutine(ShowGiftFoundMessage());

        // Check for win condition
        if (giftCount >= winThreshold)
        {
            TriggerWin();
        }
    }

    IEnumerator ShowGiftFoundMessage()
    {
        if (giftFoundText != null)
        {
            giftFoundText.gameObject.SetActive(true);
            yield return new WaitForSeconds(3f);
            giftFoundText.gameObject.SetActive(false);
        }
    }

    void UpdateGiftCountUI()
    {
        if (giftCountText != null)
        {
            giftCountText.text = $"Gifts Found: {giftCount} / 10";
        }
    }

    void TriggerWin()
    {
        hasWon = true;
        if (winUI != null)
        {
            StartCoroutine(FadeScreen(true));
            
            
        }
        Debug.Log("You Win!");
    }

    public void ResetGifts()
    {
        // Reset the gift count
        giftCount = 0;
        UpdateGiftCountUI();

        // Return collected gifts to random hiding spots
        ShuffleHidingSpots(hiddenGiftLocations);

        foreach (GameObject gift in collectedGifts)
        {
            if (hiddenGiftLocations.Length > 0)
            {
                // Reposition the gift to a random hiding spot
                Transform randomSpot = hiddenGiftLocations[Random.Range(0, hiddenGiftLocations.Length)];
                gift.transform.position = randomSpot.position;
                gift.SetActive(true);
                gift.tag = "hiddenGift";
            }
        }

        // Clear the list of collected gifts
        collectedGifts.Clear();

        Debug.Log("Gifts have been reset to hiding spots.");
    }

    IEnumerator FadeScreen(bool fadeToBlack)
    {
        if (fadeScreen == null) yield break;

        fadeScreen.gameObject.SetActive(true);

        float elapsedTime = 0f;
        Color color = fadeScreen.color;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = fadeToBlack
                ? Mathf.Clamp01(elapsedTime / fadeDuration)
                : Mathf.Clamp01(1f - elapsedTime / fadeDuration);

            color.a = alpha;
            fadeScreen.color = color;

            yield return null;
        }

        SceneManager.LoadScene("Ending_Main");

        if (!fadeToBlack)
        {
            fadeScreen.gameObject.SetActive(false);
        }
    }

    void ShuffleHidingSpots(Transform[] spots)
    {
        for (int i = spots.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Transform temp = spots[i];
            spots[i] = spots[randomIndex];
            spots[randomIndex] = temp;
        }
    }
}
