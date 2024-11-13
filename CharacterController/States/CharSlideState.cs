using UnityEngine;

public class CharSlideState : CharBaseState
{
    public CharSlideState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory) { }

    // Setup for the slide specific logic
    public override void EnterState()
    {
        // Changes the DesiredMoveForce speed to SlideSpeed for more specific slide speed
        Ctx.DesiredMoveForce = Ctx.SlideSpeed;

        Ctx.IsSliding = true;

        // Makes sure the hit box for the character is accurate
        Ctx.Colliders[0].enabled = false;
        Ctx.Colliders[1].enabled = true;

        // Only sets the speed to the slide speed when under the disered move speed
        if (Ctx.MoveForce < Ctx.SlideSpeed)
        {
            Ctx.MoveForce = Ctx.SlideSpeed;
        }

        Ctx.PlayerAnimator.SetBool("Sliding", true);


    }

    // Makes sure everything is set up when changing from the slide state
    public override void ExitState()
    {
        Ctx.IsSliding = false;

        // Makes sure the hit box for the character is accurate
        Ctx.Colliders[0].enabled = true;
        Ctx.Colliders[1].enabled = false;

        Ctx.PlayerAnimator.SetBool("Sliding", false);
    }

    #region MonoBehaveiours

    public override void UpdateState() { }

    public override void FixedUpdateState()
    {
        SlidingMovement();
        CheckSwitchStates();
    }

    public override void LateUpdateState() { }

    #endregion

    public override void InitializeSubState() { }

    // Check if the state can be switched specific for the slide state
    public override void CheckSwitchStates()
    {
        // Seperate checks for more readablity and reason for change of change for testing
        if (!Ctx.IsMove && !Ctx.UpCheck)
        {
            SwitchState(Factory.Idle());
        }
        else if (Ctx.IsMove && Ctx.IsAired && !Ctx.UpCheck)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsMove && Ctx.IsJumping && !Ctx.UpCheck)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsMove && !Ctx.IsSlide && !Ctx.UpCheck)
        {
            SwitchState(Factory.Walk());
        }
        else if (Ctx.IsMove && Ctx.IsSlide && Ctx.IsWalled && !Ctx.UpCheck)
        {
            SwitchState(Factory.Walk());
        }
        // Changes state to walk when going to slow in the slide state
        else if (Ctx.MoveForce <= Ctx.LowestSlideSpeed && !Ctx.UpCheck)
        {
            SwitchState(Factory.Walk());
        }
    }

    // Logic for sliding uses Movement based on what state or substates it is in
    private void SlidingMovement()
    {
        // Changes slide speed based on slope for extra speed
        if (Ctx.IsSloped && Ctx.Rb.velocity.y < 0.1f)
        {
            Ctx.DesiredMoveForce = Ctx.SlopeSlideSpeed;
        }
        else if (!Ctx.IsSloped && Ctx.IsGrounded || Ctx.Rb.velocity.y > 0.1f && Ctx.IsGrounded)
        {
            Ctx.DesiredMoveForce = Ctx.LowestSlideSpeed;
        }

        Ctx.Rb.AddForce(Ctx.Movement * Ctx.MoveForce * 10f * Ctx.MoveMultiplier, ForceMode.Force);
    }
}
