using UnityEngine;

public abstract class AnimalPlayer : MonoBehaviour
{
    protected enum AnimationType {
        Idle, Walk, Run, Jump, Morale, Die, Attack
    }

    protected class TemporalMotion
    {
        private AnimationType animationType;
        private string parameterName;

        public AnimationType Type { get { return animationType; } set { } }

        public string Parameter { get { return parameterName; } set { } }

        public TemporalMotion(AnimationType type, string param) {
            this.animationType = type;
            this.parameterName = (param != null) ? param : "";
        }
    }

    protected class AnimationConstraints
    {
        private string[] parameters;
        private bool immediateAction;

        public string RandomParameter { get { return Randomize(parameters); } set { } }

        public bool ImmediateAction { get { return immediateAction; } set { } }

        public AnimationConstraints(string[] parameters, bool immediate) {
            this.parameters = parameters;
            this.immediateAction = immediate;
        }

        /// <summary>
        /// Select a random string from within an array.
        /// </summary>
        /// <param name="arr">The array to select from</param>
        /// <returns>A random string from the array.</returns>
        protected string Randomize(string[] arr) {
            return arr[Random.Range(0, arr.Length)];
        }
    }

    public static readonly string TRANSITION_STATE = "transition";

    protected Animator animator;
    protected TemporalMotion currentMotion;

    private void Start() {
        this.animator = GetComponent<Animator>();
        Idle();
    }

    /// <summary>
    /// Activate 'Walk' animation.
    /// </summary>
    public void Walk() { PlayAnimation(AnimationType.Walk, WalkConstraints()); }

    /// <summary>
    /// Activate 'Run' animation.
    /// </summary>
    public void Run() { PlayAnimation(AnimationType.Run, RunConstraints()); }

    /// <summary>
    /// Activate 'Jump' animation.
    /// </summary>
    public void Jump() { PlayAnimation(AnimationType.Jump, JumpConstraints()); }

    /// <summary>
    /// Activate 'Morale' animation.
    /// </summary>
    public void Morale() { PlayAnimation(AnimationType.Morale, MoraleConstraints()); }

    /// <summary>
    /// Activate 'Attack' animation.
    /// </summary>
    public void Attack() { PlayAnimation(AnimationType.Attack, AttackConstraints()); }

    /// <summary>
    /// Activate 'Die' animation.
    /// </summary>
    public void Die() { PlayAnimation(AnimationType.Die, DieConstraints()); }

    /// <summary>
    /// Activate 'Idle' animation.
    /// </summary>
    public void Idle() {
        currentMotion = new TemporalMotion(AnimationType.Idle, null);

        foreach (AnimatorControllerParameter param in animator.parameters)
            animator.SetBool(param.name, false);
    }

    /// <summary>
    /// Play a certain animation of the avatar.
    /// </summary>
    protected void PlayAnimation(AnimationType type, AnimationConstraints constraints) {
        //new animtaion
        if (currentMotion.Type != type) {
            string parameterName = constraints.RandomParameter;
            currentMotion = new TemporalMotion(type, parameterName);
        }

        //repeat animation
        animator.SetBool(currentMotion.Parameter, true);
    }

    /// <returns>all constraints of the 'Walk' animation</returns>
    protected abstract AnimationConstraints WalkConstraints();

    /// <returns>all constraints of the 'Run' animation</returns>
    protected abstract AnimationConstraints RunConstraints();

    /// <returns>all constraints of the 'Attack' animation</returns>
    protected abstract AnimationConstraints AttackConstraints();

    /// <returns>all constraints of the 'Morale' animation</returns>
    protected abstract AnimationConstraints MoraleConstraints();

    /// <returns>all constraints of the 'Die' animation</returns>
    protected abstract AnimationConstraints DieConstraints();

    /// <returns>all constraints of the 'Jump' animation</returns>
    protected abstract AnimationConstraints JumpConstraints();
}