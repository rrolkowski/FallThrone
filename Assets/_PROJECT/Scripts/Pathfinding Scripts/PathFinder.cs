using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The PathFinder class is responsible for calculating a path between two points in a grid managed by GridManager.
/// It uses the A* algorithm to find the most efficient path, considering movement costs and obstacles.
/// </summary>
public class PathFinder : MonoBehaviour
{
    [SerializeField] GridManager _gridManager;

    // Finds the shortest path from start to end using the A* algorithm
    public List<TileNode> FindPath(Vector3Int start, Vector3Int end)
    {
        _gridManager.ResetNodes();

        TileNode startNode = _gridManager.GetNode(start);
        TileNode endNode = _gridManager.GetNode(end);

        if (startNode == null || endNode == null)
            return null;

        startNode.gCost = 0;
        startNode.hCost = Heuristic(start, end);

        List<TileNode> openSet = new List<TileNode> { startNode };
        HashSet<TileNode> closedSet = new HashSet<TileNode>();

        while (openSet.Count > 0)
        {
            int minFCost = openSet.Min(n => n.fCost);
            int tolerance = 10;

            List<TileNode> candidates = new List<TileNode>();
            foreach (var node in openSet)
            {
                if (node.fCost <= minFCost + tolerance)
                {
                    candidates.Add(node);
                }
            }

            //if two paths are with the same cost (or almost the same), draw a random path
            TileNode currentNode = candidates[Random.Range(0, candidates.Count)];

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode);
            }

            foreach (TileNode neighbor in _gridManager.GetNeighbors(currentNode))
            {
                if (neighbor.isObstacle || closedSet.Contains(neighbor)) continue;

                int newGCost = currentNode.gCost + neighbor.movementCost;

                if (newGCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newGCost;
                    neighbor.hCost = Heuristic(neighbor.position, end);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        return null;
    }


    /// Heuristic function that calculates the estimated distance between two points.
    /// (Manhattan distance) for grid pathfinding.
    public int Heuristic(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    // Traces the path from the end node back to the start node by following parent nodes
    private List<TileNode> RetracePath(TileNode startNode, TileNode endNode)
    {
        List<TileNode> path = new List<TileNode>();
        TileNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent; // Move to the parent node
        }
        path.Reverse(); // Reverse to get the path from start to end
        return path;
    }
}
