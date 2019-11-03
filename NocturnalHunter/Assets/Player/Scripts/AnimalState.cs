using UnityEngine;

public class AnimalState : StateMachineBehaviour
{
    [SerializeField] private string[] parameters;

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        foreach (string param in parameters) animator.SetBool(param, false);
    }
}