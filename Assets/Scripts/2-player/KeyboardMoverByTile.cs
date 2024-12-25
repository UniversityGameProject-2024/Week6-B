using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class KeyboardMoverByTile : KeyboardMover
{
    [SerializeField] Tilemap tilemap = null;
    [SerializeField] AllowedTiles allowedTiles = null;
    [Tooltip("Maximum number of iterations before BFS algorithm gives up on finding a path")]
    [SerializeField] int maxIterations = 1000;

    bool playerPlaced = false;
    bool playerPositionValidated = false;

    private TilemapGraph tilemapGraph = null;

    void Start()
    {
        tilemapGraph = new TilemapGraph(tilemap, allowedTiles.Get());
    }

    void Update()
    {
        // Check if the player has been placed on a tile, if not, place the player
        if (!playerPlaced)
        {
            PlacePlayerOnRandomAllowedTile();
            playerPlaced = true;
        }

        // Validate player position once they are placed
        if (playerPlaced && !playerPositionValidated)
        {
            ValidatePlayerPosition();
            playerPositionValidated = true;
        }

        Vector3 newPosition = NewPosition();
        TileBase tileOnNewPosition = TileOnPosition(newPosition);
        if (allowedTiles.Contains(tileOnNewPosition))
        {
            transform.position = newPosition;
        }
        else
        {
            Debug.Log("You cannot walk on " + tileOnNewPosition + "!");
        }
    }

    private void PlacePlayerOnRandomAllowedTile()
    {
        // Find all positions of allowed tiles
        List<Vector3Int> allowedTilePositions = new List<Vector3Int>();
        BoundsInt bounds = tilemap.cellBounds;
        foreach (var position in bounds.allPositionsWithin)
        {
            if (allowedTiles.Contains(tilemap.GetTile(position)))
            {
                allowedTilePositions.Add(position);
            }
        }

        // Select a random position from the allowed tile positions
        Vector3Int randomPosition = allowedTilePositions[Random.Range(0, allowedTilePositions.Count)];
        // Convert the position to world coordinates
        Vector3 worldPosition = tilemap.CellToWorld(randomPosition);
        // Set the player's position to the random position
        transform.position = worldPosition;
    }

    private void ValidatePlayerPosition()
    {
        Vector3Int playerPosition = tilemap.WorldToCell(transform.position);
        List<Vector3Int> reachableTiles = BFS.GetReachableTiles(tilemapGraph, playerPosition, maxIterations);

        if (reachableTiles.Count < 100)
        {
            // Player is not in a valid position, reposition them
            Debug.LogWarning("Player is in a position with less than 100 reachable tiles. Repositioning...");
            PlacePlayerOnRandomAllowedTile();
            ValidatePlayerPosition(); // Validate again recursively
        }
        else
        {
            Debug.Log("Player positioned in a valid location.");
        }
    }

    private TileBase TileOnPosition(Vector3 worldPosition)
    {
        Vector3Int cellPosition = tilemap.WorldToCell(worldPosition);
        return tilemap.GetTile(cellPosition);
    }
}
