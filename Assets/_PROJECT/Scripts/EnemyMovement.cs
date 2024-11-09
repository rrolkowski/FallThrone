using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EnemyMovement class controls the movement of enemy objects along a predefined path.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
	public PathFinderManager pathfindingManager;

	[SerializeField] float _movementSpeed;

	[HideInInspector] public Vector3Int closestPathPosition;

	[HideInInspector] public bool isMovable = true;

	private List<TileNode> _currentPath; // Przechowuje œcie¿kê dla tego konkretnego punktu startowego

	// Ustawia œcie¿kê na podstawie punktu startowego
	public void SetPath(List<TileNode> path)
	{
		_currentPath = path;
		StartCoroutine(MoveAlongPath(_currentPath));
	}

	public void RestartMovement()
	{
		StopAllCoroutines();
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

		// Loop through each node in the path list
		foreach (TileNode node in path)
		{
			Vector3 targetPosition = pathfindingManager.gridManager.tilemap.GetCellCenterWorld(node.position);
			Vector3 startPosition = transform.position;

			targetPosition.y = startPosition.y;

			while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
			{
				if (!isMovable) yield break;
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);
				yield return null;
			}

			if (node.position == closestPathPosition)
			{
				List<TileNode> newPath = pathfindingManager.GetPathFromTo(closestPathPosition, pathfindingManager.endPoint);
				StartCoroutine(MoveAlongPath(newPath));
				yield break;
			}
		}
	}
}
