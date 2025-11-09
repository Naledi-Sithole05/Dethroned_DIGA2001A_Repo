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
    }

    [Header("Tile Settings")]
    public List<TileData> tiles = new List<TileData>();
    public float colorChangeDuration = 0.5f;

    [Header("Player Settings")]
    public GameObject player;
    public Transform startPosition;

    [Header("Popup System")]
    public PopupManager popupManager;
    public bool showPopupOnEnter = true;

    [Header("Pattern Settings")]
    public int tilesPerRow = 3;
    public int patternLength = 4;

    private List<int> currentPattern = new List<int>();
    private int currentStep = 0;
    private bool patternActive = false;
    private bool playerInTrigger = false;
    private bool patternDisplaying = false;
    private bool hasShownPopup = false;
    private bool popupClosed = false;
    private Coroutine displayPatternCoroutine;

    void Start()
    {
        InitializeTiles();
        
        // Subscribe to popup events
        if (popupManager != null)
        {
            popupManager.OnPopupClosed += OnPopupClosed;
        }
    }

    void InitializeTiles()
    {
        Debug.Log($"Initializing {tiles.Count} tiles");

        for (int i = 0; i < tiles.Count; i++)
        {
            TileData tile = tiles[i];
            if (tile.tileObject != null)
            {
                // Set default material
                if (tile.defaultMaterial != null)
                {
                    tile.tileObject.GetComponent<Renderer>().material = tile.defaultMaterial;
                }

                // Add or get TrapTile component
                TrapTile trapTile = tile.tileObject.GetComponent<TrapTile>();
                if (trapTile == null)
                {
                    trapTile = tile.tileObject.AddComponent<TrapTile>();
                }
                trapTile.tileIndex = i;
                trapTile.controller = this;

                // Ensure collider is set up
                Collider collider = tile.tileObject.GetComponent<Collider>();
                if (collider == null)
                {
                    collider = tile.tileObject.AddComponent<BoxCollider>();
                }
                collider.isTrigger = true;

                Debug.Log($"Initialized tile {i}: {tile.tileObject.name}");
            }
        }
    }

    void Update()
    {
        // Only allow SPACE to start pattern after popup is closed
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
            
            // Only show popup if it hasn't been shown before AND we want to show it on enter
            if (!patternActive && !patternDisplaying && showPopupOnEnter && !hasShownPopup)
            {
                ShowInstructionPopup();
                hasShownPopup = true;
                popupClosed = false;
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

        Debug.Log("Starting pattern sequence...");
        patternDisplaying = true;
        popupClosed = false; // Reset for next time
        GenerateProgressivePattern();
        displayPatternCoroutine = StartCoroutine(DisplayPatternCoroutine());
    }

    void GenerateProgressivePattern()
    {
        currentPattern.Clear();
        currentStep = 0;

        // Reset all tiles
        foreach (TileData tile in tiles)
        {
            tile.isActiveInPattern = false;
        }

        Debug.Log($"Generating progressive pattern with {patternLength} tiles");

        // Start from first row and progress forward
        int currentRow = 0;
        List<int> availableTiles = new List<int>();

        for (int i = 0; i < patternLength; i++)
        {
            // Get available tiles for current row
            availableTiles.Clear();
            int rowStart = currentRow * tilesPerRow;
            int rowEnd = Mathf.Min((currentRow + 1) * tilesPerRow, tiles.Count);

            for (int j = rowStart; j < rowEnd; j++)
            {
                availableTiles.Add(j);
            }

            // If no tiles available in current row, move to next row
            if (availableTiles.Count == 0)
            {
                currentRow++;
                continue;
            }

            // Pick a random tile from current row
            int randomIndex = Random.Range(0, availableTiles.Count);
            int selectedTile = availableTiles[randomIndex];

            currentPattern.Add(selectedTile);
            tiles[selectedTile].isActiveInPattern = true;

            Debug.Log($"Pattern step {i}: Tile {selectedTile} (Row {currentRow})");

            // Move to next row for next step (can be same row or next row randomly)
            if (Random.Range(0, 2) == 1) // 50% chance to move to next row
            {
                currentRow++;
            }

            // If we've run out of tiles, break
            if (currentRow * tilesPerRow >= tiles.Count)
            {
                break;
            }
        }

        Debug.Log($"Generated pattern with {currentPattern.Count} tiles");
    }

    IEnumerator DisplayPatternCoroutine()
    {
        Debug.Log("Displaying pattern...");

        // Disable player movement during pattern display
        FPController playerMovement = player.GetComponent<FPController>();
        if (playerMovement != null)
            playerMovement.enabled = false;

        yield return new WaitForSeconds(1f);

        // Display each tile in the pattern
        foreach (int tileIndex in currentPattern)
        {
            if (tileIndex < tiles.Count)
            {
                Debug.Log($"Showing tile {tileIndex}");
                yield return StartCoroutine(ChangeTileColorCoroutine(tileIndex, tiles[tileIndex].activeMaterial));
                yield return new WaitForSeconds(0.5f);
            }
        }

        yield return new WaitForSeconds(0.5f);

        // Re-enable player movement
        if (playerMovement != null)
            playerMovement.enabled = true;

        patternDisplaying = false;
        patternActive = true;
        Debug.Log("Pattern display complete! Step on the tiles in order.");
    }

    IEnumerator ChangeTileColorCoroutine(int tileIndex, Material targetMaterial)
    {
        if (tileIndex >= tiles.Count || tiles[tileIndex] == null) yield break;

        TileData tile = tiles[tileIndex];
        Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();

        if (tileRenderer != null && targetMaterial != null)
        {
            Material originalMaterial = tileRenderer.material;
            tileRenderer.material = targetMaterial;
            yield return new WaitForSeconds(colorChangeDuration);
            tileRenderer.material = tile.defaultMaterial;
        }
    }

    public void OnTileStepped(int tileIndex)
    {
        Debug.Log($"Tile stepped: {tileIndex}, Pattern active: {patternActive}, Current step: {currentStep}");

        // ANTI-CHEAT: If pattern is not active, reset player immediately
        if (!patternActive && !patternDisplaying)
        {
            Debug.Log($"Cheating detected! Player stepped on tile {tileIndex} without activating pattern.");
            StartCoroutine(FlashTileColorCoroutine(tileIndex, tiles[tileIndex].wrongMaterial));
            ResetToStart();
            return;
        }

        if (!patternActive)
        {
            Debug.Log("Pattern not active yet");
            return;
        }

        if (tileIndex == currentPattern[currentStep])
        {
            // Correct tile
            Debug.Log($"Correct! Step {currentStep + 1}/{currentPattern.Count}");
            StartCoroutine(FlashTileColorCoroutine(tileIndex, tiles[tileIndex].correctMaterial));
            currentStep++;

            if (currentStep >= currentPattern.Count)
            {
                PatternCompleted();
            }
        }
        else
        {
            // Wrong tile
            Debug.Log($"Wrong tile! Expected {currentPattern[currentStep]}, got {tileIndex}");
            StartCoroutine(FlashTileColorCoroutine(tileIndex, tiles[tileIndex].wrongMaterial));
            ResetToStart();
        }
    }

    IEnumerator FlashTileColorCoroutine(int tileIndex, Material flashMaterial)
    {
        if (tileIndex >= tiles.Count || tiles[tileIndex] == null) yield break;

        TileData tile = tiles[tileIndex];
        Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();

        if (tileRenderer != null && flashMaterial != null)
        {
            Material originalMaterial = tileRenderer.material;
            tileRenderer.material = flashMaterial;
            yield return new WaitForSeconds(1f);
            tileRenderer.material = originalMaterial;
        }
    }

    void PatternCompleted()
    {
        Debug.Log("Pattern completed successfully!");
        patternActive = false;
        // Don't reset hasShownPopup here - keep it true so popup doesn't show again
        
        // Flash all tiles green
        StartCoroutine(FlashAllTilesCoroutine(tiles[0].correctMaterial));
        
        currentPattern.Clear();
        currentStep = 0;
    }

    IEnumerator FlashAllTilesCoroutine(Material successMaterial)
    {
        List<Material> originalMaterials = new List<Material>();

        // Change all tiles to success color
        foreach (TileData tile in tiles)
        {
            Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();
            if (tileRenderer != null)
            {
                originalMaterials.Add(tileRenderer.material);
                tileRenderer.material = successMaterial;
            }
        }

        yield return new WaitForSeconds(2f);

        // Restore original materials
        for (int i = 0; i < tiles.Count; i++)
        {
            if (i < originalMaterials.Count)
            {
                tiles[i].tileObject.GetComponent<Renderer>().material = tiles[i].defaultMaterial;
            }
        }
    }

    void ResetToStart()
    {
        Debug.Log("Resetting player to start position");
        patternActive = false;
        patternDisplaying = false;
        currentPattern.Clear();
        currentStep = 0;
        
        // DON'T reset hasShownPopup - keep it true so popup doesn't show again
        popupClosed = true; // Set popup as closed so player can immediately restart

        // Reset all tile materials
        foreach (TileData tile in tiles)
        {
            tile.isActiveInPattern = false;
            if (tile.tileObject != null && tile.defaultMaterial != null)
            {
                tile.tileObject.GetComponent<Renderer>().material = tile.defaultMaterial;
            }
        }

        // Reset player position
        StartCoroutine(ResetPlayerCoroutine());
    }

    IEnumerator ResetPlayerCoroutine()
    {
        Debug.Log("Resetting player position...");
        
        // Disable player movement script during reset
        FPController playerMovement = player.GetComponent<FPController>();
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }

        yield return new WaitForSeconds(0.5f);

        // Reset position PROPERLY for CharacterController to prevent falling through ground
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            // Disable controller before changing position
            controller.enabled = false;
            
            // Set the transform position
            player.transform.position = startPosition.position;
            
            // Re-enable controller immediately
            controller.enabled = true;
            
            // Force a small movement to update internal controller state
            controller.Move(Vector3.down * 0.1f);
        }
        else
        {
            // Fallback if no CharacterController
            player.transform.position = startPosition.position;
        }

        // Reset player movement variables
        if (playerMovement != null)
        {
            playerMovement.ResetPlayer();
        }

        yield return new WaitForSeconds(0.3f);

        // Re-enable movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }

        Debug.Log("Player reset to start position. Press SPACE to try again.");
        popupClosed = true;
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (popupManager != null)
        {
            popupManager.OnPopupClosed -= OnPopupClosed;
        }
        
        if (displayPatternCoroutine != null)
        {
            StopCoroutine(displayPatternCoroutine);
        }
    }
}