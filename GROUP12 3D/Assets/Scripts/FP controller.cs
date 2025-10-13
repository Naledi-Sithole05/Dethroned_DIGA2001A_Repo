using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;   
using System.Collections; 

[RequireComponent(typeof(CharacterController))]
public class FPController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runSpeed = 8f;
    public float jumpHeight = 1.5f;
    public float gravity = -9.81f;

    [Header("Speed Boost Settings")]
    public float boostSpeed = 12f;          
    public float boostDuration = 5f;         
    private float remainingBoostTime = 0f;   
    private bool isBoosted = false;

    [Header("Jump Boost Settings")]
    [Tooltip("How much the jump height is multiplied during a jump boost.")]
    public float jumpBoostMultiplier = 2f;

    [Tooltip("How long the jump boost effect lasts (in seconds).")]
    public float jumpBoostDuration = 5f;

    private Coroutine jumpBoostRoutine; // Handles jump boost timer

    [Header("UI Settings")]
    public Slider boostSlider;               // Reference to UI slider

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2.5f;

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private float defaultMoveSpeed;
    private float defaultJumpHeight;
    private bool isRunning;
    private bool isCrouching;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultMoveSpeed = moveSpeed;
        defaultJumpHeight = jumpHeight;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (boostSlider != null)
            boostSlider.gameObject.SetActive(false); // Hide at start
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleBoostDecay();
        UpdateBoostUI();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        lookInput = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        isRunning = context.ReadValueAsButton();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            isCrouching = true;
            controller.height = crouchHeight;
            moveSpeed = crouchSpeed;
        }
        else if (context.canceled)
        {
            isCrouching = false;
            controller.height = standHeight;
            moveSpeed = defaultMoveSpeed;
        }
    }

    private void HandleMovement()
    {
        float currentSpeed = moveSpeed;

        // If boosted, override speed
        if (isBoosted)
            currentSpeed = boostSpeed;

        if (isRunning && !isCrouching)
            currentSpeed = runSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void HandleBoostDecay()
    {
        if (isBoosted && moveInput.magnitude > 0f) // Only deplete boost if moving
        {
            remainingBoostTime -= Time.deltaTime;
            if (remainingBoostTime <= 0f)
            {
                isBoosted = false;
                moveSpeed = defaultMoveSpeed; // Reset to normal

                if (boostSlider != null)
                    boostSlider.gameObject.SetActive(false); // Hide UI
            }
        }
    }

    private void UpdateBoostUI()
    {
        if (isBoosted && boostSlider != null)
        {
            boostSlider.value = remainingBoostTime;
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Handle Speed Boost
        if (hit.collider.CompareTag("SpeedBoost"))
        {
            isBoosted = true;
            remainingBoostTime = boostDuration;
            moveSpeed = boostSpeed;

            if (boostSlider != null)
            {
                boostSlider.maxValue = boostDuration;
                boostSlider.value = boostDuration;
                boostSlider.gameObject.SetActive(true);
            }

            Destroy(hit.gameObject);
        }

        // Handle Jump Boost
        if (hit.collider.CompareTag("JumpBoost"))
        {
            ApplyJumpBoost(jumpBoostMultiplier, jumpBoostDuration);

            // Disable the jump boost object and respawn it later
            JumpBoostPickup pickup = hit.collider.GetComponent<JumpBoostPickup>();
            if (pickup != null)
                pickup.StartRespawn();

        }
    }

    
    public void ApplyJumpBoost(float multiplier, float duration)
    {
        if (jumpBoostRoutine != null)
            StopCoroutine(jumpBoostRoutine);

        jumpBoostRoutine = StartCoroutine(JumpBoostRoutine(multiplier, duration));
    }

    private IEnumerator JumpBoostRoutine(float multiplier, float duration)
    {
        jumpHeight = defaultJumpHeight * multiplier;

        yield return new WaitForSeconds(duration);

        jumpHeight = defaultJumpHeight;
        jumpBoostRoutine = null;
    }
}
