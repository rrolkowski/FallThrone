using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
	[SerializeField] private float _movementSpeed;
	[SerializeField] private Transform enemyModel;
	[SerializeField] private float rotationSpeed = 5f;
	[SerializeField] private Animator animator;

	[HideInInspector] public Vector3Int closestPathPosition;
	[HideInInspector] public bool isMovable = true;

	private Coroutine currentPathCoroutine;
	private List<TileNode> _currentPath;
	private bool isMoving = false;

	private float originalSpeed; // Oryginalna prêdkoœæ
	private float slowTimer = 0f; // Licznik czasu spowolnienia
	private bool isSlowed = false; // Flaga sprawdzaj¹ca, czy wróg jest spowolniony

	public System.Action OnPathEndReached;

	private void Start()
	{
		originalSpeed = _movementSpeed; // Zapamiêtanie oryginalnej prêdkoœci
	}

	private void Update()
	{
		// Jeœli wróg jest spowolniony, zmniejszamy licznik czasu spowolnienia
		if (isSlowed)
		{
			slowTimer -= Time.deltaTime;
			if (slowTimer <= 0f)
			{
				RemoveSlow();
			}
		}
	}

	public void ApplySlow(float duration, float slowPercentage)
	{
		_movementSpeed = originalSpeed * (1f - slowPercentage); // Zastosowanie efektu slow
		slowTimer = duration; // Ustawienie czasu trwania
		isSlowed = true; // Ustawienie flagi, ¿e wróg jest spowolniony
	}

	private void RemoveSlow()
	{
		_movementSpeed = originalSpeed; // Przywrócenie prêdkoœci
		isSlowed = false; // Usuniêcie efektu slow
	}

	public void SetPath(List<TileNode> path)
	{
		if (path == null || path.Count == 0)
		{
			Debug.LogError($"Enemy {gameObject.name} received an empty or null path.");
			return;
		}
		StopAllCoroutines();
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

	public IEnumerator MoveAlongPath(List<TileNode> path)
	{
		if (path == null || path.Count == 0)
		{
			Debug.LogError("No path available for enemy movement!");
			yield break;
		}
		if (isMoving) yield break;
		isMoving = true;

		if (animator != null)
		{
			animator.SetBool("walk", true);
		}

		foreach (TileNode node in path)
		{
			Vector3 targetPosition = PathFinderManager.Instance.gridManager.tilemap.GetCellCenterWorld(node.position);
			Vector3 startPosition = transform.position;
			targetPosition.y = startPosition.y;

			while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
			{
				if (!isMovable)
				{
					isMoving = false;

					if (animator != null)
					{
						animator.SetBool("walk", false);
					}
					yield break;
				}

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
				if (animator != null)
				{
					animator.SetBool("walk", false);
				}

				GameController.Instance.TakeDamage();
				this.gameObject.SetActive(false);
				yield break;
			}
		}

		isMoving = false;

		if (animator != null)
		{
			animator.SetBool("walk", false);
		}

		OnPathEndReached?.Invoke();
		OnPathEndReached = null;
	}
}
