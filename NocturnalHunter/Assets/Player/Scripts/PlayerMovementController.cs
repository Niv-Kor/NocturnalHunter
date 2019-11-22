using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Tooltip("The camera rig object.")]
    [SerializeField] private GameObject cameraRig;

    private static readonly string HORIZONTAL_AXIS = "Horizontal";
    private static readonly string VERTICAL_AXIS = "Vertical";

    private PlayerStateController playerControl;
    private RigidbodyMovement rigidbodyMovement;
    private Balance balance;
    private float lastMovement;
    
    private void Start() {
        this.rigidbodyMovement = GetComponent<RigidbodyMovement>();
        this.playerControl = GetComponent<PlayerStateController>();
        this.balance = GetComponent<Balance>();
        this.lastMovement = 0;
    }

    private void Update() {
        //get input from user
        float horInput = Input.GetAxis(HORIZONTAL_AXIS);
        float verInput = Input.GetAxis(VERTICAL_AXIS);
        float currentMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
        bool onMovement = currentMovement > lastMovement;
        float inputVolume = onMovement ? 1 : Mathf.Max(Mathf.Abs(verInput), Mathf.Abs(horInput));

        //calculate the movement speed multipliers
        if (!playerControl.MovementLocked) {
            bool running = Input.GetKey(KeyCode.LeftShift);
            bool creeping = Input.GetMouseButton(1);
            rigidbodyMovement.ApplySpeedMultiplier(RigidbodyMovement.SpeedMultiplier.Run, running);
            rigidbodyMovement.ApplySpeedMultiplier(RigidbodyMovement.SpeedMultiplier.Creep, creeping);
        }

        //move to the desired input direction
        Vector3 groundNormal = balance.AverageGroundNormal;
        Vector3 forward = Vector3.Cross(groundNormal, -cameraRig.transform.right);
        Vector3 right = Vector3.Cross(groundNormal, cameraRig.transform.forward);
        Vector3 moveDirection = Vector3.Normalize(forward * verInput + right * horInput);
        
        //move
        if (inputVolume > 0) rigidbodyMovement.Move(moveDirection * inputVolume);

        //save last movement for later use
        lastMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
    }
}