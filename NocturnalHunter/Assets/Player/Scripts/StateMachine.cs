using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [Header("Singular Essential Parameters")]

    [Tooltip("Parameter for the 'Walk' animation.")]
    [SerializeField] private string walk;

    [Tooltip("Parameter for the 'Creep' animation.")]
    [SerializeField] private string creep;

    [Tooltip("Parameter for the 'Run' animation.")]
    [SerializeField] private string run;

    [Tooltip("Parameter for the 'Jump' animation.")]
    [SerializeField] private string jump;

    [Tooltip("Parameter for the 'Die' animation.")]
    [SerializeField] private string die;

    [Header("Multiple Inessential Parameters")]

    [Tooltip("All parameters that lead to a 'Morale' type animation.")]
    [SerializeField] private string[] morale;

    [Tooltip("All parameters that lead to a 'Hit' type animation,\n"
           + "as for when the player gets hit.")]
    [SerializeField] private string[] hit;

    [Tooltip("All parameters that lead to an 'Attack' type animation,\n"
           + "as for then the player attacks.")]
    [SerializeField] private string[] attack;

    [Tooltip("All parameters that lead to a short 'Idle' type animation.\n"
           + "Short period idle animations are tiny movements of the player while waiting for a command.")]
    [SerializeField] private string[] shortIdle;

    [Tooltip("All parameters that lead to a long 'Idle' type animation.\n"
           + "Long period idle animations are ones that the player is making "
           + "after waiting for a command for a very long time.")]
    [SerializeField] private string[] longIdle;

    public enum AnimationType {
        Idle, ShortIdle, LongIdle, Walk, Creep, Run, Jump, Morale, Hit, Die, Attack
    }

    public static readonly string MOVE_LOCK_PARAM = "move_lock";
    public static readonly string JUMP_LOCK_PARAM = "jump_lock";
    public static readonly string GROUNDED_PARAM = "grounded";

    protected Animator animator;

    public bool MovementLocked {
        get { return animator.GetBool(MOVE_LOCK_PARAM); }
        set { animator.SetBool(MOVE_LOCK_PARAM, value); }
    }

    public bool JumpLocked {
        get { return animator.GetBool(JUMP_LOCK_PARAM); }
        set { animator.SetBool(JUMP_LOCK_PARAM, value); }
    }

    private void Start() {
        this.animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Activate an animation.
    /// </summary>
    /// <param name="type">The type of the animation</param>
    /// <param name="flag">True to activate or false to deactivate</param>
    public void Animate(AnimationType type, bool flag) {
        switch (type) {
            case AnimationType.Walk: PlayAnimation(walk, flag); break;
            case AnimationType.Creep: PlayAnimation(creep, flag); break;
            case AnimationType.Run: PlayAnimation(run, flag); break;
            case AnimationType.Jump: PlayAnimation(jump, flag); break;
            case AnimationType.Morale: PlayAnimation(morale, flag); break;
            case AnimationType.Hit: PlayAnimation(hit, flag); break;
            case AnimationType.Die: PlayAnimation(die, flag); break;
            case AnimationType.Attack: PlayAnimation(attack, flag); break;
            case AnimationType.ShortIdle: PlayAnimation(shortIdle, flag); break;
            case AnimationType.LongIdle: PlayAnimation(longIdle, flag); break;
        }
    }

    /// <summary>
    /// Consider the avatar as grounded or in mid air.
    /// </summary>
    /// <param name="flag">True to consider grounded</param>
    public void Ground(bool flag) {
        animator.SetBool(GROUNDED_PARAM, flag);
    }

    /// <summary>
    /// Check if the avatar is animated according to a certain type of animation.
    /// </summary>
    /// <param name="type">The type of the animation to check</param>
    /// <returns>True if the avatar is currently animated that way.</returns>
    public bool IsAnimating(AnimationType type) {
        switch (type) {
            case AnimationType.Idle: return IsIdling();
            case AnimationType.Walk: return IsAnimating(walk);
            case AnimationType.Creep: return IsAnimating(creep);
            case AnimationType.Run: return IsAnimating(run);
            case AnimationType.Jump: return IsAnimating(jump);
            case AnimationType.Morale: return IsAnimating(morale);
            case AnimationType.Hit: return IsAnimating(hit);
            case AnimationType.Die: return IsAnimating(die);
            case AnimationType.Attack: return IsAnimating(attack);
            case AnimationType.ShortIdle: return IsAnimating(shortIdle);
            case AnimationType.LongIdle: return IsAnimating(longIdle);
            default: return false;
        }
    }

    /// <summary>
    /// Check if the avatar is animated according to a certain array of parameters.
    /// </summary>
    /// <param name="param">The parameters to check</param>
    /// <returns>
    /// True if the avatar is currently animated
    /// according to one of the parameters in the array.
    /// </returns>
    private bool IsAnimating(string[] param) {
        foreach (string parameter in param)
            if (IsAnimating(parameter)) return true;

        return false;
    }

    /// <summary>
    /// Check if the avatar is animated according to a certain parameter.
    /// </summary>
    /// <param name="param">The parameter to check</param>
    /// <returns>True if the avatar is currently animated that way.</returns>
    private bool IsAnimating(string param) {
        return animator.GetBool(param);
    }

    /// <returns>True if the avatar is currently in any of the 'Idle' states.</returns>
    private bool IsIdling() {
        foreach (AnimationType type in Enum.GetValues(typeof(AnimationType))) {
            if (type == AnimationType.Idle ||
                type == AnimationType.ShortIdle ||
                type == AnimationType.LongIdle)
                continue;

            else if (IsAnimating(type)) return false;
        }

        return true;
    }

    /// <summary>
    /// Move the avatar to a certain state to play its animation.
    /// </summary>
    /// <param name="param">An array of parameters, from which only one is chosen randomly</param>
    /// <param name="flag">True to play the animation or false to stop it</param>
    protected void PlayAnimation(string[] param, bool flag) {
        if (flag) PlayAnimation(Randomize(param), true);
        else
            foreach (string parameter in param)
                PlayAnimation(parameter, false);
    }

    /// <summary>
    /// Move the avatar to a certain state to play its animation.
    /// </summary>
    /// <param name="param">An array of parameters, from which only one is chosen randomly</param>
    /// <param name="flag">True to play the animation or false to stop it</param>
    protected void PlayAnimation(string param, bool flag) {
        if (param != null && param != "") animator.SetBool(param, flag);
    }

    /// <summary>
    /// Select a random string from within an array.
    /// </summary>
    /// <param name="arr">The array to select from</param>
    /// <returns>A random string from the array.</returns>
    protected string Randomize(string[] arr) {
        if (arr.Length == 0) return null;
        else return arr[UnityEngine.Random.Range(0, arr.Length)];
    }
}