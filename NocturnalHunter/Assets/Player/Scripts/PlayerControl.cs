using UnityEngine;

public class PlayerControl : MonoBehaviour
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

    private StateMachine stateMachine;
    private RigidbodyPlayerMovement playerMovement;
    private float shortIdleTimer, longIdleTimer;
    private float randomShortIdleTime, randomLongIdleTime;

    public bool MovementLocked {
        get { return stateMachine.MovementLocked; }
        set { stateMachine.MovementLocked = value; }
    }

    public bool JumpLocked {
        get { return stateMachine.JumpLocked; }
        set { stateMachine.JumpLocked = value; }
    }

    private void Start() {
        this.stateMachine = GetComponent<StateMachine>();
        this.playerMovement = transform.parent.GetComponent<RigidbodyPlayerMovement>();
        ResetIdleTimers();
    }

    private void Update() {
        ManageIdling();

        if (playerMovement.IsGrounded) {
            bool jump = Jump();
            bool move = Move();
            bool attack = Attack();

            //cancel idling
            if (jump || move || attack) {
                stateMachine.Animate(StateMachine.AnimationType.ShortIdle, false);
                stateMachine.Animate(StateMachine.AnimationType.LongIdle, false);
            }
        }
        else CancelMovement();

        ///TEMP //if (Input.GetKeyDown(KeyCode.Space)) GetHit(); ///TEMP hit trigger
    }

    /// <summary>
    /// Consider the avatar as grounded or in mid air.
    /// </summary>
    /// <param name="flag">True to consider grounded</param>
    public void ConsiderGrounded(bool flag) {
        stateMachine.Ground(flag);
    }

    /// <summary>
    /// Animate the 'Movement' layer of the state machine.
    /// </summary>
    /// <returns>True if an animation was applied.</returns>
    private bool Move() {
        bool movement = playerMovement.IsWalking;
        stateMachine.Animate(StateMachine.AnimationType.Walk, movement);

        if (movement) {
            stateMachine.Animate(StateMachine.AnimationType.Run, Input.GetKey(KeyCode.LeftShift)); ///TEMP binding
            stateMachine.Animate(StateMachine.AnimationType.Creep, Input.GetMouseButton(1)); ///TEMP binding
        }
        else CancelMovement();

        return movement;
    }

    /// <summary>
    /// Animate the 'Jump' layer of the state machine.
    /// </summary>
    /// <returns>True if an animation was applied.</returns>
    private bool Jump() {
        if (playerMovement.IsGrounded && !MovementLocked && !JumpLocked && Input.GetMouseButtonDown(2)) {
            stateMachine.Animate(StateMachine.AnimationType.Jump, true); ///TEMP binding
            playerMovement.Jump();
            return true;
        }

        return false;
    }

    /// <summary>
    /// Animate the 'Attack' layer of the state machine.
    /// </summary>
    /// <returns>True if an animation was applied.</returns>
    private bool Attack() {
        if (Input.GetMouseButtonDown(0) && !stateMachine.IsAnimating(StateMachine.AnimationType.Attack)) {
            stateMachine.Animate(StateMachine.AnimationType.Attack, true); ///TEMP binding
            return true;
        }

        return false;
    }

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

    private void ResetIdleTimers() {
        shortIdleTimer = 0;
        longIdleTimer = 0;
        randomShortIdleTime = UnityEngine.Random.Range(minShortIdleTime, maxShortIdleTime);
        randomLongIdleTime = UnityEngine.Random.Range(minLongIdleTime, maxLongIdleTime);
    }

    /// <summary>
    /// Animate the 'Hit' layer of the state machine.
    /// </summary>
    public void GetHit() {
        stateMachine.Animate(StateMachine.AnimationType.Hit, true);
    }

    /// <summary>
    /// Cancel all movement animations and return to idle states.
    /// </summary>
    private void CancelMovement() {
        stateMachine.Animate(StateMachine.AnimationType.Walk, false);
        stateMachine.Animate(StateMachine.AnimationType.Creep, false);
        stateMachine.Animate(StateMachine.AnimationType.Run, false);
    }
}