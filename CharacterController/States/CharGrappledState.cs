using UnityEngine;
using UnityEngine.Video;

public class CharGrappledState : CharBaseState
{
    public CharGrappledState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the Grappled specific logic
    public override void EnterState()
    {
        InitializeSubState();

        Ctx.FinishedGrapple = false;
        Ctx.IsGrappling = true;

        Ctx.GrappleHooks--;

        // Changes DesiredMoveForce to GrappleSpeed for more control over speed
        Ctx.DesiredMoveForce = Ctx.GrappleSpeed;

        Ctx.IsForced = true;

        // Grapple uses ExtraForce to move so that you can go faster than the regular speed ceiling
        Ctx.ExtraForce = Ctx.GrappleSpeed;

        // Changes the GrappleDirection based on the grapple point and player position
        Ctx.GrappleDirection = (Ctx.GrapplePoint - Ctx.transform.position).normalized;

        // Line renderer for the grappling hook
        Ctx.GrappleLr.enabled = true;
        Ctx.GrappleLr.SetPosition(1, Ctx.GrapplePoint);

        Ctx.PlayerAnimator.SetTrigger("Grapple");

        Ctx.GrappleDelay = Ctx.MaxGrappleDelay;
    }

    // Makes sure everything is set up when changing from the grapple state
    public override void ExitState()
    {
        Ctx.IsGrappling = false;
        Ctx.GrappleLr.enabled = false;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();

        // Sets second position of the line renderer
        Ctx.GrappleLr.SetPosition(0, Ctx.GrappleLr.transform.position);

        Ctx.GrappleDelay -= Time.deltaTime;

        if (Ctx.GrappleDelay <= 0)
        {
            HandleGrappleMovement();
            Ctx.GrappleDelay = Ctx.MaxGrappleDelay;
        }
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState() { }

    #endregion

    // Grapple does not have any sub states because there is no extra logic needed
    public override void InitializeSubState() { }

    // Check if the state can be switched specific for the grapple state
    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded && Ctx.FinishedGrapple)
        {
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.IsSloped && Ctx.FinishedGrapple)
        {
            SwitchState(Factory.Sloped());
        }
        else if (!Ctx.IsGrounded && !Ctx.IsSloped && Ctx.FinishedGrapple)
        {
            SwitchState(Factory.Fall());
        }
    }
    // Handles the grapple logic
    private void HandleGrappleMovement()
    {
        Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0, Ctx.Rb.velocity.z);

        // The grapple is one boost of speed in the direction of the grapple
        Ctx.Rb.AddForce(Ctx.GrappleDirection * Ctx.GrappleSpeed, ForceMode.Impulse);

        Ctx.FinishedGrapple = true;
    }
}
