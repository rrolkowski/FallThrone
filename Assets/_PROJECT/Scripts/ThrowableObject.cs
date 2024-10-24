using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    void OnCollisionEnter(Collision collision)
    {

        if (collision.gameObject.CompareTag("Ground"))
        {
            Debug.Log("Collision with the ground");

            if (TryGetComponent(out Rigidbody rb))
            {
                rb.isKinematic = true; // Setting the tower back to the kinematic
            }
            SetObjectAlpha(1.0f);

        }
    }

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
