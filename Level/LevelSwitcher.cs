using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitcher : MonoBehaviour
{
    [SerializeField] string _sceneToLoad;
    [SerializeField] LevelManager _levelManager;

    private void Awake()
    {
        _levelManager = FindObjectOfType<LevelManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.GetComponentInParent<CharStateMachine>())
        {
            _levelManager.LoadLevel(_sceneToLoad);
        }
    }
}
