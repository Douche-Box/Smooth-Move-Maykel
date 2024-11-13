using UnityEngine;

public class CharSlopeState : CharBaseState
{
    public CharSlopeState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the slope specific logic
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.Rb.useGravity = false;

        // Sets the JumpMent to be straight up when in the slope state
        Ctx.JumpMent = new Vector3(0, 1, 0);
    }

    // Makes sure everything is set up when changing from the slope state
    public override void ExitState()
    {
        Ctx.Rb.useGravity = true;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        // Sets the Movement to be based on the slope and movement direction
        Ctx.Movement = Ctx.GetSlopeMoveDirection(Ctx.CurrentMovement);

        // Changes the MoveMultiplier when going down the slope
        if (Ctx.Rb.velocity.y > 0)
        {
            Ctx.MoveMultiplier = 2f;
        }

        // Makes sure the character is stuck to the slope when going down the slope or sliding
        if (Ctx.Rb.velocity.y > 0 || Ctx.IsSliding)
        {
            Ctx.Rb.AddForce(Vector3.down * 80f, ForceMode.Force);
        }
    }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    #endregion

    // Initialize sub states for the slope state
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
        else if (Ctx.IsSlide && Ctx.IsMove)
        {
            SetSubState(Factory.Slide());
        }
        else if (Ctx.IsAired)
        {
            SetSubState(Factory.Fall());
        }
    }

    // Check if the state can be switched specific for the slope state
    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsGrounded && !Ctx.IsSloped)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump && !Ctx.IsSlide)
        {
            SwitchState(Factory.Jump());
        }
    }
}
