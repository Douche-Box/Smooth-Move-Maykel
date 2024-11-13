using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharWallrunState : CharBaseState
{
    public CharWallrunState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the wall run specific logic
    public override void EnterState()
    {
        InitializeSubState();

        Ctx.Rb.useGravity = false;
        Ctx.IsWallRunning = true;

        Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0f, Ctx.Rb.velocity.z);

        Ctx.PlayerAnimator.SetBool("IsWallRunning", Ctx.IsWallRunning);
        Ctx.PlayerAnimator.SetFloat("WallRun", Ctx.WallLeftRight);

        if (Ctx.CurrentWall != null)
        {
            Ctx.CurrentWall = Ctx.WallLeft ? Ctx.LeftWallHit.transform : Ctx.WallRight ? Ctx.RightWallHit.transform : null;
            if (Ctx.CurrentWall != Ctx.PreviousWall)
            {
                // Regain a GrappleHooks when wall running on a new wall
                Ctx.GrappleHooks = 1;

                // Resets WallClingTime when wall running on a new wall
                Ctx.WallClingTime = Ctx.MaxWallClingTime;
                Ctx.CanStartWallTimer = true;
            }
            else
            {
                Ctx.CanStartWallTimer = true;
            }
        }
        else
        {
            Ctx.CurrentWall = Ctx.WallLeft ? Ctx.LeftWallHit.transform : Ctx.WallRight ? Ctx.RightWallHit.transform : null;
            Ctx.CanStartWallTimer = true;
        }
    }


    // Makes sure everything is set up when changing from the wall run state
    public override void ExitState()
    {
        Ctx.IsWallRunning = false;

        Ctx.PlayerAnimator.SetBool("WallRunningL", false);
        Ctx.PlayerAnimator.SetBool("WallRunningR", false);

        Ctx.PlayerAnimator.SetBool("IsWallRunning", Ctx.IsWallRunning);
        Ctx.Rb.useGravity = true;
        Ctx.PreviousWall = Ctx.CurrentWall;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        Ctx.WallRunDownForce = 10 - Ctx.MoveForce;

        // Slows down the wall running and makes the player move down the wall when the speed is too low or the wall cling time is below 0
        if (Ctx.WallClingTime <= 0 || Ctx.MovementSpeed <= 2)
        {
            Ctx.Rb.AddForce(Vector3.down * Ctx.WallRunDownForce, ForceMode.Force);
            Ctx.DesiredMoveForce = 0f;
            Ctx.MoveMultiplier = 0.5f;
        }
        else
        {
            Ctx.DesiredMoveForce = Ctx.WallRunSpeed;
        }
    }

    public override void LateUpdateState() { }

    public override void FixedUpdateState()
    {
        CheckSwitchStates();
        WallRunMovement();
    }

    #endregion

    // Initialize sub states for the wall run state
    public override void InitializeSubState()
    {
        if (Ctx.IsMove)
        {
            SetSubState(Factory.Walk());
        }
    }

    // Check if the state can be switched specific for the wall run state
    public override void CheckSwitchStates()
    {
        if (!Ctx.IsMove || !Ctx.IsWalled)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJump)
        {
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    // Logic for the wall run movement based on walls and movement direction
    private void WallRunMovement()
    {
        if (Ctx.Rb.velocity.y > 0)
        {
            Ctx.Rb.velocity = new Vector3(Ctx.Rb.velocity.x, 0f, Ctx.Rb.velocity.z);
        }

        if ((Ctx.PlayerObj.forward - Ctx.WallForward).magnitude > (Ctx.PlayerObj.forward - -Ctx.WallForward).magnitude)
        {
            Ctx.WallForward = new Vector3(-Ctx.WallForward.x, -Ctx.WallForward.y, -Ctx.WallForward.z).normalized;
        }

        // Makes the player cling to the wall usefull for moving walls
        if (!Ctx.IsExitingSlope)
        {
            Ctx.Rb.AddForce(-Ctx.WallNormal.normalized * 225, ForceMode.Force);
        }

        // Changes the JumpMent based on the wall 
        Ctx.JumpMent = new Vector3(Ctx.WallNormal.x * 3, 1, Ctx.WallNormal.z * 3);

        // Changes the Movment based on the wall
        Ctx.Movement = new Vector3(Ctx.WallForward.x, 0, Ctx.WallForward.z).normalized;
        Ctx.MoveMultiplier = 2f;
    }
}