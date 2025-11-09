 using UnityEngine;

public class TrapTile : MonoBehaviour
{
    public int tileIndex;
    public TrapTileController controller;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == controller.player)
        {
            controller.OnTileStepped(tileIndex);
        }
    }
}