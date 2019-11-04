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

    public void ConsiderGrounded(bool flag) {
        animalPlayer.Ground(flag);
    }

    private void Move() {
        bool movement = playerMovement.IsWalking;
        animalPlayer.Walk(movement);

        if (movement) {
            animalPlayer.Run(Input.GetKey(KeyCode.LeftShift)); ///TEMP binding
            animalPlayer.Creep(Input.GetMouseButton(1)); ///TEMP binding
        }
        else CancelMovement();
    }

    private void CancelMovement() {
        animalPlayer.Walk(false);
        animalPlayer.Run(false);
        animalPlayer.Creep(false);
    }

    private void Jump() {
        if (playerMovement.IsGrounded && !MovementLocked && !JumpLocked && Input.GetMouseButtonDown(2)) {
            animalPlayer.Jump(); ///TEMP binding
            playerMovement.Jump();
        }
    }

    private void Attack() {
        if (Input.GetMouseButtonDown(0)) animalPlayer.Attack(); ///TEMP binding
    }
}