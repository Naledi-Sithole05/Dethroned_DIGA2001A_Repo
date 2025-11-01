using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointManager : MonoBehaviour
{
    [System.Serializable]
    public class Checkpoint
    {
        public Collider startTrigger;
        public Collider endTrigger;
        public Sprite checkpointSprite; // Sprite to show on UI
    }

    public Checkpoint[] checkpoints;
    public Image checkpointUIImage; // Reference to the UI Image in Canvas
    public float displayDuration = 2f;
    public float fadeDuration = 1f;

    private bool isFading = false;
    private Transform player;
    private Vector3 lastCheckpointPosition;
    private bool checkpointReached = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (checkpointUIImage != null)
        {
            Color c = checkpointUIImage.color;
            c.a = 0f;
            checkpointUIImage.color = c;
        }

        foreach (var cp in checkpoints)
        {
            if (cp.startTrigger != null)
            {
                var startTriggerScript = cp.startTrigger.gameObject.AddComponent<CheckpointTrigger>();
                startTriggerScript.Setup(this, cp, true);
            }
            if (cp.endTrigger != null)
            {
                var endTriggerScript = cp.endTrigger.gameObject.AddComponent<CheckpointTrigger>();
                endTriggerScript.Setup(this, cp, false);
            }
        }

        if (player != null)
            lastCheckpointPosition = player.position;
    }

    public void TriggerCheckpoint(Checkpoint cp, bool isStart)
    {
        if (isStart)
        {
            lastCheckpointPosition = cp.startTrigger.transform.position;
            checkpointReached = true;

            if (!isFading && checkpointUIImage != null && cp.checkpointSprite != null)
                StartCoroutine(FadeCheckpointSprite(cp.checkpointSprite));
        }
        else
        {
            Debug.Log($"{cp.checkpointSprite?.name ?? "Checkpoint"} - End trigger reached!");
        }
    }

    private IEnumerator FadeCheckpointSprite(Sprite sprite)
    {
        isFading = true;
        checkpointUIImage.sprite = sprite;

        yield return StartCoroutine(FadeImageAlpha(0f, 1f));
        yield return new WaitForSeconds(displayDuration);
        yield return StartCoroutine(FadeImageAlpha(1f, 0f));

        isFading = false;
    }

    private IEnumerator FadeImageAlpha(float from, float to)
    {
        float elapsed = 0f;
        Color c = checkpointUIImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, elapsed / fadeDuration);
            checkpointUIImage.color = c;
            yield return null;
        }

        c.a = to;
        checkpointUIImage.color = c;
    }

    public void RespawnPlayer()
    {
        if (player != null && checkpointReached)
        {
            Vector3 respawnPos = lastCheckpointPosition + Vector3.up * 1f;

            var controller = player.GetComponent<CharacterController>();
            var rb = player.GetComponent<Rigidbody>();

            if (controller != null)
                controller.enabled = false;

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.Sleep();
            }

            player.position = respawnPos;

            if (controller != null)
                controller.enabled = true;
        }
        else
        {
            Debug.LogWarning("No checkpoint reached yet. Respawn unavailable.");
        }
    }
}
