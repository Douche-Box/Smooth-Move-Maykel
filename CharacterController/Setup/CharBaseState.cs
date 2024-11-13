using Unity.VisualScripting;
using UnityEngine;

public abstract class CharBaseState
{
    protected bool _isRootState = false;
    protected bool IsRootState
    { set { _isRootState = value; } }

    private CharStateMachine _ctx;
    protected CharStateMachine Ctx
    { get { return _ctx; } }

    private CharStateFactory _factory;
    protected CharStateFactory Factory
    { get { return _factory; } }

    private CharBaseState _currentSubState;
    protected CharBaseState _currentSuperState;

    public CharBaseState(CharStateMachine currentContext, CharStateFactory charachterStateFactory)
    {
        _ctx = currentContext;
        _factory = charachterStateFactory;
    }

    /// <summary>
    /// Enter a new state and use state specific enter logic
    /// </summary>
    public abstract void EnterState();

    /// <summary>
    /// Exit the current state and use state specific exit logic
    /// </summary>
    public abstract void ExitState();

    /// <summary>
    /// Updates the currently active state
    /// </summary>
    public abstract void UpdateState();

    /// <summary>
    /// Updates the currently active state while also being fixed
    /// </summary>
    public abstract void FixedUpdateState();

    /// <summary>
    /// Use for state logic cleanup at the late update
    /// </summary>
    public abstract void LateUpdateState();

    /// <summary>
    /// Check for new states to switch to
    /// </summary>
    public abstract void CheckSwitchStates();

    /// <summary>
    /// Set a sub state to make the logic compact but expandable
    /// </summary>
    public abstract void InitializeSubState();

    /// <summary>
    /// Updates all states and substates
    /// </summary>
    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    /// <summary>
    /// Updates all states and substates while being fixed
    /// </summary>
    public void FixedUpdateStates()
    {
        FixedUpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.FixedUpdateState();
        }
    }

    /// <summary>
    /// Updates all states and substates late
    /// </summary>
    public void LateUpdateStates()
    {
        LateUpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.LateUpdateState();
        }
    }

    /// <summary>
    /// Switches the state and sets a substate if possible
    /// </summary>
    /// <param name="newState"></param>
    protected void SwitchState(CharBaseState newState)
    {
        ExitState();

        newState.EnterState();

        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }
    }

    /// <summary>
    /// Sets the super state to the new super state for state hierarchy
    /// </summary>
    /// <param name="newSuperState"></param>
    protected void SetSuperState(CharBaseState newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    /// <summary>
    /// Sets the sub state to the new sub state for state hierarchy
    /// </summary>
    /// <param name="newSubState"></param>
    protected void SetSubState(CharBaseState newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}