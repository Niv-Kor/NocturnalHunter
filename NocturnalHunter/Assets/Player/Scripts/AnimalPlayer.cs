using System;
using UnityEngine;

public abstract class AnimalPlayer : MonoBehaviour
{
    public enum AnimationType {
        Idle, Walk, Creep, Run, Jump, Morale, Hit, Die, Attack
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
    /// Activate 'Walk' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Walk(bool flag) {
        PlayAnimation(WalkParameter(), flag);
    }

    /// <summary>
    /// Activate 'Run' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Run(bool flag) {
        PlayAnimation(RunParameter(), flag);
    }

    /// <summary>
    /// Activate 'Creep' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Creep(bool flag) {
        PlayAnimation(CreepParameter(), flag);
    }

    /// <summary>
    /// Activate 'Jump' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Jump(bool flag) {
        PlayAnimation(JumpParameter(), flag);
    }

    /// <summary>
    /// Activate 'Morale' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Morale(bool flag) {
        PlayAnimation(MoraleParameters(), flag);
    }

    /// <summary>
    /// Activate 'Attack' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Attack(bool flag) {
        PlayAnimation(AttackParameters(), flag); 
    }

    /// <summary>
    /// Activate 'Hit' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Hit(bool flag) {
        PlayAnimation(HitParameters(), flag);
    }

    /// <summary>
    /// Activate 'Die' animation.
    /// </summary>
    /// <param name="flag">True to play the animation, or false to stop it</param>
    public void Die(bool flag) {
        PlayAnimation(DieParameter(), flag);
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
            case AnimationType.Walk: return IsAnimating(WalkParameter());
            case AnimationType.Creep: return IsAnimating(CreepParameter());
            case AnimationType.Run: return IsAnimating(RunParameter());
            case AnimationType.Jump: return IsAnimating(JumpParameter());
            case AnimationType.Morale: return IsAnimating(MoraleParameters());
            case AnimationType.Hit: return IsAnimating(HitParameters());
            case AnimationType.Die: return IsAnimating(DieParameter());
            case AnimationType.Attack: return IsAnimating(AttackParameters());
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
            if (type == AnimationType.Idle) continue;
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
        animator.SetBool(param, flag);
    }

    /// <summary>
    /// Select a random string from within an array.
    /// </summary>
    /// <param name="arr">The array to select from</param>
    /// <returns>A random string from the array.</returns>
    protected string Randomize(string[] arr) {
        return arr[UnityEngine.Random.Range(0, arr.Length)];
    }

    /// <returns>The paramete of the 'Walk' animation</returns>
    protected abstract string WalkParameter();

    /// <returns>The parameter of the 'Creep' animation</returns>
    protected abstract string CreepParameter();

    /// <returns>The parametes of the 'Run' animation</returns>
    protected abstract string RunParameter();

    /// <returns>The paramete of the 'Die' animation</returns>
    protected abstract string DieParameter();

    /// <returns>The paramete of the 'Jump' animation</returns>
    protected abstract string JumpParameter();

    /// <returns>All parameters of the 'Attack' animation</returns>
    protected abstract string[] AttackParameters();

    /// <returns>All parameters of the 'Morale' animation</returns>
    protected abstract string[] MoraleParameters();

    /// <returns>All parameters of the 'Hit' animation</returns>
    protected abstract string[] HitParameters();

    /// <returns>All parameters of special 'Idle' animations</returns>
    protected abstract string[] SpecialIdleParameters();
}