 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTileController : MonoBehaviour
{
    [System.Serializable]
    public class TileData
    {
        public GameObject tileObject;
        public Material defaultMaterial;
        public Material activeMaterial;
        public Material correctMaterial;
        public Material wrongMaterial;
        [HideInInspector] public bool isActiveInPattern = false;
        [HideInInspector] public bool isSteppedOn = false;
    }

    [Header("Tile Settings")]
    public List<TileData> tiles = new List<TileData>();
    public float patternDisplayTime = 3f;
    public float colorChangeDuration = 0.5f;

    [Header("Player Settings")]
    public GameObject player;
    public Transform startPosition;

    [Header("Popup System")]
    public PopupManager popupManager;
    public bool showPopupOnEnter = true;

    [Header("Pattern Settings")]
    public int tilesPerRow = 3; // Number of tiles in each row
    public int startingRows = 1; // How many rows to use for the starting pattern

    private List<int> currentPattern = new List<int>();
    private int currentStep = 0;
    private bool patternActive = false;
    private bool playerInTrigger = false;
    private bool patternDisplaying = false;
    private bool hasShownPopup = false;
    private bool popupClosed = false;

    void Start()
    {
        InitializeTiles();
        
        // Subscribe to popup closed event
        if (popupManager != null)
        {
            popupManager.OnPopupClosed += OnPopupClosed;
        }
    }

    void InitializeTiles()
    {
        // Initialize tiles and add individual tile components
        for (int i = 0; i < tiles.Count; i++)
        {
            TileData tile = tiles[i];
            if (tile.tileObject != null && tile.defaultMaterial != null)
            {
                tile.tileObject.GetComponent<Renderer>().material = tile.defaultMaterial;
                
                // Add individual tile component if not present
                TrapTile trapTile = tile.tileObject.GetComponent<TrapTile>();
                if (trapTile == null)
                {
                    trapTile = tile.tileObject.AddComponent<TrapTile>();
                }
                trapTile.tileIndex = i;
                trapTile.controller = this;
            }
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from event to prevent memory leaks
        if (popupManager != null)
        {
            popupManager.OnPopupClosed -= OnPopupClosed;
        }
    }

    void Update()
    {
        // Check for player input when pattern is active and player is in trigger
        // Now SPACE only works after popup is closed
        if (!patternActive && !patternDisplaying && playerInTrigger && popupClosed && Input.GetKeyDown(KeyCode.Space))
        {
            StartPatternSequence();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInTrigger = true;
            
            if (!patternActive && !patternDisplaying && showPopupOnEnter && !hasShownPopup)
            {
                ShowInstructionPopup();
                hasShownPopup = true;
                popupClosed = false; // Reset popup closed state
            }
            else if (!patternActive && !patternDisplaying && popupClosed)
            {
                Debug.Log("Press SPACE to start the pattern sequence");
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInTrigger = false;
        }
    }

    void OnPopupClosed()
    {
        popupClosed = true;
        Debug.Log("Popup closed. Press SPACE to start the pattern when you're ready.");
    }

    void ShowInstructionPopup()
    {
        if (popupManager != null)
        {
            popupManager.ShowPatternMessage();
        }
        else
        {
            // Fallback to console message
            Debug.Log("Watch carefully! The tiles will light up in a pattern. Remember the sequence and step on them in the correct order. Press SPACE to start when you're ready.");
            popupClosed = true; // If no popup manager, allow immediate start
        }
    }

    public void StartPatternSequence()
    {
        if (patternActive || patternDisplaying) return;

        patternDisplaying = true;
        popupClosed = false; // Reset for next time
        GenerateRandomPattern();
        StartCoroutine(DisplayPattern());
    }

    void GenerateRandomPattern()
    {
        currentPattern.Clear();
        currentStep = 0;

        // Reset all tiles
        foreach (TileData tile in tiles)
        {
            tile.isActiveInPattern = false;
        }

        // Calculate how many starting tiles we have
        int totalStartingTiles = tilesPerRow * startingRows;
        
        // Generate random pattern (3-5 tiles) only from starting rows
        int patternLength = Random.Range(3, 6);
        for (int i = 0; i < patternLength; i++)
        {
            int randomTileIndex;
            do
            {
                // Only choose from starting tiles
                randomTileIndex = Random.Range(0, totalStartingTiles);
            } while (currentPattern.Contains(randomTileIndex)); // Ensure no duplicates

            currentPattern.Add(randomTileIndex);
            tiles[randomTileIndex].isActiveInPattern = true;
        }

        Debug.Log($"Pattern generated with {patternLength} tiles from first {startingRows} row(s)");
    }

    IEnumerator DisplayPattern()
    {
        // Disable player movement during pattern display
        FPController playerMovement = player.GetComponent<FPController>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        Debug.Log("Watch the pattern carefully!");

        // Wait a moment before starting pattern
        yield return new WaitForSeconds(1f);

        // Display the pattern with longer duration
        foreach (int tileIndex in currentPattern)
        {
            yield return StartCoroutine(ChangeTileColor(tileIndex, tiles[tileIndex].activeMaterial));
            yield return new WaitForSeconds(0.8f); // Longer pause between tiles
        }

        // Brief pause after showing full pattern
        yield return new WaitForSeconds(0.5f);

        // Enable player movement after pattern display
        if (playerMovement != null)
            playerMovement.enabled = true;

        patternDisplaying = false;
        patternActive = true;

        Debug.Log("Pattern display complete. Step on the tiles in the correct order!");
    }

    IEnumerator ChangeTileColor(int tileIndex, Material targetMaterial)
    {
        TileData tile = tiles[tileIndex];
        Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();
        
        if (tileRenderer != null && targetMaterial != null)
        {
            tileRenderer.material = targetMaterial;
            yield return new WaitForSeconds(colorChangeDuration);
            
            // Only change back to default if we're not in the middle of displaying pattern
            if (!patternDisplaying)
            {
                tileRenderer.material = tile.defaultMaterial;
            }
        }
    }

    // This method should be called when player steps on a tile
    public void OnTileStepped(int tileIndex)
    {
        if (!patternActive) return;

        TileData steppedTile = tiles[tileIndex];

        // Check if this is the correct tile in sequence
        if (tileIndex == currentPattern[currentStep])
        {
            // Correct tile
            StartCoroutine(FlashTileColor(tileIndex, steppedTile.correctMaterial, true));
            currentStep++;

            // Check if pattern is complete
            if (currentStep >= currentPattern.Count)
            {
                PatternCompleted();
            }
            else
            {
                Debug.Log($"Correct! Next tile: {currentStep + 1}/{currentPattern.Count}");
            }
        }
        else
        {
            // Wrong tile
            StartCoroutine(FlashTileColor(tileIndex, steppedTile.wrongMaterial, false));
            ResetPattern();
        }
    }

    IEnumerator FlashTileColor(int tileIndex, Material flashMaterial, bool isCorrect)
    {
        TileData tile = tiles[tileIndex];
        Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();
        
        if (tileRenderer != null && flashMaterial != null)
        {
            Material originalMaterial = tileRenderer.material;
            tileRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.5f);
            
            // Only revert to default if it's not a correct step in an active pattern
            if (!isCorrect || !patternActive)
            {
                tileRenderer.material = originalMaterial;
            }
        }
    }

    void PatternCompleted()
    {
        Debug.Log("Pattern completed successfully! You can now cross safely.");
        patternActive = false;
        currentPattern.Clear();
        currentStep = 0;
        hasShownPopup = false; // Reset so popup shows again if player comes back

        // Optional: Make all tiles safe color for a moment
        StartCoroutine(SuccessFlash());

        // Reset all tiles
        foreach (TileData tile in tiles)
        {
            tile.isActiveInPattern = false;
        }
    }

    IEnumerator SuccessFlash()
    {
        // Flash all tiles green to indicate success
        List<Material> originalMaterials = new List<Material>();
        
        foreach (TileData tile in tiles)
        {
            Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();
            if (tileRenderer != null && tile.correctMaterial != null)
            {
                originalMaterials.Add(tileRenderer.material);
                tileRenderer.material = tile.correctMaterial;
            }
        }

        yield return new WaitForSeconds(1f);

        // Restore original materials
        for (int i = 0; i < tiles.Count; i++)
        {
            if (i < originalMaterials.Count && originalMaterials[i] != null)
            {
                tiles[i].tileObject.GetComponent<Renderer>().material = tiles[i].defaultMaterial;
            }
        }
    }

    void ResetPattern()
    {
        Debug.Log("Wrong tile! Resetting to start position.");
        
        // Reset player position
        StartCoroutine(ResetPlayerPosition());
        
        // Reset pattern state
        patternActive = false;
        currentPattern.Clear();
        currentStep = 0;
        hasShownPopup = false; // Reset so popup shows again
        popupClosed = false; // Reset popup state

        // Reset all tiles to default material
        foreach (TileData tile in tiles)
        {
            tile.isActiveInPattern = false;
            if (tile.tileObject != null && tile.defaultMaterial != null)
            {
                tile.tileObject.GetComponent<Renderer>().material = tile.defaultMaterial;
            }
        }
    }

    IEnumerator ResetPlayerPosition()
    {
        // Disable player movement during reset
        FPController playerMovement = player.GetComponent<FPController>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        // Wait a moment before resetting
        yield return new WaitForSeconds(0.5f);
        
        // Reset position
        player.transform.position = startPosition.position;
        
        // Reset velocity if using CharacterController
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            // This helps prevent the player from carrying momentum
            controller.enabled = false;
            yield return null;
            controller.enabled = true;
        }

        // Re-enable movement after a brief delay
        yield return new WaitForSeconds(0.5f);
        if (playerMovement != null)
            playerMovement.enabled = true;

        // Show popup again after reset
        if (showPopupOnEnter)
        {
            ShowInstructionPopup();
        }
        else
        {
            popupClosed = true;
            Debug.Log("Ready to try again! Press SPACE to start new pattern.");
        }
    }
}