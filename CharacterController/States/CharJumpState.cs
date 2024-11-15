using UnityEngine;

public class CharJumpState : CharBaseState
{
    public CharJumpState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the jump specific logic
    public override void EnterState()
    {
        InitializeSubState();

        Ctx.IsJumping = true;

        Ctx.PlayerAnimator.SetBool("Jump", true);
        Ctx.IsExitingSlope = true;

        HandleJump();
    }

    // Makes sure everything is set up when changing from the jump state
    public override void ExitState()
    {
        Ctx.IsJumping = false;
        Ctx.IsForced = false;
        Ctx.IsExitingSlope = false;
        Ctx.PlayerAnimator.SetBool("Jump", false);
    }

    #region MonoBehaveiours

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
        HandleJumpTime();
    }

    public override void LateUpdateState() { }

    #endregion

    // Initialize sub states for the jump state
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
    }

    // Check if the state can be switched specific for the jump state
    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
        else if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
    }

    // Logic for jumping uses JumpMent based on what state or substates it is in
    void HandleJump()
    {
        Ctx.IsForced = true;
        Ctx.ExtraForce = 21;

        Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0f, Ctx.Rb.velocity.z);

        Ctx.Rb.AddForce(Ctx.JumpMent * Ctx.JumpForce, ForceMode.Impulse);
    }

    // Hanles the refreshing of the jump time
    void HandleJumpTime()
    {
        if (Ctx.IsJumpTime > 0)
        {
            Ctx.IsJumpTime -= Time.deltaTime;
        }
        else
        {
            Ctx.IsJumpTime = 0;
        }
    }
}
