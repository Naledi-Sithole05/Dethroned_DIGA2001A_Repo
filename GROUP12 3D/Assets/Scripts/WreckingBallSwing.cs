using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(HingeJoint))]
public class PhysicsWreckingBall : MonoBehaviour
{
    public Transform anchorPoint;      // Empty GameObject where the chain is attached
    public float swingForce = 50f;     // Force applied to swing the ball
    public float swingFrequency = 2f;  // How often to push (like oscillation)

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.mass = 20f;
        rb.linearDamping = 0f;   // replaces rb.drag
        rb.angularDamping = 0.05f; // replaces rb.angularDrag

        // Setup hinge joint
        HingeJoint joint = GetComponent<HingeJoint>();
        if (anchorPoint != null)
        {
            joint.connectedBody = anchorPoint.GetComponent<Rigidbody>();
            if (joint.connectedBody == null)
            {
                // If the anchor doesn’t have a Rigidbody, make one (kinematic so it doesn’t move)
                Rigidbody anchorRb = anchorPoint.gameObject.AddComponent<Rigidbody>();
                anchorRb.isKinematic = true;
                joint.connectedBody = anchorRb;
            }
        }
    }

    void FixedUpdate()
    {
        float torque = Mathf.Sin(Time.time * swingFrequency) * swingForce;
        rb.AddTorque(Vector3.forward * torque); // rotate around Z axis
    }

}
