using UnityEngine;

public class CharWalkState : CharBaseState
{
    public CharWalkState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    // Setup for the walk specific logic
    public override void EnterState()
    {
        Ctx.DesiredMoveForce = Ctx.MoveSpeed;

        // Only sets the speed to the desired speed when under the disered move speed
        if (Ctx.MoveForce < Ctx.MoveSpeed)
        {
            Ctx.MoveForce = Ctx.MoveSpeed;
        }
    }

    public override void ExitState() { }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        WalkMovement();
    }

    #endregion

    public override void InitializeSubState() { }

    // Check if the state can be switched specific for the walk state
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMove)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsSlide && Ctx.IsMove && !Ctx.IsWalled && Ctx.MoveForce >= Ctx.MoveSpeed && !Ctx.IsJumping)
        {
            SwitchState(Factory.Slide());
        }
    }

    // Logic for walking uses Movement based on the input and the state and sub states it is in
    private void WalkMovement()
    {
        Ctx.Rb.AddForce(Ctx.Movement * Ctx.MoveForce * 10f * Ctx.MoveMultiplier * Ctx.StrafeSpeedMultiplier, ForceMode.Force);
    }

}