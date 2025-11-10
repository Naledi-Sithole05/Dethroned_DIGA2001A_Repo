using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    public CharacterController _Player;
    int isWalkingHash;
    int isJumpingHash;
    bool jumpStarted = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    void Update()
    {
        bool forwardPressed = Input.GetKey("w");
        bool jumpPressed = Input.GetKeyDown("space");
        bool isGrounded = _Player.isGrounded;

        // Walking logic
        animator.SetBool(isWalkingHash, forwardPressed);

        // Jump trigger
        if (jumpPressed && isGrounded)
        {
            animator.SetBool(isJumpingHash, true);
            jumpStarted = true;
        }

        // Reset jump only when character lands
        if (jumpStarted && isGrounded)
        {
            animator.SetBool(isJumpingHash, false);
            jumpStarted = false;
        }
    }
}



