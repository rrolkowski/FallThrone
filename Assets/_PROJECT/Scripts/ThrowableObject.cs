using System.Collections.Generic;
using System.Net;
using UnityEngine;

/// <summary>
/// The ThrowableObject class controls the behavior of objects that can be grabbed, thrown,
/// and interact with the environment upon landing. It handles collision effects, visibility, 
/// and enemy interactions upon landing.
/// </summary>
public class ThrowableObject : MonoBehaviour
{
    // Handles collision events
    void OnCollisionEnter(Collision collision)
    {
        // Checks if the object collides with the ground
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Collision with the ground");

            //AUDIO
            if (this.gameObject.tag == "Enemy")
				AudioManager.PlaySound(SoundType.GAME_Enemy_Throw);
			if (this.gameObject.tag == "Tower")
				AudioManager.PlaySound(SoundType.GAME_Turret_Throw);

			if (TryGetComponent(out Rigidbody rb))
            {
               rb.isKinematic = true;
            }

            SetObjectAlpha(1.0f); // Resets the object's visibility to full opacity

            // If the object collides with an enemy, it triggers enemy movement
            if (gameObject.CompareTag("Enemy"))
            {
                Debug.Log("Enemy Object Detected!");

                if (TryGetComponent(out EnemyMovement enemy))
                {
                    enemy.OnPathEndReached = null; // Zapobiega nieoczekiwanym wywo³aniom
                    enemy.isMovable = false;

                    enemy.isMovable = true; // Reactivates enemy movement

                    // Determines the grid position where the object landed, and searching the closest wakable nodes
                    Vector3Int landingPosition = PathFinderManager.Instance.gridManager.tilemap.WorldToCell(transform.position);
                    TileNode closestNode = PathFinderManager.Instance.gridManager.GetClosestWalkableNode(landingPosition);

                    // If a walkable node is found, checks if it's on the path and initiates enemy movement along the path
                    if (closestNode != null)
                    {
                        //Debug.Log("Nearest walkable tile after landing: " + closestNode.position);

                        Transform childAtPosition = PathFinderManager.Instance.gridManager.GetChildAtGridPosition(closestNode.position);
                        //Checks if the enemy landed on a path tile
                        if (childAtPosition != null && childAtPosition.CompareTag("Path"))
                        {
                            //enemy.closestPathPosition = closestNode.position; // Sets the closest path position, so the enemy resumes its movement from here

                            // Retrieves the path from the current position to the end goal
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

                            // Gdy dotrze do kafelka œcie¿ki, ustaw œcie¿kê do punktu koñcowego
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
