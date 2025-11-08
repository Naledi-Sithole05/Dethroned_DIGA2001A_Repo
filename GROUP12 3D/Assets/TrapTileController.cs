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

    [Header("Instructions")]
    public bool showInstructions = true;
    public string instructionMessage = "Watch the pattern! Press SPACE to start.";

    private List<int> currentPattern = new List<int>();
    private int currentStep = 0;
    private bool patternActive = false;
    private bool playerInTrigger = false;
    private bool patternDisplaying = false;
    private bool hasShownInstructions = false;

    void Start()
    {
        // Initialize tiles
        foreach (TileData tile in tiles)
        {
            if (tile.tileObject != null && tile.defaultMaterial != null)
            {
                tile.tileObject.GetComponent<Renderer>().material = tile.defaultMaterial;
            }
        }
    }

    void Update()
    {
        // Check for player input when pattern is active and player is in trigger
        if (!patternActive && !patternDisplaying && playerInTrigger && Input.GetKeyDown(KeyCode.Space))
        {
            StartPatternSequence();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            playerInTrigger = true;
            
            if (!patternActive && !patternDisplaying && showInstructions && !hasShownInstructions)
            {
                ShowInstructions();
                hasShownInstructions = true;
            }
            else if (!patternActive && !patternDisplaying)
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

    void ShowInstructions()
    {
        Debug.Log("Watch carefully! The tiles will light up in a pattern. Remember the sequence and step on them in the correct order. Press SPACE to start when you're ready.");
        
        // You can also show on-screen text here if you want
        // StartCoroutine(ShowTemporaryMessage(instructionMessage, 5f));
    }

    public void StartPatternSequence()
    {
        if (patternActive || patternDisplaying) return;

        patternDisplaying = true;
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

        // Generate random pattern (3-5 tiles)
        int patternLength = Random.Range(3, 6);
        for (int i = 0; i < patternLength; i++)
        {
            int randomTileIndex;
            do
            {
                randomTileIndex = Random.Range(0, tiles.Count);
            } while (currentPattern.Contains(randomTileIndex)); // Ensure no duplicates

            currentPattern.Add(randomTileIndex);
            tiles[randomTileIndex].isActiveInPattern = true;
        }

        Debug.Log($"Pattern generated with {patternLength} tiles");
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

        // Display the pattern
        foreach (int tileIndex in currentPattern)
        {
            yield return StartCoroutine(ChangeTileColor(tileIndex, tiles[tileIndex].activeMaterial));
            yield return new WaitForSeconds(0.5f); // Pause between tiles
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
            tileRenderer.material = tile.defaultMaterial;
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
            StartCoroutine(FlashTileColor(tileIndex, steppedTile.correctMaterial));
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
            StartCoroutine(FlashTileColor(tileIndex, steppedTile.wrongMaterial));
            ResetPattern();
        }
    }

    IEnumerator FlashTileColor(int tileIndex, Material flashMaterial)
    {
        TileData tile = tiles[tileIndex];
        Renderer tileRenderer = tile.tileObject.GetComponent<Renderer>();
        
        if (tileRenderer != null && flashMaterial != null)
        {
            Material originalMaterial = tileRenderer.material;
            tileRenderer.material = flashMaterial;
            yield return new WaitForSeconds(0.5f);
            tileRenderer.material = originalMaterial;
        }
    }

    void PatternCompleted()
    {
        Debug.Log("Pattern completed successfully! You can now cross safely.");
        patternActive = false;
        currentPattern.Clear();
        currentStep = 0;
        hasShownInstructions = false; // Reset so instructions show again if player comes back

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
        hasShownInstructions = false; // Reset so instructions show again

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

        // Show instructions again after reset
        if (showInstructions)
        {
            ShowInstructions();
        }
        else
        {
            Debug.Log("Ready to try again! Press SPACE to start new pattern.");
        }
    }

    // Optional: Method to show temporary on-screen message
    private IEnumerator ShowTemporaryMessage(string message, float duration)
    {
        // You can implement GUI drawing here if needed
        Debug.Log(message);
        yield return new WaitForSeconds(duration);
        // Hide message logic here
    }
}