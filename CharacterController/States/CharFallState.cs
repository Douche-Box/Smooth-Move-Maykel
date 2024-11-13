using UnityEngine;

public class CharFallState : CharBaseState
{
    public CharFallState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the fall specific logic
    public override void EnterState()
    {
        InitializeSubState();
        Ctx.IsAired = true;

        Ctx.MoveMultiplier = Ctx.AirSpeed;

        // ForceSlowDownRate based on what state it is in will slow down the character slower than the ground state
        Ctx.ForceSlowDownRate = 1;
    }

    // Makes sure everything is set up when changing from the fall state
    public override void ExitState()
    {
        Ctx.IsAired = false;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        // Sets the Movement to be based on inputs
        Ctx.Movement = Ctx.CurrentMovement.normalized;
    }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    #endregion

    // Initialize sub states for the fall state
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

    // Check if the state can be switched specific for the fall state
    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsJump && Ctx.VaultLow && !Ctx.VaultMedium)
        {
            SwitchState(Factory.Vaulted());
        }
        else if (Ctx.IsSloped)
        {
            SwitchState(Factory.Sloped());
        }
        else if (Ctx.IsWalled && !(Ctx.WallLeft && Ctx.CurrentMovementInput.x > 0) && !(Ctx.WallRight && Ctx.CurrentMovementInput.x < 0) && Ctx.IsMove)
        {
            SwitchState(Factory.Walled());
        }
        else if (Ctx.IsGrappled && Ctx.IsShoot && Ctx.GrappleHooks > 0)
        {
            SwitchState(Factory.Grappled());
        }
    }
}