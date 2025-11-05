 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTile : MonoBehaviour
{
    public int tileIndex;
    public TrapTileController controller;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && controller != null)
        {
            controller.OnTileStepped(tileIndex);
        }
    }
}
