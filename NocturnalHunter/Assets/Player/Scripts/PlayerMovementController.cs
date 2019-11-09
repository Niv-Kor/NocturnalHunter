using UnityEngine;

public class PlayerMovementController : MonoBehaviour
{
    [Tooltip("The base of the cameras.")]
    [SerializeField] private GameObject cameraBase;

    private static readonly string HORIZONTAL_AXIS = "Horizontal";
    private static readonly string VERTICAL_AXIS = "Vertical";

    private Transform parentTransform;
    private PlayerControl playerControl;
    private RigidbodyMovement rigidbodyMovement;
    private AnimalStats animalStats;
    private float lastMovement;
    

    private void Start() {
        this.rigidbodyMovement = GetComponent<RigidbodyMovement>();
        this.playerControl = GetComponent<PlayerControl>();
        this.animalStats = GetComponent<AnimalStats>();
        this.lastMovement = 0;
        this.parentTransform = transform.parent;
        parentTransform.forward = Vector3.zero;
    }

    private void Update() {
        //get input from user
        float horInput = Input.GetAxis(HORIZONTAL_AXIS);
        float verInput = Input.GetAxis(VERTICAL_AXIS);
        float currentMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
        bool onMovement = currentMovement > lastMovement;
        float inputVolume = onMovement ? 1 : Mathf.Max(Mathf.Abs(verInput), Mathf.Abs(horInput));

        //calculate the movement speed
        float movementSpeed = 0;
        if (!playerControl.MovementLocked) {
            if (Input.GetKey(KeyCode.LeftShift))
                movementSpeed = animalStats.walkSpeed * animalStats.runSpeedMultiplier;
            else if (Input.GetMouseButton(1))
                movementSpeed = animalStats.walkSpeed * animalStats.creepSpeedMultiplier;
            else
                movementSpeed = animalStats.walkSpeed;
        }

        //move to the desired input direction
        Vector3 groundNormal = rigidbodyMovement.GroundNormal;
        Vector3 forward = Vector3.Cross(groundNormal, -cameraBase.transform.right);
        Vector3 right = Vector3.Cross(groundNormal, cameraBase.transform.forward);
        Vector3 moveDirection = Vector3.Normalize(forward * verInput + right * horInput);
        
        //move
        if (inputVolume > 0) rigidbodyMovement.Move(moveDirection * inputVolume, movementSpeed);

        //save last movement for later use
        lastMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
    }
}