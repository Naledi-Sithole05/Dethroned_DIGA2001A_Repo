using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    private InputMaster controls;         // Input actions (generated class)
    [SerializeField] private float mouseSensitivity = 60f;

    private Vector2 mouseLook;            // Stores raw input from mouse/stick
    private float xRotation = 0f;         // Tracks vertical camera rotation
    [SerializeField] private Transform playerBody; // Player root (rotates left/right)

    private void Awake()
    {
        controls = new InputMaster();
        Cursor.lockState = CursorLockMode.Locked;  // Hide and lock cursor to screen center
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    private void Update()
    {
        Look();
    }

    private void Look()
    {
        // Get input from Input System (mouse delta / right stick)
        mouseLook = controls.Player.Look.ReadValue<Vector2>();

        float mouseX = mouseLook.x * mouseSensitivity * Time.deltaTime;
        float mouseY = mouseLook.y * mouseSensitivity * Time.deltaTime;

        // Pitch (vertical rotation) on the camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f); // Prevent over-rotating up/down
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Yaw (horizontal rotation) on the player body
        playerBody.Rotate(Vector3.up * mouseX);
    }
}















