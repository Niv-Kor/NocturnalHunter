using UnityEngine;

public abstract class StateController : MonoBehaviour
{
    protected StateMachine stateMachine;
    protected RigidbodyMovement rigidbodyMovement;

    public virtual bool MovementLocked {
        get { return stateMachine.MovementLocked; }
        set { stateMachine.MovementLocked = value; }
    }

    public virtual bool JumpLocked {
        get { return stateMachine.JumpLocked; }
        set { stateMachine.JumpLocked = value; }
    }

    protected virtual void Start() {
        this.stateMachine = GetComponent<StateMachine>();
        this.rigidbodyMovement = GetComponent<RigidbodyMovement>();
    }

    protected virtual void Update() {
        bool move = false, jump = false, attack = false;

        if (rigidbodyMovement.IsWalking) {
            Move();
            move = true;
        }
        else CancelMovement();

        if (rigidbodyMovement.IsGrounded) {
            jump = Jump();
            attack = Attack();
        }
        //cancel idling
        if (jump || move || attack) {
            stateMachine.Animate(StateMachine.AnimationType.ShortIdle, false);
            stateMachine.Animate(StateMachine.AnimationType.LongIdle, false);
        }
    }

    /// <summary>
    /// Consider the avatar as grounded or in mid air.
    /// </summary>
    /// <param name="flag">True to consider grounded</param>
    public virtual void ConsiderGrounded(bool flag) {
        stateMachine.Ground(flag);
    }

    /// <summary>
    /// Cancel all movement animations and return to idle states.
    /// </summary>
    protected virtual void CancelMovement() {
        stateMachine.Animate(StateMachine.AnimationType.Walk, false);
        stateMachine.Animate(StateMachine.AnimationType.Creep, false);
        stateMachine.Animate(StateMachine.AnimationType.Run, false);
    }

    /// <summary>
    /// Animate the 'Hit' layer of the state machine.
    /// </summary>
    public virtual void GetHit() {
        stateMachine.Animate(StateMachine.AnimationType.Hit, true);
    }

    /// <summary>
    /// Animate the 'Movement' layer of the state machine.
    /// </summary>
    protected abstract void Move();

    /// <summary>
    /// Animate the 'Jump' layer of the state machine.
    /// </summary>
    /// <returns>True if an animation was applied.</returns>
    protected abstract bool Jump();

    /// <summary>
    /// Animate the 'Attack' layer of the state machine.
    /// </summary>
    /// <returns>True if an animation was applied.</returns>
    protected abstract bool Attack();
}