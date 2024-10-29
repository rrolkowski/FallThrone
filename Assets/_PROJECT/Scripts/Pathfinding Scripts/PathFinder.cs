using System.Collections.Generic;
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

        List<TileNode> openSet = new List<TileNode> { startNode }; // Nodes to be evaluated
        HashSet<TileNode> closedSet = new HashSet<TileNode>(); // Nodes already evaluated


        while (openSet.Count > 0)
        {
            TileNode currentNode = openSet[0];

            // Find the node in openSet with the lowest total cost (movement cost + heuristic)
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].movementCost + Heuristic(openSet[i].position, end) < currentNode.movementCost + Heuristic(currentNode.position, end))
                {
                    currentNode = openSet[i];
                }
            }

            openSet.Remove(currentNode);// Move current node from openSet to closedSet
            closedSet.Add(currentNode);

            if (currentNode == endNode)
            {
                return RetracePath(startNode, endNode); // Trace the path back from end to start
            }

            // Process each neighboring node of the current node
            foreach (TileNode neighbor in _gridManager.GetNeighbors(currentNode))
            {
                if (neighbor.isObstacle || closedSet.Contains(neighbor)) continue;

                int newCost = currentNode.movementCost + neighbor.movementCost;

                // If a shorter path to the neighbor is found, update its cost and parent
                if (newCost < neighbor.movementCost || !openSet.Contains(neighbor))
                {
                    neighbor.movementCost = newCost;
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
