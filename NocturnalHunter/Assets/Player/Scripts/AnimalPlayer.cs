using UnityEngine;

public abstract class AnimalPlayer : MonoBehaviour
{
    protected Animator animator;
    protected string currentAnimation;

    private void Start() {
        this.animator = GetComponent<Animator>();
    }

    public void Walk() {
        PlayAnimation(Randomize(WalkParameters()));
    }

    public void Run() {
        PlayAnimation(Randomize(RunParameters()));
    }

    public void Morale() {
        PlayAnimation(Randomize(MoraleParameters()));
    }

    public void Attack() {
        PlayAnimation(Randomize(AttackParameters()));
    }

    public void Die() {
        PlayAnimation(Randomize(DieParameters()));
    }

    public void Idle() {
        if (currentAnimation != null) animator.SetBool(currentAnimation, false);
    }

    protected string Randomize(string[] arr) {
        return arr[Random.Range(0, arr.Length)];
    }

    protected void PlayAnimation(string paramName) {
        if (!animator.GetBool(paramName)) {
            Idle();
            currentAnimation = paramName;
            animator.SetBool(paramName, true);
        }
    }

    protected abstract string[] WalkParameters();
    protected abstract string[] RunParameters();
    protected abstract string[] AttackParameters();
    protected abstract string[] MoraleParameters();
    protected abstract string[] DieParameters();
}