using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// The EnemyMovement class controls the movement of enemy objects along a predefined path.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
    public PathFinderManager pathfindingManager;

    [SerializeField] float _movementSpeed;

    [HideInInspector] public Vector3Int closestPathPosition;

    [HideInInspector] public bool isMovable = true;

    void Start()
    {
        // TeleportToStart();
        List<TileNode> path = pathfindingManager.GetPath(); // Retrieves the path to follow
        StartCoroutine(MoveAlongPath(path)); // Starts moving along the path
    }

    // Moves the enemy directly to the start position on the grid // Temporary option!
    void TeleportToStart()
    {
        Vector3 startPosition = pathfindingManager.gridManager.tilemap.GetCellCenterWorld(pathfindingManager.startPoint);

        // Adjusts Y position so the enemy appears on top of the tilemap
        float yOffset = transform.localScale.y;
        transform.position = new Vector3(startPosition.x, startPosition.y + yOffset, startPosition.z);
    }

    public void RestartMovement()
    {
            StopAllCoroutines(); // Stop coroutines
            List<TileNode> path = pathfindingManager.GetPath();
            StartCoroutine(MoveAlongPath(path));
    }

    // Coroutine to move the enemy along a path node by node
    public IEnumerator MoveAlongPath(List<TileNode> path)
    {
        if (path == null || path.Count == 0)
        {
            Debug.LogError("No path available for enemy movement!");
            yield break;
        }

        // Loop through each node in the path list
        foreach (TileNode node in path)
        {
            //Debug.Log("Enemy moving towards tile: " + node.position);

            // Calculate the world position of the target tile for this path node
            Vector3 targetPosition = pathfindingManager.gridManager.tilemap.GetCellCenterWorld(node.position);
            Vector3 startPosition = transform.position;

            targetPosition.y = startPosition.y; // Keeps movement on the same Y plane as the start position

            // Move the enemy towards the target position
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
            {
                if (!isMovable) yield break; // Check if movement is allowed
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);

                yield return null;
            }

            // If the return point is reached, recalculate the path from the current position to the final target
            if (node.position == closestPathPosition)
            {
                //Debug.Log("Reached designated return point. Continuing towards main objective.");

                List<TileNode> newPath = pathfindingManager.GetPathFromTo(closestPathPosition, pathfindingManager.endPoint);
                StartCoroutine(MoveAlongPath(newPath));
                yield break;
            }
        }

    }
}

