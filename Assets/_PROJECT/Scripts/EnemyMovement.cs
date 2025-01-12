using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The EnemyMovement class controls the movement of enemy objects along a predefined path and manages its animation.
/// </summary>
public class EnemyMovement : MonoBehaviour
{
	[SerializeField] private float _movementSpeed;
	[SerializeField] private Transform enemyModel; // Referencja do modelu wroga, który ma byæ rotowany.
	[SerializeField] private float rotationSpeed = 5f; // Szybkoœæ rotacji
	[SerializeField] private Animator animator; // Referencja do Animatora

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

		// Aktywuj animacjê chodzenia
		if (animator != null)
		{
			animator.SetBool("walk", true);
		}

		// Loop through each node in the path list
		foreach (TileNode node in path)
		{
			Vector3 targetPosition = PathFinderManager.Instance.gridManager.tilemap.GetCellCenterWorld(node.position);
			Vector3 startPosition = transform.position;
			targetPosition.y = startPosition.y;

			// Przemieszczanie wroga do kolejnego punktu
			while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
			{
				if (!isMovable)
				{
					isMoving = false;

					// Dezaktywuj animacjê chodzenia
					if (animator != null)
					{
						animator.SetBool("walk", false);
					}
					yield break;
				}

				// Oblicz kierunek i p³ynn¹ rotacjê modelu
				Vector3 direction = (targetPosition - transform.position).normalized;
				if (enemyModel != null && direction != Vector3.zero)
				{
					Quaternion targetRotation = Quaternion.LookRotation(direction);
					enemyModel.rotation = Quaternion.Slerp(
						enemyModel.rotation,
						targetRotation,
						rotationSpeed * Time.deltaTime
					);
				}

				// Przesuñ wroga w kierunku celu
				transform.position = Vector3.MoveTowards(transform.position, targetPosition, _movementSpeed * Time.deltaTime);

				yield return null;
			}

			if (node.position == closestPathPosition)
			{
				List<TileNode> newPath = PathFinderManager.Instance.GetPathFromTo(closestPathPosition, PathFinderManager.Instance.endPoint);
				StartCoroutine(MoveAlongPath(newPath));
				yield break;
			}

			if (node.position == PathFinderManager.Instance.endPoint)
			{
				// Dezaktywuj animacjê chodzenia
				if (animator != null)
				{
					animator.SetBool("walk", false);
				}

				GameController.Instance.TakeDamage();
				this.gameObject.SetActive(false);
				yield break; // Przerywa korutynê
			}
		}

		isMoving = false;

		// Dezaktywuj animacjê chodzenia, wróæ do Idle
		if (animator != null)
		{
			animator.SetBool("walk", false);
		}

		OnPathEndReached?.Invoke();
		OnPathEndReached = null;
	}
}
