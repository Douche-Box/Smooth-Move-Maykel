using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using UnityEngine;
using UnityEngine.InputSystem;

public class DeathManager : MonoBehaviour
{
    [SerializeField] PlayerInput _playerInput;
    [SerializeField] CharStateMachine _player;
    public CharStateMachine Player
    { get { return _player; } }

    [SerializeField] Transform _checkpointCollection;
    public Transform CheckPointCollection
    {
        set { _checkpointCollection = value; }
    }
    public List<Transform> _checkPointsList = new List<Transform>();


    [SerializeField] int _deathCount;
    public int DeathCount
    { get { return _deathCount; } set { _deathCount = value; } }

    [SerializeField] bool _hasDied;
    public bool HasDied
    { get { return _hasDied; } set { _hasDied = value; } }

    [SerializeField] bool _restartCheckpoints;
    public bool RestartCheckPoints
    { get { return _restartCheckpoints; } set { _restartCheckpoints = value; } }

    [SerializeField] Transform _resetPoint;

    [SerializeField] GameObject _deathScreen;
    public GameObject _sceneChangeScreen;

    [SerializeField] GameObject _checkPointScreen;
    [SerializeField] float _checkPointScreenTime;

    [SerializeField] TimeManager _timeManager;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _player = FindObjectOfType<CharStateMachine>();

        _timeManager = FindObjectOfType<TimeManager>();

        _checkpointCollection = GameObject.FindGameObjectWithTag("CheckPoints").transform;

        for (int i = 0; i < _checkpointCollection.childCount; i++)
        {
            _checkPointsList.Add(_checkpointCollection.GetChild(i));
        }
    }

    /// <summary>
    /// Finds all checkpoints in the level and does setup for respawning
    /// </summary>
    public void FindCheckPoints()
    {
        _checkpointCollection = GameObject.FindGameObjectWithTag("CheckPoints").transform;

        for (int i = 0; i < _checkpointCollection.childCount; i++)
        {
            _checkPointsList.Add(_checkpointCollection.GetChild(i));
        }
        _resetPoint = _checkPointsList[0];
        _player.transform.position = _resetPoint.position;
        _player.transform.forward = _resetPoint.forward;
        _player.Rb.useGravity = true;
        _sceneChangeScreen.SetActive(false);
    }

    private void Update()
    {
        if (_checkpointCollection == null && _restartCheckpoints)
        {
            FindCheckPoints();
            RestartCheckPoints = false;
        }
    }

    /// <summary>
    /// Handles logic for when the player dies and increases the total deaths for the run
    /// </summary>
    public void DoDeath()
    {
        _deathCount++;
        _timeManager.CanTime = false;
        if (_resetPoint != null && _checkpointCollection != null)
        {
            HasDied = true;
            _player.HasDied = true;
            _deathScreen.SetActive(true);
            _playerInput.enabled = false;
            FindObjectOfType<CharStateMachine>().transform.position = _resetPoint.position;
            _player.Rb.velocity = new Vector3(0, 0, 0);
            _player.PlayerObj.forward = _resetPoint.forward;
        }
    }

    /// <summary>
    /// Respawns the player at the last reached checkpoint
    /// </summary>
    public void DoRespawn()
    {
        HasDied = false;
        _player.HasDied = false;
        _deathScreen.SetActive(false);
        _playerInput.enabled = true;
    }

    /// <summary>
    /// Changes the newest checkpoint 
    /// </summary>
    /// <param name="point"></param>
    public void DoCheckPoint(Transform point)
    {
        if (_resetPoint != point)
        {
            _checkPointScreen.SetActive(true);
            StartCoroutine(CheckPointScreenTimer());
        }
        _resetPoint = point;
    }

    IEnumerator CheckPointScreenTimer()
    {
        yield return new WaitForSeconds(_checkPointScreenTime);
        _checkPointScreen.SetActive(false);
    }
}
