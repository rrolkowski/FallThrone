using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The GridManager class manages a tile-based grid where each tile has properties
/// such as movement cost and whether it's an obstacle. This class initializes the grid
/// with nodes, resets nodes to default states, and provides pathfinding-related functions.
/// It integrates with the PathFinder class to support pathfinding in a game environment.
/// </summary>
public class GridManager : MonoBehaviour
{
    [Header("Script Reference")]
    [SerializeField] PathFinder _pathFinder;

    [Header("Tilemap")]
    public Tilemap tilemap;

    [Header("Tiles")]
    public TileBase pathTile;       // Path tile (brown) with lowest movement cost
    public TileBase terrainTile;    // Terrain tile (black) with a higher movement cost
    public TileBase obstacleTile;   // Obstacle tile (red), blocking movement

    private Dictionary<Vector3Int, TileNode> _nodes; // Stores each tile’s node data (position, cost, and obstacle status)


    void Awake()
    {
        InitializeGrid();
    }

    // Initializes the grid by creating nodes for each tile
    public void InitializeGrid()
    {
        _nodes = new Dictionary<Vector3Int, TileNode>();

        foreach (Vector3Int pos in tilemap.cellBounds.allPositionsWithin) // Iterates through each tile position in the tilemap
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile != null)
            {
                bool isObstacle = false;
                int movementCost;

                // Assigns movement cost and obstacle status based on tile type
                if (tile == pathTile)
                {
                    movementCost = 1;  // Path tile has the lowest cost
                }
                else if (tile == terrainTile)
                {
                    movementCost = 100; // Terrain tile has a higher cost
                }
                else if (tile == obstacleTile)
                {
                    movementCost = int.MaxValue;  // Obstacle tile blocks movement
                    isObstacle = true;
                }
                else
                {
                    movementCost = int.MaxValue;  // Defaults to obstacle if tile type is unknown
                    isObstacle = true;
                }

                _nodes[pos] = new TileNode(pos, isObstacle, movementCost); // Creates and stores a TileNode for each tile
            }
        }
    }

    // Resets movement costs for each node to default based on tile type
    public void ResetNodes()
    {
        foreach (var node in _nodes.Values)
        {
            if (node.isObstacle)
            {
                node.movementCost = int.MaxValue;
            }
            else if (tilemap.GetTile(node.position) == pathTile)
            {
                node.movementCost = 1; // Domyœlny koszt dla kafelków œcie¿ki
            }
            else if (tilemap.GetTile(node.position) == terrainTile)
            {
                node.movementCost = 100; // Wy¿szy koszt dla kafelków terenu
            }
        }
    }

    // Returns the node at a specific position if it exists in the grid
    public TileNode GetNode(Vector3Int position)
    {
        _nodes.TryGetValue(position, out TileNode node);
        return node;
    }

    // Returns a list of neighboring nodes in the four directions (up, down, left, right)
    public List<TileNode> GetNeighbors(TileNode node)
    {
        List<TileNode> neighbors = new List<TileNode>();
        Vector3Int[] directions = { Vector3Int.up, Vector3Int.down, Vector3Int.left, Vector3Int.right };

        foreach (var dir in directions)
        {
            Vector3Int neighborPos = node.position + dir;
            if (_nodes.ContainsKey(neighborPos))
            {
                neighbors.Add(_nodes[neighborPos]);
            }
        }
        return neighbors;
    }

    // Finds and returns the closest node that is walkable from a given position
    public TileNode GetClosestWalkableNode(Vector3Int position)
    {
        TileNode startNode = GetNode(position);
        if (startNode == null || !startNode.isObstacle)
        {
            return startNode;
        }

        // Use a priority queue and visited set to explore nodes efficiently
        PriorityQueue<TileNode> queue = new PriorityQueue<TileNode>();
        HashSet<TileNode> visited = new HashSet<TileNode>();

        // Start with the initial node
        queue.Enqueue(startNode, 0);
        visited.Add(startNode);

        // Explore nodes until a walkable one is found
        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            if (!current.isObstacle)
            {
                return current;
            }

            // Enqueue neighbors for exploration if they haven’t been visited
            foreach (TileNode neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);

                    // Calculacte priority as distance to starting position
                    float priority = _pathFinder.Heuristic(neighbor.position, position);
                    queue.Enqueue(neighbor, priority);
                }
            }
        }

        return null; // No walkable node found within the search area
    }

    // Finds the closest path tile node from the given start position
    public TileNode GetClosestPathTileNode(Vector3Int startPosition)
    {
        TileNode startNode = GetNode(startPosition);
        PriorityQueue<TileNode> queue = new PriorityQueue<TileNode>();
        HashSet<TileNode> visited = new HashSet<TileNode>();
        float closestDistance = float.MaxValue;
        TileNode closestPathTile = null;

        // Initialize search with the starting node if it’s not an obstacle
        if (startNode != null && !startNode.isObstacle)
        {
            queue.Enqueue(startNode, 0);
            visited.Add(startNode);
        }

        // Process nodes in the queue
        while (queue.Count > 0)
        {
            TileNode current = queue.Dequeue();

            // Calculate the heuristic distance  from the starting position to the current node’s positio to help prioritize closer nodes.
            if (tilemap.GetTile(current.position) == pathTile && !current.isObstacle)
            {
                float distance = _pathFinder.Heuristic(startPosition, current.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestPathTile = current;
                }
                break; // Optionally exit the loop upon finding the first path tile
            }

            // Explore neighboring nodes if they haven’t been visited
            foreach (TileNode neighbor in GetNeighbors(current))
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);

                    // Calculacte priority as distance to starting position
                    float hPriority = _pathFinder.Heuristic(neighbor.position, startPosition);
                    queue.Enqueue(neighbor, hPriority);
                }
            }
        }

        return closestPathTile;
    }

}