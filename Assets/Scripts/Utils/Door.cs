using UnityEngine;

public class Door : MonoBehaviour
{
    private bool isOpen = false;
    private Quaternion closedRotation;
    private Quaternion openRotation;
    public Transform doorTransform; // Reference to the door Transform
    public float doorOpenAngle = 90f; // Angle to open the door
    public float doorSpeed = 2f; // Speed of the door animation

    void Start()
    {
        if (doorTransform == null)
        {
            doorTransform = transform;
        }

        // Save the initial closed rotation
        closedRotation = doorTransform.rotation;

        // Calculate the open rotation based on the open angle
        openRotation = closedRotation * Quaternion.Euler(0, doorOpenAngle, 0);
    }

    void Update()
    {
        // Smoothly animate the door's rotation
        doorTransform.rotation = Quaternion.Slerp(doorTransform.rotation, isOpen ? openRotation : closedRotation, Time.deltaTime * doorSpeed);
    }

    public void ToggleDoor()
    {
        isOpen = !isOpen;
    }
}
