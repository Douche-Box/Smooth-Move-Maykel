using UnityEngine;

public class CharVaultState : CharBaseState
{
    public CharVaultState(CharStateMachine currentContext, CharStateFactory charachterStateFactory) : base(currentContext, charachterStateFactory)
    {
        // Makes the state be able to have sub states for state hierarchy
        IsRootState = true;
    }

    // Setup for the vault specific logic
    public override void EnterState()
    {
        Ctx.IsVaulted = true;
        Ctx.PlayerAnimator.SetTrigger("Vault");
    }

    // Makes sure everything is set up when changing from the vault state
    public override void ExitState()
    {
        Ctx.IsVaulted = false;
    }

    #region MonoBehaveiours

    public override void UpdateState()
    {
        CheckSwitchStates();
        HandleSmoothPosition();
    }

    public override void FixedUpdateState() { }

    public override void LateUpdateState() { }

    #endregion

    public override void InitializeSubState() { }

    // Check if the state can be switched specific for the vault state
    public override void CheckSwitchStates()
    {
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    // Changes the position of the player over the vault smoothly
    private void HandleSmoothPosition()
    {
        float yOffset = Ctx.VaultObj.GetComponent<Renderer>().bounds.max.y + 1f;
        float xOffset = Mathf.Abs(Ctx.transform.forward.x) > Mathf.Abs(Ctx.transform.forward.z) ? (Ctx.VaultObj.transform.position.x - Ctx.transform.position.x) : 0f;
        float zOffset = Mathf.Abs(Ctx.transform.forward.z) > Mathf.Abs(Ctx.transform.forward.x) ? (Ctx.VaultObj.transform.position.z - Ctx.transform.position.z) : 0f;

        Vector3 newPosition = new Vector3(Ctx.transform.position.x + xOffset, yOffset, Ctx.transform.position.z + zOffset);

        Ctx.transform.position = Vector3.Slerp(Ctx.transform.position, newPosition, 1);
    }
}