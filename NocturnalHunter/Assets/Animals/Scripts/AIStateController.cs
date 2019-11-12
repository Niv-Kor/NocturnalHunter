public class AIStateController : StateController
{
    public enum AIMovementMode {
        Walk, Creep, Run
    }

    private AIMovementMode movementMode;
    private bool requestJump, requestAttack;

    protected override void Start() {
        base.Start();
        this.movementMode = AIMovementMode.Walk;
        this.requestJump = false;
        this.requestAttack = false;
    }

    /// <summary>
    /// Consider the animal moving in the specified mode.
    /// If the animal is not physically walking, this request doesn't have an effect.
    /// </summary>
    /// <param name="mode"></param>
    public void RequestMovement(AIMovementMode mode) { movementMode = mode; }

    /// <summary>
    /// Request a jump animation.
    /// If the conditions are not suitable for a jump, nothing happens.
    /// </summary>
    public void RequestJump() { requestJump = true; }

    /// <summary>
    /// Request an attacking animation.
    /// If the conditions are not suitable for an attack, nothing happens.
    /// </summary>
    public void RequestAttack() { requestAttack = true; }

    protected override void Move() {
        bool isCreeping = movementMode == AIMovementMode.Creep;
        bool isRunning = movementMode == AIMovementMode.Run;

        stateMachine.Animate(StateMachine.AnimationType.Walk, true);
        stateMachine.Animate(StateMachine.AnimationType.Run, isRunning);
        stateMachine.Animate(StateMachine.AnimationType.Creep, isCreeping);
    }

    protected override bool Jump() {
        if (requestJump && rigidbodyMovement.IsGrounded && !MovementLocked && !JumpLocked) {
            stateMachine.Animate(StateMachine.AnimationType.Jump, true);
            rigidbodyMovement.Jump();
            return true;
        }

        requestJump = false;
        return false;
    }

    protected override bool Attack() {
        if (requestAttack && !stateMachine.IsAnimating(StateMachine.AnimationType.Attack)) {
            stateMachine.Animate(StateMachine.AnimationType.Attack, true);
            return true;
        }

        requestAttack = false;
        return false;
    }
}