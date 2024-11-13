using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThirdPersonCam : MonoBehaviour
{
    [SerializeField] Transform _orientation;
    [SerializeField] Transform _player;
    [SerializeField] Transform _camHolder;
    [SerializeField] Transform _playerObj;

    [SerializeField] float _rotationSpeed;

    [SerializeField] CharStateMachine _stateMachine;

    public enum CamState
    {
        ground,
        air,
        jump,
        wallrun,
    }

    public CamState camState;

    float inputY;
    float inputX;

    float oldInputY;
    float oldInputX;


    Vector3 inputDir;

    Quaternion currentCameraRotation;
    Quaternion previousCameraRotation;

    [SerializeField] float _rotationThreshold;


    [SerializeField] List<GameObject> _cinemachineCams = new List<GameObject>();

    private void Start()
    {
        DontDestroyOnLoad(_camHolder);
        _stateMachine = FindObjectOfType<CharStateMachine>();

        _cinemachineCams.Add(GameObject.FindGameObjectWithTag("CineMachine"));

        for (int i = 0; i < _cinemachineCams.Count; i++)
        {
            DontDestroyOnLoad(_cinemachineCams[i]);
        }

        _orientation = _stateMachine.Orientation;
        _player = _stateMachine.transform;
        _playerObj = _stateMachine.PlayerObj;
    }

    void Update()
    {
        Vector3 viewDir = _player.position - new Vector3(transform.position.x, _player.position.y, transform.position.z);
        _orientation.forward = viewDir.normalized;

        inputDir = _orientation.forward * _stateMachine.CurrentMovementInput.y + _orientation.right * _stateMachine.CurrentMovementInput.x;
        inputY = _stateMachine.CurrentMovementInput.y;
        inputX = _stateMachine.CurrentMovementInput.x;

        inputDir.y = 0;

        currentCameraRotation = transform.rotation;
        float rotationChange = Quaternion.Angle(currentCameraRotation, previousCameraRotation);

        // Airborne camera logic
        if (_stateMachine.IsAired && rotationChange > _rotationThreshold)
        {
            Quaternion lookRotation = Quaternion.LookRotation(viewDir, Vector3.up);
            _playerObj.transform.rotation = Quaternion.Slerp(_playerObj.transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
            previousCameraRotation = currentCameraRotation;
        }
        // Wallrunning camera logic
        else if (_stateMachine.IsWallRunning)
        {
            if ((_stateMachine.PlayerObj.forward - _stateMachine.WallForward).magnitude > (_stateMachine.PlayerObj.forward + _stateMachine.WallForward).magnitude)
            {
                _stateMachine.WallForward = new Vector3(-_stateMachine.WallForward.x, -_stateMachine.WallForward.y, -_stateMachine.WallForward.z);
            }
            _orientation.forward = _stateMachine.WallForward;
            Quaternion wallLookRotation = Quaternion.LookRotation(_stateMachine.WallForward.normalized, Vector3.up);
            _playerObj.transform.rotation = wallLookRotation;
        }
        // Grappling camera logic
        else if (_stateMachine.IsGrappling)
        {
            Quaternion GrappleLookRotation = Quaternion.LookRotation(viewDir, Vector3.up);
            _playerObj.transform.rotation = GrappleLookRotation;
        }
        // Sliding camera logic
        else if (_stateMachine.IsSliding && !_stateMachine.IsSloped)
        {
            Quaternion slopeAdjustedRotation = Quaternion.FromToRotation(Vector3.up, _stateMachine._slopeHit.normal);
            Quaternion lookRotation = Quaternion.LookRotation(inputDir, Vector3.up);
            Quaternion finalRotation = slopeAdjustedRotation * lookRotation;
            _playerObj.transform.rotation = Quaternion.Slerp(_playerObj.transform.rotation, finalRotation, Time.deltaTime * _rotationSpeed);

        }
        // Sliding camera logic
        else if (_stateMachine.IsSliding && _stateMachine.IsSloped)
        {
            Quaternion slopeAdjustedRotation = Quaternion.FromToRotation(Vector3.up, _stateMachine._slopeHit.normal);
            Quaternion lookRotation = Quaternion.LookRotation(viewDir, Vector3.up);
            Quaternion finalRotation = slopeAdjustedRotation * lookRotation;
            _playerObj.transform.rotation = Quaternion.Slerp(_playerObj.transform.rotation, finalRotation, Time.deltaTime * _rotationSpeed);

        }
        // Idle camera logic
        else if (inputDir != Vector3.zero && !_stateMachine.IsAired)
        {
            if (inputY == -oldInputY && inputY == 1f || inputY == -oldInputY && inputY == -1f || inputX == -oldInputX && inputX == 1f || inputX == -oldInputX && inputX == -1f)
            {
                Quaternion lookRotation = Quaternion.LookRotation(inputDir, Vector3.up);
                _playerObj.transform.rotation = lookRotation;
            }
            else
            {
                Quaternion lookRotation = Quaternion.LookRotation(inputDir, Vector3.up);
                _playerObj.transform.rotation = Quaternion.Slerp(_playerObj.transform.rotation, lookRotation, Time.deltaTime * _rotationSpeed);
            }
        }

        if (inputY != 0)
        {
            oldInputY = inputY;
            oldInputX = inputX;
        }
        if (inputX != 0)
        {
            oldInputX = inputX;
            oldInputY = inputY;
        }
    }
}
