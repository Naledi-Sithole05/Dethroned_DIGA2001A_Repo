  using UnityEngine;

public class TrapTile : MonoBehaviour
{
    public int tileIndex;
    public TrapTileController controller;
    
    void Start()
    {
        // Ensure there's a collider
        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
            GetComponent<BoxCollider>().isTrigger = true;
        }
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (controller != null && other.gameObject == controller.player)
        {
            Debug.Log($"Player stepped on tile {tileIndex}");
            controller.OnTileStepped(tileIndex);
        }
    }
}