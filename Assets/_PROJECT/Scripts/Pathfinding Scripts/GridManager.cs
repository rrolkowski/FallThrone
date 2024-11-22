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

	private Dictionary<Vector3Int, TileNode> _nodes; // Stores each tile’s node data (position, cost, and obstacle status)
	private List<Vector3Int> _spawnTilePositions = new List<Vector3Int>(); // Stores positions of spawn tiles  
    private Vector3Int _endTilePosition;
    void Awake()
	{
		InitializeGrid();
  //      Color tilemapColor = tilemap.color;
  //      tilemapColor.a = 0;
		//tilemap.color = tilemapColor;
    }

	// Initializes the grid by creating nodes for each tile
	public void InitializeGrid()
	{
        _nodes = new Dictionary<Vector3Int, TileNode>();
        _spawnTilePositions.Clear();
        _endTilePosition = Vector3Int.zero;

        // Iteracja przez wszystkie dzieci Tilemapy
        foreach (Transform child in tilemap.transform)
        {
            // Pobierz pozycjê w gridzie Tilemapy
            Vector3Int gridPosition = tilemap.WorldToCell(child.position);

            // Rozpoznaj typ obiektu na podstawie tagu
            int movementCost = int.MaxValue;
            bool isObstacle = true;

            if (child.CompareTag("Path"))
            {
                movementCost = 1;
                isObstacle = false;
            }
            else if (child.CompareTag("Terrain"))
            {
                movementCost = 50;
                isObstacle = false;
            }
            else if (child.CompareTag("Obstacle"))
            {
                movementCost = int.MaxValue;
                isObstacle = true;
            }
            else if (child.CompareTag("Spawn"))
            {
                movementCost = 1;
                isObstacle = false;
                _spawnTilePositions.Add(gridPosition);
            }
			else if (child.CompareTag("End"))
			{
				movementCost = 1;
				isObstacle = false;
				_endTilePosition = gridPosition;
			}

            // Tworzymy wêze³ TileNode
            _nodes[gridPosition] = new TileNode(gridPosition, isObstacle, movementCost);
        }

        Debug.Log($"Grid initialized with {_nodes.Values} nodes.");
    }

	// Provides the list of spawn tile positions
	public List<Vector3Int> GetSpawnTilePositions()
	{
		return _spawnTilePositions;
	}
	public Vector3Int GetEndTilePosition()
	{
		return _endTilePosition;
	}

    // Resets movement costs for each node to default based on tile type
    public void ResetNodes()
	{
        foreach (var node in _nodes.Values)
        {
            // Reset kosztu w zale¿noœci od tagu GameObjectu w gridzie
            Transform child = GetChildAtGridPosition(node.position);
            if (child == null) continue;

            if (child.CompareTag("Path"))
            {
                node.movementCost = 1;
                node.isObstacle = false;
            }
            else if (child.CompareTag("Terrain"))
            {
                node.movementCost = 50;
                node.isObstacle = false;
            }
            else if (child.CompareTag("Obstacle"))
            {
                node.movementCost = int.MaxValue;
                node.isObstacle = true;
            }
        }
    }
    public Transform GetChildAtGridPosition(Vector3Int gridPosition)
    {
        foreach (Transform child in tilemap.transform)
        {
            if (tilemap.WorldToCell(child.position) == gridPosition)
                return child;
        }
        return null;
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

	//Finds the closest path tile node from the given start position
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

            Transform childAtPosition = GetChildAtGridPosition(current.position);
			// Calculate the heuristic distance from the starting position to the current node’s position to help prioritize closer nodes.
			if (childAtPosition != null && childAtPosition.CompareTag("Path") && !current.isObstacle)
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
