using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public KeyCode toggleKey = KeyCode.F; // Key to toggle the flashlight
    public Light flashlight; // Reference to the flashlight (Spotlight or Point Light)

    private bool isFlashlightOn = true; // Tracks the flashlight's state

    void Start()
    {
        if (flashlight == null)
        {
            Debug.LogError("No flashlight assigned. Please attach a Light component.");
            enabled = false;
            return;
        }

        // Ensure the flashlight starts in the correct state
        flashlight.enabled = isFlashlightOn;
    }

    void Update()
    {
        // Check if the toggle key is pressed
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        isFlashlightOn = !isFlashlightOn;
        flashlight.enabled = isFlashlightOn;
    }
}
