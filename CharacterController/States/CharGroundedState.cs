using UnityEngine;
public class CharGroundedState : CharBaseState
{
    public CharGroundedState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the ground specific logic
    public override void EnterState()
    {
        InitializeSubState();

        // MoveMultiplier based on what state it is in
        Ctx.MoveMultiplier = 1f;

        // Resets the wall running
        Ctx.CanStartWallTimer = false;
        Ctx.WallClingTime = Ctx.MaxWallClingTime;

        // ForceSlowDownRate based on what state it is in will slow down the character faster
        Ctx.ForceSlowDownRate = 5;
        Ctx.IsAired = false;

        // Sets the DesiredMoveForce to the MoveSpeed for speed handling
        Ctx.DesiredMoveForce = Ctx.MoveSpeed;
        Ctx.IsJumpTime = Ctx.MaxJumpTime;

        // Sets the JumpMent to be straight up when in the ground state
        Ctx.JumpMent = new Vector3(0, 1, 0);

        // Resets the grapple hooks
        Ctx.GrappleHooks = 1;

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
        Ctx.PlayerAnimator.SetFloat("Running", Ctx.MovementSpeed);

        // Sets the Movement to be based on inputs
        Ctx.Movement = Ctx.CurrentMovement.normalized;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    #endregion

    // Initialize sub states for the ground state
    public override void InitializeSubState()
    {
        if (!Ctx.IsMove)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMove && !Ctx.IsSlide)
        {
            SetSubState(Factory.Walk());
        }
        else if (Ctx.IsMove && Ctx.IsSlide && Ctx.MoveForce >= Ctx.MoveSpeed && !Ctx.IsAired)
        {
            SetSubState(Factory.Slide());
        }
    }

    // Check if the state can be switched specific for the ground state
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump && Ctx.VaultLow && !Ctx.VaultMedium)
        {
            SwitchState(Factory.Vaulted());
        }
        else if (Ctx.IsJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
        else if (Ctx.IsGrappled && Ctx.IsShoot && Ctx.GrappleHooks > 0)
        {
            SwitchState(Factory.Grappled());
        }
    }
}