using UnityEngine;

[RequireComponent(typeof(MeshCollider))]
public class WallMovement : MonoBehaviour
{
    public Transform wall1;
    public Transform wall2;
    public float moveDistance = 2f;
    public float moveSpeed = 2f;

    private Vector3 wall1StartPos;
    private Vector3 wall2StartPos;
    private bool movingIn = true;

    private MeshCollider wall1Collider;
    private MeshCollider wall2Collider;

    void Start()
    {
        // Store starting positions
        wall1StartPos = wall1.position;
        wall2StartPos = wall2.position;

        // Ensure Mesh Colliders exist on both walls
        wall1Collider = wall1.GetComponent<MeshCollider>();
        wall2Collider = wall2.GetComponent<MeshCollider>();

        if (wall1Collider == null)
            wall1Collider = wall1.gameObject.AddComponent<MeshCollider>();

        if (wall2Collider == null)
            wall2Collider = wall2.gameObject.AddComponent<MeshCollider>();

        // Ensure the mesh colliders are convex for physics interactions (optional)
        wall1Collider.convex = true;
        wall2Collider.convex = true;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        if (movingIn)
        {
            // Move walls inward
            wall1.position = Vector3.MoveTowards(wall1.position, wall1StartPos + Vector3.right * moveDistance, step);
            wall2.position = Vector3.MoveTowards(wall2.position, wall2StartPos + Vector3.left * moveDistance, step);

            if (Vector3.Distance(wall1.position, wall1StartPos + Vector3.right * moveDistance) < 0.01f)
                movingIn = false;
        }
        else
        {
            // Move walls outward
            wall1.position = Vector3.MoveTowards(wall1.position, wall1StartPos, step);
            wall2.position = Vector3.MoveTowards(wall2.position, wall2StartPos, step);

            if (Vector3.Distance(wall1.position, wall1StartPos) < 0.01f)
                movingIn = true;
        }

        // Optional: Update collider positions (if they are not children of the mesh)
        if (wall1Collider != null) wall1Collider.transform.position = wall1.position;
        if (wall2Collider != null) wall2Collider.transform.position = wall2.position;
    }
}
