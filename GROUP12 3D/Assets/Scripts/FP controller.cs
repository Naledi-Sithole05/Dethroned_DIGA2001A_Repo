 using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 6f;
    public float runSpeed = 12f;
    public float gravity = 20f;
    public float jumpPower = 7f;

    [Header("Look")]
    public Camera playerCamera;
    public float lookSpeed = 2f;
    public float lookXLimit = 80f;

    [Header("Crouch")]
    public float crouchHeight = 1f;
    public float crouchSpeed = 3f;
    private float defaultHeight;
    private bool isCrouching;

    private CharacterController characterController;
    private Vector3 moveDirection = Vector3.zero;
    private float rotationX = 0;
    private bool canMove = true;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        defaultHeight = characterController.height;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // Movement
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float curSpeedX = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Vertical") : 0;
        float curSpeedY = canMove ? (isRunning ? runSpeed : walkSpeed) * Input.GetAxis("Horizontal") : 0;
        float moveDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);

        // Jump
        if (Input.GetButton("Jump") && canMove && characterController.isGrounded)
        {
            moveDirection.y = jumpPower;
        }
        else
        {
            moveDirection.y = moveDirectionY;
        }

        // Gravity
        if (!characterController.isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }

        // Crouch
        if (Input.GetKeyDown(KeyCode.R))
        {
            isCrouching = !isCrouching;
            characterController.height = isCrouching ? crouchHeight : defaultHeight;
            walkSpeed = isCrouching ? crouchSpeed : 6f;
            runSpeed = isCrouching ? crouchSpeed * 1.5f : 12f;
        }

        // Apply movement
        characterController.Move(moveDirection * Time.deltaTime);

        // Camera rotation
        if (canMove)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
    }
}