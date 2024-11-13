using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoints : MonoBehaviour
{
    [SerializeField] DeathManager _deathManager;

    [SerializeField] Transform _checkPoint;

    // Basic checkpoint

    private void Awake()
    {
        _deathManager = FindObjectOfType<DeathManager>();
        _checkPoint = this.transform;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _deathManager.DoCheckPoint(_checkPoint);
        }
    }
}
