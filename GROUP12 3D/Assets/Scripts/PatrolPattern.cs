using UnityEngine;

[ExecuteAlways]
public class PatrolPattern : MonoBehaviour
{
    private const float GizmoRadius = 0.3f;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        for (int i = 0; i < transform.childCount; i++)
        {
            int j = GetNextIndex(i);
            Gizmos.DrawSphere(GetWaypoint(i), GizmoRadius);
            Gizmos.DrawLine(GetWaypoint(i), GetWaypoint(j));
        }
    }

    public int GetNextIndex(int i)
    {
        return (i + 1) % transform.childCount;
    }

    public Vector3 GetWaypoint(int i)
    {
        return transform.GetChild(i).position;
    }
}
