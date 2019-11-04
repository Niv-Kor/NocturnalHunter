using UnityEngine;

public abstract class AnimalPlayer : MonoBehaviour
{
    protected enum AnimationType {
        Idle, Walk, Creeping, Run, Jump, Morale, Hit, Die, Attack
    }

    protected enum TriggerType {
        Boolean, OnTrigger
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

    public static readonly string MOVE_LOCK_PARAM = "move_lock";
    public static readonly string JUMP_LOCK_PARAM = "jump_lock";
    public static readonly string GROUNDED_PARAM = "grounded";

    protected Animator animator;
    protected TemporalMotion currentMotion;

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
    public void Walk(bool flag) {
        PlayAnimation(WalkParameter(),TriggerType.Boolean, flag);
    }

    /// <summary>
    /// Activate 'Run' animation.
    /// </summary>
    public void Run(bool flag) {
        PlayAnimation(RunParameter(), TriggerType.Boolean, flag);
    }

    /// <summary>
    /// Activate 'Creep' animation.
    /// </summary>
    public void Creep(bool flag) {
        PlayAnimation(CreepParameter(), TriggerType.Boolean, flag);
    }

    /// <summary>
    /// Activate 'Jump' animation.
    /// </summary>
    public void Jump() {
        PlayAnimation(JumpParameter(), TriggerType.OnTrigger, true);
    }

    /// <summary>
    /// Activate 'Morale' animation.
    /// </summary>
    public void Morale() {
        PlayAnimation(MoraleParameters());
    }

    /// <summary>
    /// Activate 'Attack' animation.
    /// </summary>
    public void Attack() {
        PlayAnimation(AttackParameters()); 
    }

    /// <summary>
    /// Activate 'Hit' animation.
    /// </summary>
    public void Hit() {
        PlayAnimation(HitParameters());
    }

    /// <summary>
    /// Activate 'Die' animation.
    /// </summary>
    public void Die() {
        PlayAnimation(DieParameter(), TriggerType.OnTrigger, true);
    }

    public void Ground(bool flag) {
        animator.SetBool(GROUNDED_PARAM, flag);
    }

    protected void PlayAnimation(string[] param) {
        PlayAnimation(Randomize(param), TriggerType.OnTrigger, true);
    }

    protected void PlayAnimation(string param, TriggerType trigger, bool flag) {
        switch (trigger) {
            case TriggerType.Boolean: animator.SetBool(param, flag); break;
            case TriggerType.OnTrigger: animator.SetTrigger(param); break;
        }
    }

    /// <summary>
    /// Select a random string from within an array.
    /// </summary>
    /// <param name="arr">The array to select from</param>
    /// <returns>A random string from the array.</returns>
    protected string Randomize(string[] arr) {
        return arr[Random.Range(0, arr.Length)];
    }

    /// <summary>
    /// Play a certain animation of the avatar.
    /// </summary>
    /*protected void PlayAnimation(AnimationType type, AnimationConstraints constraints) {
        //new animtaion
        if (currentMotion.Type != type) {
            string parameterName = constraints.RandomParameter;
            currentMotion = new TemporalMotion(type, parameterName);
        }

        //repeat animation
        animator.SetBool(currentMotion.Parameter, true);
    }*/

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