 using UnityEngine;
using UnityEngine.InputSystem;
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

    [Header("Look Settings")]
    public Transform cameraTransform;
    public float lookSensitivity = 2f;
    public float verticalLookLimit = 90f;

   // [Header("Crouch Settings")]
   // public float crouchHeight = 1f;
   // public float standHeight = 2f;
    //public float crouchSpeed = 2.5f;

    [Header("Pickup / Throw Settings")]
    public Transform holdPoint;           // empty in front of camera
    public float pickupRange = 3f;
    public float throwForwardForce = 10f;
    public float throwUpwardForce = 5f;

    [Header("UI Settings")]
    public TMP_Text interactText;         // appears when near object
    public TMP_Text throwText;            // appears after pickup

    private CharacterController controller;
    private Vector2 moveInput;
    private Vector2 lookInput;
    private Vector3 velocity;
    private float verticalRotation;

    private float defaultMoveSpeed;
    private bool isRunning;
    private bool isCrouching;

    // Pickup
    private Rigidbody heldObject;
    private Collider heldCollider;
    private bool isHolding = false;
    private GameObject nearestObject;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        defaultMoveSpeed = moveSpeed;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (interactText != null) interactText.gameObject.SetActive(false);
        if (throwText != null) throwText.gameObject.SetActive(false);
    }

    private void Update()
    {
        HandleMovement();
        HandleLook();
        DetectPickupObject();
        UpdateHeldObjectPosition();
    }

    private void HandleMovement()
    {
        // Check if controller is active before moving
        if (controller == null || !controller.enabled)
            return;

        float currentSpeed = isRunning ? runSpeed : moveSpeed;
        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        controller.Move(move * currentSpeed * Time.deltaTime);

        if (controller.isGrounded && velocity.y < 0) velocity.y = -2f;
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

    public void OnMove(InputAction.CallbackContext context) => moveInput = context.ReadValue<Vector2>();
    public void OnLook(InputAction.CallbackContext context) => lookInput = context.ReadValue<Vector2>();
    public void OnRun(InputAction.CallbackContext context) => isRunning = context.ReadValueAsButton();
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && controller.isGrounded)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity) * jumpSpeedMultiplier;
    }

   // public void OnCrouch(InputAction.CallbackContext context)
   // {
      //  if (context.performed)
      //  {
          //  isCrouching = true;
       //     controller.height = crouchHeight;
      //      moveSpeed = crouchSpeed;
    //    }
   //     else if (context.canceled)
    //    {
   //         isCrouching = false;
   //         controller.height = standHeight;
    //        moveSpeed = defaultMoveSpeed;
  //      }
  //  }

    public void OnPickUp(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (isHolding)
            DropObject();
        else if (nearestObject != null)
            PickUpObject();
    }

    public void OnThrow(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        if (isHolding) ThrowObject();
    }

    private void DetectPickupObject()
    {
        if (isHolding) return;

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, pickupRange))
        {
            if (hit.collider.CompareTag("Interact"))
            {
                nearestObject = hit.collider.gameObject;
                if (interactText != null)
                {
                    interactText.text = "Press E or West Button to pick up";
                    interactText.gameObject.SetActive(true);
                }
                return;
            }
        }

        nearestObject = null;
        if (interactText != null) interactText.gameObject.SetActive(false);
    }

    private void PickUpObject()
    {
        if (nearestObject == null) return;

        heldObject = nearestObject.GetComponent<Rigidbody>();
        heldCollider = nearestObject.GetComponent<Collider>();
        if (heldObject != null && heldCollider != null)
        {
            isHolding = true;

            // Disable gravity, keep Rigidbody non-kinematic
            heldObject.useGravity = false;

            // Reset velocities safely
            heldObject.linearVelocity = Vector3.zero;
            heldObject.angularVelocity = Vector3.zero;

            // Make collider a trigger
            heldCollider.isTrigger = true;

            // Show throw text
            if (throwText != null)
            {
                throwText.text = "Press 'G' to throw or the North Button";
                throwText.gameObject.SetActive(true);
            }

            if (interactText != null)
                interactText.gameObject.SetActive(false);
        }
    }

    private void UpdateHeldObjectPosition()
    {
        if (isHolding && heldObject != null)
        {
            // Smoothly move in front of camera without parenting
            heldObject.MovePosition(Vector3.Lerp(heldObject.position, holdPoint.position, Time.deltaTime * 10f));
            heldObject.MoveRotation(Quaternion.Lerp(heldObject.rotation, holdPoint.rotation, Time.deltaTime * 10f));
        }
    }

    private void DropObject()
    {
        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldCollider.isTrigger = false;
            heldObject = null;
            heldCollider = null;
            isHolding = false;
        }

        if (throwText != null)
            throwText.gameObject.SetActive(false);
    }

    private void ThrowObject()
    {
        if (heldObject != null)
        {
            heldObject.useGravity = true;
            heldCollider.isTrigger = false;

            heldObject.AddForce(cameraTransform.forward * throwForwardForce, ForceMode.Impulse);
            heldObject.AddForce(cameraTransform.up * throwUpwardForce, ForceMode.Impulse);

            heldObject = null;
            heldCollider = null;
            isHolding = false;

            if (throwText != null)
                throwText.gameObject.SetActive(false);
        }
    }

    // Add this to your FPController script
    public void ResetPlayer()
    {
        // Reset movement variables
        velocity = Vector3.zero;
        moveInput = Vector2.zero;
        isRunning = false;
        
        Debug.Log("FPController: Player reset - movement variables cleared");
    }
}