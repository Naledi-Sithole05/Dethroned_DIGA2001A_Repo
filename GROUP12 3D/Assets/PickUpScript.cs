 using UnityEngine;

public class CrownCollection : MonoBehaviour
{
    [Header("Player References")]
    public GameObject playerCapsule;
    public Transform holdPosition;
    
    [Header("Camera Settings")]
    public Camera mainCamera;
    public Camera pickupCamera;
    
    [Header("Pickup Settings")]
    [SerializeField] private float pickUpRange = 3f;
    [SerializeField] private float rotationSpeed = 2f;
    [SerializeField] private float throwForce = 300f;
    
    private GameObject heldCrown;
    private Rigidbody heldCrownRb;
    private bool canDrop = true;
    private int holdLayer;

    void Start()
    {
        holdLayer = LayerMask.NameToLayer("holdLayer");
        pickupCamera.enabled = false;
    }

    void Update()
    {
        HandlePickupInput();
        HandleHeldObject();
    }

    void HandlePickupInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldCrown == null)
            {
                TryPickUpCrown();
            }
            else if (canDrop)
            {
                StopClipping();
                DropCrown();
            }
        }
    }

    void HandleHeldObject()
    {
        if (heldCrown != null)
        {
            HoldCrownInPosition();
            RotateCrown();
            
            if (Input.GetKeyDown(KeyCode.G) && canDrop) // Changed to G key
            {
                StopClipping();
                ThrowCrown();
            }
        }
    }

    void TryPickUpCrown()
    {
        RaycastHit hit;
        if (Physics.Raycast(mainCamera.transform.position, mainCamera.transform.forward, out hit, pickUpRange))
        {
            if (hit.transform.CompareTag("Crown"))
            {
                PickUpCrown(hit.transform.gameObject);
                SwitchToPickupCamera();
            }
        }
    }

    void PickUpCrown(GameObject crown)
    {
        heldCrown = crown;
        heldCrownRb = crown.GetComponent<Rigidbody>();
        
        if (heldCrownRb != null)
        {
            heldCrownRb.isKinematic = true;
            heldCrown.transform.parent = holdPosition;
            heldCrown.layer = holdLayer;
            Physics.IgnoreCollision(heldCrown.GetComponent<Collider>(), 
                                  playerCapsule.GetComponent<Collider>(), true);
        }
    }

    void DropCrown()
    {
        if (heldCrownRb != null)
        {
            Physics.IgnoreCollision(heldCrown.GetComponent<Collider>(), 
                                  playerCapsule.GetComponent<Collider>(), false);
            
            heldCrown.layer = 0;
            heldCrownRb.isKinematic = false;
            heldCrown.transform.parent = null;
            heldCrown = null;
            
            SwitchToMainCamera();
        }
    }

    void ThrowCrown()
    {
        Physics.IgnoreCollision(heldCrown.GetComponent<Collider>(), 
                              playerCapsule.GetComponent<Collider>(), false);
        
        heldCrown.layer = 0;
        heldCrownRb.isKinematic = false;
        heldCrown.transform.parent = null;
        heldCrownRb.AddForce(mainCamera.transform.forward * throwForce);
        heldCrown = null;
        
        SwitchToMainCamera();
    }

    void HoldCrownInPosition()
    {
        heldCrown.transform.position = holdPosition.position;
    }

    void RotateCrown()
    {
        if (Input.GetKey(KeyCode.R))
        {
            canDrop = false;
            
            float xRotation = Input.GetAxis("Mouse X") * rotationSpeed;
            float yRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
            
            heldCrown.transform.Rotate(Vector3.up, -xRotation, Space.World);
            heldCrown.transform.Rotate(Vector3.right, yRotation, Space.World);
        }
        else
        {
            canDrop = true;
        }
    }

    void StopClipping()
    {
        float distance = Vector3.Distance(heldCrown.transform.position, mainCamera.transform.position);
        RaycastHit[] hits = Physics.RaycastAll(mainCamera.transform.position, 
                                             mainCamera.transform.forward, 
                                             distance);
        
        if (hits.Length > 1)
        {
            heldCrown.transform.position = mainCamera.transform.position + 
                                        new Vector3(0f, -0.3f, 0f);
        }
    }

    void SwitchToPickupCamera()
    {
        if (pickupCamera != null)
        {
            mainCamera.enabled = false;
            pickupCamera.enabled = true;
        }
    }

    void SwitchToMainCamera()
    {
        if (pickupCamera != null)
        {
            pickupCamera.enabled = false;
            mainCamera.enabled = true;
        }
    }
}