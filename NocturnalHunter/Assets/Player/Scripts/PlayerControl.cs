using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private AnimalPlayer animalPlayer;
    private RigidbodyPlayerMovement playerMovement;

    public bool MovementLocked {
        get { return animalPlayer.MovementLocked; }
        set { animalPlayer.MovementLocked = value; }
    }

    public bool JumpLocked {
        get { return animalPlayer.JumpLocked; }
        set { animalPlayer.JumpLocked = value; }
    }

    private void Start() {
        this.animalPlayer = GetComponent<AnimalPlayer>();
        this.playerMovement = transform.parent.GetComponent<RigidbodyPlayerMovement>();
    }

    private void Update() {
        if (playerMovement.IsGrounded) {
            Jump();
            Move();
            Attack();
        }
        else CancelMovement();
    }

    /// <summary>
    /// Consider the avatar as grounded or in mid air.
    /// </summary>
    /// <param name="flag">True to consider grounded</param>
    public void ConsiderGrounded(bool flag) {
        animalPlayer.Ground(flag);
    }

    /// <summary>
    /// Animate the 'Movement' layer of the state machine.
    /// </summary>
    private void Move() {
        bool movement = playerMovement.IsWalking;
        animalPlayer.Walk(movement);

        if (movement) {
            animalPlayer.Run(Input.GetKey(KeyCode.LeftShift)); ///TEMP binding
            animalPlayer.Creep(Input.GetMouseButton(1)); ///TEMP binding
        }
        else CancelMovement();
    }

    /// <summary>
    /// Animate the 'Jump' layer of the state machine.
    /// </summary>
    private void Jump() {
        if (playerMovement.IsGrounded && !MovementLocked && !JumpLocked && Input.GetMouseButtonDown(2)) {
            animalPlayer.Jump(true); ///TEMP binding
            playerMovement.Jump();
        }
    }

    /// <summary>
    /// Animate the 'Attack' layer of the state machine.
    /// </summary>
    private void Attack() {
        if (Input.GetMouseButtonDown(0) && !animalPlayer.IsAnimating(AnimalPlayer.AnimationType.Attack))
            animalPlayer.Attack(true); ///TEMP binding
    }

    /// <summary>
    /// Cancel all movement animations and return to idle states.
    /// </summary>
    private void CancelMovement() {
        animalPlayer.Walk(false);
        animalPlayer.Run(false);
        animalPlayer.Creep(false);
    }
}