using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EnemyMovement class controls the movement of enemy objects along a predefined path.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
	//public PathFinderManager pathfindingManager;

	[SerializeField] float _movementSpeed;

	[HideInInspector] public Vector3Int closestPathPosition;

	[HideInInspector] public bool isMovable = true;

	private Coroutine currentPathCoroutine;

    private List<TileNode> _currentPath; // Przechowuje œcie¿kê dla tego konkretnego punktu startowego

	private bool isMoving = false;
    public System.Action OnPathEndReached;
	
    // Ustawia œcie¿kê na podstawie punktu startowego
    public void SetPath(List<TileNode> path)
	{
		if (path == null || path.Count == 0)
		{
			Debug.LogError($"Enemy {gameObject.name} received an empty or null path.");
			return;
		}
        StopAllCoroutines(); // Zatrzymuje bie¿¹ce ruchy
        isMoving = false;
		_currentPath = path;

		//if (isMoving)
		//{
		//    Debug.LogWarning("Enemy is already moving! Ignoring new path.");
		//    return;
		//}

		StartCoroutine(MoveAlongPath(_currentPath));
	}
    public void RestartMovement()
	{
		StopAllCoroutines();
		isMoving = false;
        if (_currentPath != null)
		{
			StartCoroutine(MoveAlongPath(_currentPath));
		}
	}

	// Coroutine to move the enemy along a path node by node
	public IEnumerator MoveAlongPath(List<TileNode> path)
	{
		if (path == null || path.Count == 0)
		{
			Debug.LogError("No path available for enemy movement!");
			yield break;
		}
        if (isMoving) yield break;
        isMoving = true;

		// Loop through each node in the path list
		foreach (TileNode node in path)
		{
			Vector3 targetPosition = PathFinderManager.Instance.gridManager.tilemap.GetCellCenterWorld(node.position);
            Vector3 startPosition = transform.position;
			targetPosition.y = startPosition.y;

           //Debug.Log($"Moving from {startPosition} to {targetPosition}, Distance: {Vector3.Distance(startPosition, targetPosition)}");
            while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
			{
                if (!isMovable)
                {
                    isMoving = false;
                    yield break;
                }
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
                yield return null;
			}

			if (node.position == closestPathPosition)
			{
				List<TileNode> newPath = PathFinderManager.Instance.GetPathFromTo(closestPathPosition, PathFinderManager.Instance.endPoint);
				Debug.Log($"closestPathPosition: {closestPathPosition}");
				StartCoroutine(MoveAlongPath(newPath));
				yield break;
			}

            if (node.position == PathFinderManager.Instance.endPoint)
            {
                Debug.LogWarning($"Enemy {gameObject.name} reached endpoint {node.position}.");
                yield break; // Przerywa korutynê
            }
        }
		isMoving = false;
		OnPathEndReached?.Invoke();
        OnPathEndReached = null;
    }
}
