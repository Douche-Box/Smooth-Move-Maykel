using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Robot : MonoBehaviour
{
    // Basic robot enemy

    [SerializeField] CharStateMachine _player;

    [SerializeField] int _maxRoboHealth;
    [SerializeField] int _robotHealth;
    [SerializeField] bool _roboHit;

    [SerializeField] Navmeshplayer _roboMover;
    [SerializeField] NavMeshAgent _roboNavmesh;

    [SerializeField] int _roboRebootTime;

    [SerializeField] Collider _hitbox;
    [SerializeField] Transform scaleer;
    [SerializeField] Animator _topSaw;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _roboHit = true;
        }
    }

    private void Awake()
    {
        _robotHealth = _maxRoboHealth;
        _player = FindObjectOfType<CharStateMachine>();
    }

    private void Update()
    {
        if (_player.HasDied)
        {
            _robotHealth = _maxRoboHealth;
        }
        if (_roboHit)
        {
            _roboHit = false;
            _roboMover.Cando = false;
            _roboNavmesh.enabled = false;
            _hitbox.enabled = false;
            StartCoroutine(RobotReboot());
        }
        if (_robotHealth <= 0)
        {
            Destroy(scaleer.gameObject);
        }
    }

    // Makes sure the player cant keep jumping on the robot by using its saws
    IEnumerator RobotReboot()
    {
        yield return new WaitForSeconds(_roboRebootTime);
        if (_topSaw != null)
        {
            _topSaw.SetBool("Out", true);
        }
        _roboMover.Cando = true;
        _roboNavmesh.enabled = true;
        _hitbox.enabled = true;
        _robotHealth--;
        yield return new WaitForSeconds(_roboRebootTime);
        if (_topSaw != null)
        {
            _topSaw.SetBool("Out", false);
        }
    }
}
