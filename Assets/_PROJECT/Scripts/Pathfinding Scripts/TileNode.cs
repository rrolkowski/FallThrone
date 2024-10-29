using UnityEngine;

/// <summary>
/// Represents a single tile in the grid with properties such as position, movement cost, 
/// and whether it’s an obstacle. Each TileNode can also store a reference to its parent node,
/// which is used during pathfinding to trace back the path.
/// </summary>
public class TileNode
{
    public Vector3Int position; // The grid position of this tile
    public bool isObstacle;     // Indicates if the tile is an obstacle(not walkable)
    public int movementCost;    // Cost to move through this tile; higher costs make paths less preferable
    public TileNode parent;     // Reference to the parent tile in the path, used for retracing paths

    // Constructor to initialize a tile node with its position, obstacle status, and movement cost
    public TileNode(Vector3Int position, bool isObstacle, int movementCost)
    {
        this.position = position;
        this.isObstacle = isObstacle;
        this.movementCost = movementCost;
        this.parent = null;     // Initially, no parent is assigned
    }
}