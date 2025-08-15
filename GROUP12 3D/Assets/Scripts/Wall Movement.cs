using UnityEngine;

public class WallMovement : MonoBehaviour
{
    public Transform wall1;      
    public Transform wall2;      
    public float moveDistance = 2f; 
    public float moveSpeed = 2f;    

    private Vector3 wall1StartPos;
    private Vector3 wall2StartPos;
    private bool movingIn = true;

    void Start()
    {
        
        wall1StartPos = wall1.position;
        wall2StartPos = wall2.position;
    }

    void Update()
    {
        float step = moveSpeed * Time.deltaTime;

        if (movingIn)
        {
            
            wall1.position = Vector3.MoveTowards(wall1.position, wall1StartPos + Vector3.right * moveDistance, step);
            wall2.position = Vector3.MoveTowards(wall2.position, wall2StartPos + Vector3.left * moveDistance, step);

           
            if (Vector3.Distance(wall1.position, wall1StartPos + Vector3.right * moveDistance) < 0.01f)
                movingIn = false;
        }
        else
        {
            
            wall1.position = Vector3.MoveTowards(wall1.position, wall1StartPos, step);
            wall2.position = Vector3.MoveTowards(wall2.position, wall2StartPos, step);

           
            if (Vector3.Distance(wall1.position, wall1StartPos) < 0.01f)
                movingIn = true;
        }
    }
}
