using UnityEngine;

public class HeadCollision : MonoBehaviour
{
    [SerializeField] float _slideForce = 2f;
    [SerializeField] Rigidbody _rb;

    void OnCollisionStay(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            if (contact.otherCollider.CompareTag("PlayerHead"))
            {
                Vector3 awayFromCenter = (transform.position - contact.otherCollider.transform.position);
                awayFromCenter.y = 0;

                if (awayFromCenter.magnitude < 0.1f)
                    awayFromCenter = new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));

                Vector3 slideDirection = awayFromCenter.normalized;

                _rb.AddForce(slideDirection * _slideForce, ForceMode.Acceleration);
                break;
            }
        }
    }

}
