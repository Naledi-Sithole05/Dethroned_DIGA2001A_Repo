using UnityEngine;

public class animationStateController : MonoBehaviour
{
    Animator animator;
    public CharacterController _Player;
    int isWalkingHash;
    int isJumpingHash;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
        isJumpingHash = Animator.StringToHash("isJumping");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        bool isJumping = animator.GetBool(isJumpingHash);
        bool forwardPressed = Input.GetKey("w");
        bool jumpPressed = Input.GetKey("space");

        if (!isWalking && forwardPressed)
        {
            animator.SetBool(isWalkingHash, true);

        }

        if (isWalking && !forwardPressed)
        {
            animator.SetBool(isWalkingHash, false);

        }

        if  (jumpPressed)
        {
          animator.SetBool (isJumpingHash, true);
        }

        if (_Player.isGrounded)
        {
            animator.SetBool(isJumpingHash, false);
        }


    }

}

