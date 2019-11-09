using UnityEngine;

public class LockerKeyState : StateMachineBehaviour
{
    [Tooltip("The parameter to toggle on/off.")]
    [SerializeField] private string lockParam;

    [Tooltip("Check to turn the parameter on when entering this state, and off on exit.\n"
           + "Uncheck to apply the opposite effect.")]
    [SerializeField] private bool lockOnEnter;

    [Tooltip("Lock or unlock the parameter during transition to this state,\n"
           + "right before entering it completely.")]
    [SerializeField] private bool toggleDuringTransition;

    [Tooltip("Lock or unlock the parameter after a fixed percentage of animation time.")]
    [SerializeField] [Range(0, 100f)] private float toggleAfterPercent = 100;

    private float exitTime, exitCounter;
    private bool toggled;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        this.exitCounter = 0;
        this.exitTime = toggleAfterPercent * stateInfo.length / 100;
        if (toggleDuringTransition) LockMovement(animator, lockOnEnter);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (!toggleDuringTransition && !toggled && !animator.IsInTransition(layerIndex)) {
            LockMovement(animator, lockOnEnter);
            toggled = true;
        }

        if (toggleAfterPercent < 100) {
            if (exitCounter >= exitTime) LockMovement(animator, !lockOnEnter);

            exitCounter += Time.deltaTime;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        LockMovement(animator, !lockOnEnter);
        toggled = false;
    }

    /// <summary>
    /// Toggle the parameter on/off.
    /// </summary>
    /// <param name="animator">The state machine object</param>
    /// <param name="flag">True to turn the parameter on, or false otherwise</param>
    private void LockMovement(Animator animator, bool flag) {
        animator.SetBool(lockParam, flag);
    }
}