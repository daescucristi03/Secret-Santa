using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 5f;
    public float sprintSpeed = 8f;
    public float gravity = -9.8f;

    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode sprintControllerKey = KeyCode.JoystickButton8;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 100f;
    public Transform playerCamera;
    public float maxLookAngle = 80f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;


    void Awake()
    {
        // Dynamically add required components
        if (!GetComponent<CharacterController>())
        {
            characterController = gameObject.AddComponent<CharacterController>();
            characterController.height = 2f;
            characterController.center = new Vector3(0, 1f, 0);
            characterController.radius = 0.5f;
        }
        else
        {
            characterController = GetComponent<CharacterController>();
        }

        if (!playerCamera)
        {
            Debug.LogError("Player Camera not assigned. Please assign a camera to the script.");
            enabled = false;
        }
    }

    void Start()
    {
        // Lock the cursor to the game window
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    float findBestXLookValue() {
        if (Input.GetAxis("Controller X") != 0)
        return Input.GetAxis("Controller X");
        else return Input.GetAxis("Mouse X");
    }

    float findBestYLookValue() {
        if (Input.GetAxis("Controller Y") != 0)
        return Input.GetAxis("Controller Y");
        else return Input.GetAxis("Mouse Y");
    }

    private KeyCode findSprintKey() {
        if(Input.GetKey(sprintKey))
        return sprintKey;

        else return sprintControllerKey;
    }

    void HandleMouseLook()
    {
        
        // Get mouse input
        float mouseX = findBestXLookValue() * mouseSensitivity * Time.deltaTime;
        float mouseY = findBestYLookValue() * mouseSensitivity * Time.deltaTime;

        // Rotate player horizontally
        transform.Rotate(Vector3.up * mouseX);

        // Rotate camera vertically
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    void HandleMovement()
    {
        // Get input for movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        // Determine the move direction
        Vector3 move = transform.right * moveX + transform.forward * moveZ;

        float currentSpeed;
        // Handle sprinting
        if(Input.GetKey(findSprintKey()))
        {
            currentSpeed = sprintSpeed;
        } else currentSpeed = walkSpeed;

        // Apply movement
        characterController.Move(move * currentSpeed * Time.deltaTime);

        // Apply gravity
        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Keep the player grounded
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
}
