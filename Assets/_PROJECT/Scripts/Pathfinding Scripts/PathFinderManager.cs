using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PathFinderManager class handles interactions with the PathFinder component to calculate
/// paths between designated start and end points on the grid. It manages start and end positions
/// as Vector3Int coordinates on the grid and provides methods to retrieve and debug paths.
/// </summary>
public class PathFinderManager : MonoBehaviour
{
    public GridManager gridManager;

    // Zamiast Transform, przechowujemy wspó³rzêdne kafelków
    public Vector3Int startPoint;  // The grid position of the starting point
    public Vector3Int endPoint;    // The grid position of the endpoint

    // Retrieves a path from the start point to the end point using PathFinder
    public List<TileNode> GetPath()
    {
        PathFinder pathfinding = GetComponent<PathFinder>();
        return pathfinding.FindPath(startPoint, endPoint);
    }

    // Retrieves a path from a specified start position to a specified end position
    public List<TileNode> GetPathFromTo(Vector3Int start, Vector3Int end)
    {
        Debug.Log("GetPathFromTo wywo³ane od punktu " + start + " do " + end);
        PathFinder pathfinding = GetComponent<PathFinder>();
        return pathfinding.FindPath(start, end);
    }
}
