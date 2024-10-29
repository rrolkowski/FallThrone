using System.Collections.Generic;
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
        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Collision with the ground");

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
                    enemy.isMovable = true; // Reactivates enemy movement

                    // Determines the grid position where the object landed, and searching the closest wakable nodes
                    Vector3Int landingPosition = enemy.pathfindingManager.gridManager.tilemap.WorldToCell(transform.position);
                    TileNode closestNode = enemy.pathfindingManager.gridManager.GetClosestWalkableNode(landingPosition);

                    // If a walkable node is found, checks if it's on the path and initiates enemy movement along the path
                    if (closestNode != null)
                    {
                        //Debug.Log("Nearest walkable tile after landing: " + closestNode.position);


                        // Checks if the enemy landed on a path tile
                        if (enemy.pathfindingManager.gridManager.tilemap.GetTile(closestNode.position) == enemy.pathfindingManager.gridManager.pathTile)
                        {

                            //enemy.closestPathPosition = closestNode.position; // Sets the closest path position, so the enemy resumes its movement from here

                            // Retrieves the path from the current position to the end goal
                            List<TileNode> path = enemy.pathfindingManager.GetPathFromTo(closestNode.position, enemy.pathfindingManager.endPoint); 
                            enemy.StartCoroutine(enemy.MoveAlongPath(path));
                        }
                        else
                        {
                            // Finds the nearest path tile to continue the enemy’s path movement
                            TileNode closestPathNode = enemy.pathfindingManager.gridManager.GetClosestPathTileNode(closestNode.position);
                            //Debug.Log("Value of closestPathTile: " + closestPathNode.position);

                            // Updates the closest path position and recalculates the path from the landing point to the closest path tile
                            enemy.closestPathPosition = closestPathNode.position;
                            List<TileNode> path = enemy.pathfindingManager.GetPathFromTo(closestNode.position, closestPathNode.position);
                            enemy.StartCoroutine(enemy.MoveAlongPath(path));
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
