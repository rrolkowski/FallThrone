using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The PathFinderManager class handles interactions with the PathFinder component to calculate
/// paths between designated start and end points on the grid. It manages start and end positions
/// as Vector3Int coordinates on the grid and provides methods to retrieve and debug paths.
/// </summary>
public class PathFinderManager : MonoBehaviour
{	
    public static PathFinderManager Instance { get; private set; }

    public GridManager gridManager;

    [HideInInspector]
    public Vector3Int endPoint; // Punkt koñcowy na siatce

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Zapewniamy, ¿e jest tylko jeden
        }
    }

    private void Start()
    {
		endPoint = gridManager.GetEndTilePosition();
    }

    /// <summary>
    /// Metoda do pobrania œcie¿ki od konkretnego punktu startowego do punktu koñcowego.
    /// </summary>
    public List<TileNode> GetPathFromSpawnPoint(Vector3Int spawnPoint)
	{
		PathFinder pathfinding = GetComponent<PathFinder>();
		return pathfinding.FindPath(spawnPoint, endPoint);
	}

	/// <summary>
	/// Pobiera wszystkie spawn tile positions
	/// </summary>
	public List<Vector3Int> GetSpawnPositions()
	{
		return gridManager.GetSpawnTilePositions();
	}

	/// <summary>
	/// Metoda do pobrania œcie¿ki od dowolnego punktu startowego do koñcowego
	/// </summary>
	public List<TileNode> GetPathFromTo(Vector3Int start, Vector3Int end)
	{
		Debug.Log("GetPathFromTo wywo³ane od punktu " + start + " do " + end);
		PathFinder pathfinding = GetComponent<PathFinder>();
		return pathfinding.FindPath(start, end);
	}
}
