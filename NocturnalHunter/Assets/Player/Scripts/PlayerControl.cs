using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    private AnimalPlayer animalPlayer;
    private RigidbodyPlayerMovement playerMovement;

    private void Start() {
        this.animalPlayer = GetComponent<AnimalPlayer>();
        this.playerMovement = transform.parent.GetComponent<RigidbodyPlayerMovement>();
    }

    private void Update() {
        //jump
        if (playerMovement.InMidAir) animalPlayer.Jump(); //only animate
        else if (Input.GetKey(KeyCode.Space)) playerMovement.Jump(); //perform jump

        //walk
        else if (playerMovement.IsWalking) {
            if (Input.GetKey(KeyCode.LeftShift)) animalPlayer.Run();
            else animalPlayer.Walk();
        }

        //idle
        else if (Input.GetKey(KeyCode.E)) animalPlayer.Morale();
        else if (Input.GetMouseButton(0)) animalPlayer.Attack();
        else animalPlayer.Idle();
    }

    /// <summary>
    /// Activate a movement animation (walk / run).
    /// </summary>
    /// <param name="flag">True if the player is moving / False to stop moving</param>
    private void Move(bool flag) {
        if (flag) {
            
        }
        else animalPlayer.Idle();
    }

    private void Jump() {
        if (playerMovement.InMidAir) animalPlayer.Jump(); //only animate
        else if (Input.GetKey(KeyCode.Space)) playerMovement.Jump(); //perform jump
        else animalPlayer.Idle();
    }
}