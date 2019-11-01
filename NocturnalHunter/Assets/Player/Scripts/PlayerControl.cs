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
        Move(playerMovement.IsWalking());
    }

    private void Move(bool flag) {
        if (flag) {
            if (Input.GetKey(KeyCode.LeftShift)) animalPlayer.Run();
            else animalPlayer.Walk();
        }
        else animalPlayer.Idle();
    }
}