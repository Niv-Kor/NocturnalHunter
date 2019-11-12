using UnityEngine;

public class PlayerStateController : StateController
{
    [Tooltip("Apply short idle animations while waiting for a command.")]
    [SerializeField] private bool applyShortIdle;

    [Tooltip("Minimum time until the next short idle animation.")]
    [SerializeField] private float minShortIdleTime;

    [Tooltip("Maximum time until the next short idle animation.")]
    [SerializeField] private float maxShortIdleTime;

    [Tooltip("Apply long idle animations while waiting for a command for too long.")]
    [SerializeField] private bool applyLongIdle;

    [Tooltip("Minimum time until the next long idle animation.")]
    [SerializeField] private float minLongIdleTime;

    [Tooltip("Minimum time until the next long idle animation.")]
    [SerializeField] private float maxLongIdleTime;

    private float shortIdleTimer, longIdleTimer;
    private float randomShortIdleTime, randomLongIdleTime;

    protected override void Start() {
        base.Start();
        ResetIdleTimers();
    }

    protected override void Update() {
        base.Update();
        ManageIdling();
    }

    protected override void Move() {
        stateMachine.Animate(StateMachine.AnimationType.Walk, true);
        stateMachine.Animate(StateMachine.AnimationType.Run, Input.GetKey(KeyCode.LeftShift)); ///TEMP binding
        stateMachine.Animate(StateMachine.AnimationType.Creep, Input.GetMouseButton(1)); ///TEMP binding
    }

    protected override bool Jump() {
        if (rigidbodyMovement.IsGrounded && !MovementLocked && !JumpLocked && Input.GetMouseButtonDown(2)) {
            stateMachine.Animate(StateMachine.AnimationType.Jump, true); ///TEMP binding
            rigidbodyMovement.Jump();
            return true;
        }

        return false;
    }

    protected override bool Attack() {
        if (Input.GetMouseButtonDown(0) && !stateMachine.IsAnimating(StateMachine.AnimationType.Attack)) {
            stateMachine.Animate(StateMachine.AnimationType.Attack, true); ///TEMP binding
            return true;
        }

        return false;
    }

    /// <summary>
    /// Perform short or long motions while wating for user input.
    /// </summary>
    private void ManageIdling() {
        bool animateLong = stateMachine.IsAnimating(StateMachine.AnimationType.LongIdle);

        //the player is not doing anything
        if (!animateLong && stateMachine.IsAnimating(StateMachine.AnimationType.Idle)) {
            //check long idle timing
            if (applyLongIdle && longIdleTimer >= randomLongIdleTime) {
                stateMachine.Animate(StateMachine.AnimationType.LongIdle, true);
                longIdleTimer = 0;
            }
            //check short idle timing
            else if (applyShortIdle && shortIdleTimer >= randomShortIdleTime) {
                stateMachine.Animate(StateMachine.AnimationType.ShortIdle, true);
                shortIdleTimer = 0;
            }

            //advance timers 
            shortIdleTimer += Time.deltaTime;
            longIdleTimer += Time.deltaTime;
        }
        else ResetIdleTimers();
    }

    /// <summary>
    /// Set all idling timers to 0 and start counting again.
    /// </summary>
    private void ResetIdleTimers() {
        shortIdleTimer = 0;
        longIdleTimer = 0;
        randomShortIdleTime = UnityEngine.Random.Range(minShortIdleTime, maxShortIdleTime);
        randomLongIdleTime = UnityEngine.Random.Range(minLongIdleTime, maxLongIdleTime);
    }
}