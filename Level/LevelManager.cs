using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using Unity.VisualScripting;
using TMPro;

public class LevelManager : MonoBehaviour
{
    [SerializeField] TimeManager _timeManager;
    [SerializeField] DeathManager _deathManager;

    [SerializeField] private PlayerInput playerInput = null;
    public PlayerInput PlayerInput => playerInput;

    [SerializeField] bool _isReset;

    [SerializeField] bool _isNext;
    [SerializeField] float _nextTimer;
    [SerializeField] float _maxNextTimer;
    [SerializeField] bool _canNext;

    [SerializeField] CharStateMachine _player;

    [SerializeField] int checkPointInt;

    [SerializeField] string _levelScene;

    [SerializeField] GameObject _winScreen;
    [SerializeField] TMP_Text _winDeathTxt;

    [SerializeField] GameObject _optionsScreen;

    [SerializeField] bool _optionYn;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        _player = FindObjectOfType<CharStateMachine>();

        // Inputs for the level manager
        playerInput.actions.FindAction("Reset").started += OnReset;
        playerInput.actions.FindAction("Reset").performed += OnReset;
        playerInput.actions.FindAction("Reset").canceled += OnReset;

        playerInput.actions.FindAction("Next").started += OnNext;
        playerInput.actions.FindAction("Next").performed += OnNext;
        playerInput.actions.FindAction("Next").canceled += OnNext;
    }

    private void Update()
    {
        if (_player == null)
        {
            _player = FindObjectOfType<CharStateMachine>();
        }
        if (_player.IsMove)
        {
            _timeManager.CanTime = true;
        }

        if (_isReset)
        {
            _deathManager.DoDeath();
        }

        if (_deathManager.HasDied && Input.GetKeyDown(KeyCode.Space))
        {
            _deathManager.DoRespawn();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Resume();
        }

        if (_isNext && _canNext)
        {
            _nextTimer = _maxNextTimer;
            _canNext = false;
            if (checkPointInt >= _deathManager._checkPointsList.Count - 1)
            {
                LoadLevel(_levelScene);
            }
            else
            {
                checkPointInt++;
                _player.transform.position = _deathManager._checkPointsList[checkPointInt].position;
                _player.transform.forward = _deathManager._checkPointsList[checkPointInt].forward;
            }

        }

        if (_nextTimer <= 0)
        {
            _canNext = true;
            _nextTimer = 0;
        }
        if (!_canNext)
        {
            _nextTimer -= Time.deltaTime;
        }
    }

    // Opens the menu or closes the menu
    public void Resume()
    {
        _optionYn = !_optionYn;

        if (_optionYn)
        {
            Cursor.visible = true;

            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;

            Cursor.lockState = CursorLockMode.Locked;
        }

        _optionsScreen.SetActive(_optionYn);
    }

    void OnReset(InputAction.CallbackContext context)
    {
        _isReset = context.ReadValueAsButton();
    }

    void OnNext(InputAction.CallbackContext context)
    {
        _isNext = context.ReadValueAsButton();
    }

    /// <summary>
    /// End the run and display the winscreen with a death count and a end time
    /// </summary>
    public void EndGame()
    {
        _timeManager.CanTime = false;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        _winDeathTxt.text = _deathManager.DeathCount.ToString() + " deaths";
        _winScreen.SetActive(true);
    }

    /// <summary>
    /// Loads a new scene and sets up the checkpoints and stops time for loading
    /// </summary>
    /// <param name="scene"></param>
    public void LoadLevel(string scene)
    {
        checkPointInt = 0;
        _deathManager._checkPointsList.Clear();
        _deathManager._sceneChangeScreen.SetActive(true);
        _player.Rb.useGravity = false;
        _player.Rb.velocity = new Vector3(0, 0, 0);
        _timeManager.CanTime = false;
        Debug.Log(_deathManager.DeathCount);
        SceneManager.LoadScene(scene);
        _deathManager.RestartCheckPoints = true;
    }

    /// <summary>
    /// Restarts the game and sets the time back to 0 for a new run
    /// </summary>
    public void RestartBtn()
    {
        _timeManager.CanTime = false;
        _timeManager.ElapsedTime = 0;
        checkPointInt = 0;
        _deathManager.DeathCount = 0;
        Destroy(_player.gameObject);
        Resume();
        SceneManager.LoadScene("Level_1");
        Destroy(this.gameObject);
    }

    public void Quit()
    {
        Resume();
        Destroy(_player.gameObject);
        SceneManager.LoadScene("MainMenu");
        Destroy(this.gameObject);
    }
}
