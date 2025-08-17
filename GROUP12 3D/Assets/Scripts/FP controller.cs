using UnityEngine;
using UnityEngine.InputSystem;

public class FPcontroller : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gravity = -9.81f;

    [Header("Jump Settings")]
    public float jumpHeight = 1.5f;    
    public float jumpDistance = 2f; 
    private Vector3 jumpVelocity;       

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;
    

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void HandleMovement()
    {
        
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * moveSpeed * Time.deltaTime);

        
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            jumpVelocity = Vector3.zero; 
        }

        
        velocity.y += gravity * Time.deltaTime;

        
        controller.Move((velocity + jumpVelocity) * Time.deltaTime);
    }

   public void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

       verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
   }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            PerformJump();
        }
    }

    private void PerformJump()
    {
        
        velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        
        Vector3 forward = new Vector3(transform.forward.x, 0, transform.forward.z).normalized;
        jumpVelocity = forward * jumpDistance;
    }
}
