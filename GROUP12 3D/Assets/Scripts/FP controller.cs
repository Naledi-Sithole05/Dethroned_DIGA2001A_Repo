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

   
    public float gravity = -25f;
    public float jumpSpeedMultiplier = 1.5f;

    [Header("Speed Boost Settings")]
    public float boostSpeed = 12f;
    public float boostDuration = 5f;

    [Header("Jump Boost Settings")]
    
    public float jumpBoostMultiplier = 2f;
    
    public float jumpBoostDuration = 5f;

    [Header("Invisibility Settings")]
   
    public bool isInvisible = false;
    private float invisibilityTimer = 0f;

    [Header("UI Settings")]
    public Slider boostSlider;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2.5f;


    [Header("ANIMATION SETTINGS")]
    [Space(5)]
    public Animator animator;
   
    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation = 0f;

    private float defaultMoveSpeed;
    private float defaultJumpHeight;
    private bool isRunning;
    private bool isCrouching;
    private bool isBoosted = false;
    private float remainingBoostTime = 0f;

    private Coroutine jumpBoostRoutine;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultMoveSpeed = moveSpeed;
        defaultJumpHeight = jumpHeight;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (boostSlider != null)
            boostSlider.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleBoostDecay();
        HandleInvisibilityTimer();
        UpdateBoostUI();
    }

    #region Input Methods
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
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * jumpSpeedMultiplier;
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
    #endregion

    #region Movement & Look
    private void HandleMovement()
    {
        float currentSpeed = moveSpeed;

        // Adjust speeds
        if (isBoosted)
            currentSpeed = boostSpeed;
        else if (isRunning && !isCrouching)
            currentSpeed = runSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        // Gravity handling
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        //Animation
        float movementMagnitude = new Vector2(moveInput .x, moveInput.y) .magnitude ;
        animator. SetFloat("Speed", movementMagnitude );
    }

    private void HandleLook()
    {
        float mouseX = lookInput.x * lookSensitivity;
        float mouseY = lookInput.y * lookSensitivity;

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -verticalLookLimit, verticalLookLimit);

        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);

        float movementMagnitude = new Vector2 (moveInput.x, moveInput.y) .magnitude;
    }
    #endregion

    #region Boost Logic
    private void HandleBoostDecay()
    {
        if (isBoosted && moveInput.magnitude > 0f)
        {
            remainingBoostTime -= Time.deltaTime;
            if (remainingBoostTime <= 0f)
            {
                isBoosted = false;
                moveSpeed = defaultMoveSpeed;

                if (boostSlider != null)
                    boostSlider.gameObject.SetActive(false);
            }
        }
    }

    private void UpdateBoostUI()
    {
        if (isBoosted && boostSlider != null)
            boostSlider.value = remainingBoostTime;
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // SPEED BOOST pickup
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

        // JUMP BOOST pickup
        if (hit.collider.CompareTag("JumpBoost"))
        {
            ApplyJumpBoost(jumpBoostMultiplier, jumpBoostDuration);

            JumpBoostPickup pickup = hit.collider.GetComponent<JumpBoostPickup>();
            if (pickup != null)
                pickup.StartRespawn();
        }

        
        if (hit.collider.CompareTag("InvisibilityPowerUp"))
        {
            InvisibilityPickup pickup = hit.collider.GetComponent<InvisibilityPickup>();
            if (pickup != null)
            {
                // Give the player invisibility
                ActivateInvisibility(pickup.invisibilityDuration);

               
            }
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
    #endregion

    #region Invisibility
    public void ActivateInvisibility(float duration)
    {
        isInvisible = true;
        invisibilityTimer = duration;
        Debug.Log("Player is now invisible to guards!");
    }

    private void HandleInvisibilityTimer()
    {
        if (isInvisible)
        {
            invisibilityTimer -= Time.deltaTime;
            if (invisibilityTimer <= 0f)
            {
                isInvisible = false;
                Debug.Log("Invisibility has worn off.");
            }
        }
    }
    #endregion
}
