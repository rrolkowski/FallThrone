using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The ThrowableObject class controls the behavior of objects that can be grabbed, thrown,
/// and interact with the environment upon landing. It handles collision effects, visibility, 
/// and enemy interactions upon landing.
/// </summary>
public class ThrowableObject : MonoBehaviour
{
	private float lastSoundTime = -Mathf.Infinity;
	private const float soundCooldown = 0.5f;

	void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
		{
			if (Time.time - lastSoundTime >= soundCooldown)
			{
				PlayThrowSound();
				lastSoundTime = Time.time;
			}

			if (TryGetComponent(out Rigidbody rb))
				rb.isKinematic = true;

			SetObjectAlpha(1.0f); // Full opacity

			if (gameObject.CompareTag("Enemy") && TryGetComponent(out EnemyMovement enemy))
			{
				enemy.OnPathEndReached = null;
				enemy.isMovable = true;

				Vector3Int landingPosition = PathFinderManager.Instance.gridManager.tilemap.WorldToCell(transform.position);
				TileNode closestNode = PathFinderManager.Instance.gridManager.GetClosestWalkableNode(landingPosition);

				if (closestNode != null)
				{
					Transform childAtPosition = PathFinderManager.Instance.gridManager.GetChildAtGridPosition(closestNode.position);
					if (childAtPosition != null && childAtPosition.CompareTag("Path"))
					{
						List<TileNode> path = PathFinderManager.Instance.GetPathFromTo(closestNode.position, PathFinderManager.Instance.endPoint);
						enemy.OnPathEndReached = null;
						enemy.SetPath(path);
					}
					else
					{
						TileNode closestPathNode = PathFinderManager.Instance.gridManager.GetClosestPathTileNode(closestNode.position);
						List<TileNode> pathToPathTile = PathFinderManager.Instance.GetPathFromTo(closestNode.position, closestPathNode.position);
						enemy.OnPathEndReached = null;
						enemy.SetPath(pathToPathTile);

						enemy.OnPathEndReached = () =>
						{
							List<TileNode> pathToEnd = PathFinderManager.Instance.GetPathFromTo(closestPathNode.position, PathFinderManager.Instance.endPoint);
							enemy.SetPath(pathToEnd);
						};
					}
				}
			}
		}
	}

	private void PlayThrowSound()
	{
		if (gameObject.tag == "Enemy")
		{
			int layer = gameObject.layer;

			if (layer == LayerMask.NameToLayer("Bomba"))
				AudioManager.PlaySound(SoundType.GAME_Enemy_Throw);
			else if (layer == LayerMask.NameToLayer("Duch"))
				AudioManager.PlaySound(SoundType.GAME_Enemy_Duch_Throw);
			else if (layer == LayerMask.NameToLayer("Bober"))
				AudioManager.PlaySound(SoundType.GAME_Enemy_Bober_Throw);
			else if (layer == LayerMask.NameToLayer("Slimak"))
				AudioManager.PlaySound(SoundType.GAME_Enemy_Slimak_Throw);
		}
		else if (gameObject.tag == "Tower")
		{
			AudioManager.PlaySound(SoundType.GAME_Turret_Throw);
		}
	}

	// Adjusts the object's transparency by setting the alpha value of its material
	public void SetObjectAlpha(float alpha)
	{
		if (TryGetComponent(out Renderer renderer))
		{
			Color color = renderer.material.color;
			color.a = alpha;
			renderer.material.color = color;
		}
	}
}
