using UnityEngine;

public class CheckpointAnimationTrigger : MonoBehaviour
{
    public Collider checkpointTrigger;
    public Animator[] animators;
    private bool hasTriggered = false;

    void Start()
    {
        if (checkpointTrigger != null)
        {
            var trigger = checkpointTrigger.gameObject.AddComponent<CheckpointTriggerHandler>();
            trigger.Setup(this);
        }
    }

    public void ActivateAnimations()
    {
        if (hasTriggered) return;
        hasTriggered = true;

        foreach (var animator in animators)
        {
            if (animator != null)
            {
                animator.enabled = true;
                animator.Play(animator.GetCurrentAnimatorStateInfo(0).shortNameHash, 0, 0f);
            }
        }
    }
}

public class CheckpointTriggerHandler : MonoBehaviour
{
    private CheckpointAnimationTrigger parentTrigger;

    public void Setup(CheckpointAnimationTrigger trigger)
    {
        parentTrigger = trigger;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            parentTrigger.ActivateAnimations();
    }
}
