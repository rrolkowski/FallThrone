using System;
using UnityEngine;

public abstract class BasePowerUp : MonoBehaviour
{
    public abstract void ApplyEffect();

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameController.Instance.SetPowerUp(this);
            
            GetComponent<Collider>().enabled = false;

            if (GetComponent<SpriteRenderer>() != null)
                GetComponent<SpriteRenderer>().enabled = false;

            if (GetComponent<ParticleSystem>() != null)
                GetComponent<ParticleSystem>().Stop();
        }
    }
}
