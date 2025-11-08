using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections;
using TMPro;  



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
    public TMP_Text interactText;

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;

    [Header("Crouch Settings")]
    public float crouchHeight = 1f;
    public float standHeight = 2f;
    public float crouchSpeed = 2.5f;

    [Header("Throw & Pickup Settings")]
    public Transform throwPoint;
    public float throwForce = 10f;
    public float pickupRange = 3f;
    public float holdSmoothness = 10f;

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

    private Rigidbody heldObject;
    private bool isHolding = false;
    private InteractableObject nearbyObject;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultMoveSpeed = moveSpeed;
        defaultJumpHeight = jumpHeight;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (boostSlider != null)
            boostSlider.gameObject.SetActive(false);

        if (interactText != null)
            interactText.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        HandleBoostDecay();
        HandleInvisibilityTimer();
        UpdateBoostUI();

        if (isHolding && heldObject != null)
        {
            Vector3 targetPos = throwPoint.position;
            heldObject.MovePosition(Vector3.Lerp(heldObject.position, targetPos, Time.deltaTime * holdSmoothness));
        }
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
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * jumpSpeedMultiplier;
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

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (isHolding)
                DropObject();
            else
                TryPickUpObject();
        }
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (context.performed && isHolding && heldObject != null)
            ThrowObject();
    }

    

    private void TryPickUpObject()
    {
        if (nearbyObject != null && nearbyObject.CompareTag("Interact"))
        {
            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                heldObject = rb;
                heldObject.useGravity = false;
                heldObject.linearDamping = 10f; //  replaced drag
                heldObject.constraints = RigidbodyConstraints.FreezeRotation;
                isHolding = true;

                nearbyObject.HidePrompt();
                nearbyObject = null;
                Debug.Log("Picked up: " + heldObject.name);
            }
        }
        else
        {
            Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
            {
                if (hit.collider.CompareTag("Interact"))
                {
                    Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
                    if (rb != null)
                    {
                        heldObject = rb;
                        heldObject.useGravity = false;
                        heldObject.linearDamping = 10f;
                        heldObject.constraints = RigidbodyConstraints.FreezeRotation;
                        isHolding = true;
                        Debug.Log("Picked up (via raycast): " + hit.collider.name);
                    }
                }
            }
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldObject.linearDamping = 1f;
            heldObject.constraints = RigidbodyConstraints.None;
            heldObject = null;
        }
        isHolding = false;
        Debug.Log("Dropped object.");
    }

    private void ThrowObject()
    {
        heldObject.useGravity = true;
        heldObject.linearDamping = 1f;
        heldObject.constraints = RigidbodyConstraints.None;
        heldObject.AddForce(cameraTransform.forward * throwForce, ForceMode.VelocityChange);
        Debug.Log($"Threw {heldObject.name}");
        heldObject = null;
        isHolding = false;
    }

    

    public void SetNearbyObject(InteractableObject obj)
    {
        nearbyObject = obj;

        if (interactText != null && !isHolding)
        {
            interactText.text = "Press E to pick up";
            interactText.gameObject.SetActive(true);
        }
    }

    public void ClearNearbyObject(InteractableObject obj)
    {
        if (nearbyObject == obj)
        {
            nearbyObject = null;
            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }

    private void HandleMovement()
    {
        float currentSpeed = moveSpeed;

        if (isBoosted)
            currentSpeed = boostSpeed;
        else if (isRunning && !isCrouching)
            currentSpeed = runSpeed;

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;

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

        if (hit.collider.CompareTag("JumpBoost"))
        {
            ApplyJumpBoost(jumpBoostMultiplier, jumpBoostDuration);
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

    public void ActivateInvisibility(float duration)
    {
        isInvisible = true;
        invisibilityTimer = duration;
    }

    private void HandleInvisibilityTimer()
    {
        if (isInvisible)
        {
            invisibilityTimer -= Time.deltaTime;
            if (invisibilityTimer <= 0f)
                isInvisible = false;
        }
    }
}
