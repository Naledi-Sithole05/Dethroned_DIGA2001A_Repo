using UnityEngine;

public class GuardAnimationStateContoller : MonoBehaviour
{
    Animator animator;
    int isWalkingHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");
    }

    // Update is called once per frame
    void Update()
    {
        bool isWalking = animator.GetBool(isWalkingHash);
        if (isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
        if (!isWalking)
        {
            animator.SetBool(isWalkingHash, true);
        }
    }
}
