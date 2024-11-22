using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private Action<Unit> _action;

    private bool _isInitialized = false;
    public void Init(Action<Unit> action)
    {
        _action = action;
    }

    public void ResetValues()
    {
        if (_isInitialized)
        {
            if (TryGetComponent<HealthController>(out var healthController))
            {
                healthController.ResetHealth();
            }
            if (TryGetComponent<EnemyMovement>(out var enemyMovement))
            {
                enemyMovement.RestartMovement();
                enemyMovement.isMovable = true;
            }
        }
        else
        {
            _isInitialized = true;
        }
    }

}
