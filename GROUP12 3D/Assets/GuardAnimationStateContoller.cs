using UnityEngine;

public class GuardAnimationStateController : MonoBehaviour
{
    private Animator animator;
    private int isWalkingHash;

    void Start()
    {
        animator = GetComponent<Animator>();
        isWalkingHash = Animator.StringToHash("isWalking");

        // Ensure the parameter exists before trying to set it
        if (animator.HasParameterOfType(isWalkingHash, AnimatorControllerParameterType.Bool))
        {
            animator.SetBool(isWalkingHash, true); // Guard will always walk
        }
        else
        {
            Debug.LogWarning(" Animator missing parameter 'isWalking' on " + gameObject.name);
        }
    }

    void Update()
    {
        // Optional: Add patrol or AI movement later here.
        // The guard will stay in the walking animation continuously.
    }
}

//  Extension helper (you can leave this in the same file)
public static class AnimatorExtensions
{
    public static bool HasParameterOfType(this Animator self, int hash, AnimatorControllerParameterType type)
    {
        foreach (var param in self.parameters)
        {
            if (param.type == type && param.nameHash == hash)
                return true;
        }
        return false;
    }
}

