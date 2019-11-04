using UnityEngine;

public class RigidbodyPlayerMovement : MonoBehaviour
{
    [Tooltip("The player's avatar.")]
    [SerializeField] private GameObject avatar;

    [Tooltip("The base of the cameras.")]
    [SerializeField] private GameObject cameraBase;

    [Tooltip("The avatars's legs object (containing all its legs as children).")]
    [SerializeField] private GameObject legs;

    [Tooltip("Layers of objects that can be stepped on.")]
    [SerializeField] private LayerMask groundLayer;

    [Header("Force")]
    
    [Tooltip("The speed of regular walking.")]
    [SerializeField] private float walkSpeed = 15;

    [Tooltip("Creeping speed as a percentage of the current walking speed.")]
    [SerializeField] private float creepingSpeedMultiplier = .5f;

    [Tooltip("The speed of running.")]
    [SerializeField] private float runSpeed = 30;

    [Tooltip("Minimum speed of rotation.")]
    [SerializeField] private float minRotationSpeed = 2;

    [Tooltip("Maximum speed of rotation.")]
    [SerializeField] private float maxRotationSpeed = 12;

    [Tooltip("The minimum force being applied on the player when jumping.")]
    [SerializeField] private float minJumpForce = 50;

    [Tooltip("The maximum force being applied on the player when jumping.")]
    [SerializeField] private float maxJumpForce = 100;

    [Header("Animation Delay")]

    [Tooltip("Delay until a jump occurs.")]
    [SerializeField] private float jumpDelay = 0;

    private static readonly string HORIZONTAL_AXIS = "Horizontal";
    private static readonly string VERTICAL_AXIS = "Vertical";
    private static readonly float SHARP_PITCH_ANGLE = 70;
    private static readonly float CASUAL_ROTATION_RATE = .5f;
    private static readonly float INITIAL_ALIGNMENT_TIME = .5f;

    private Rigidbody rigidBody;
    private GameObject[] feet;
    private TerrainGlider terrainGlider;
    private PlayerControl playerControl;
    private Vector3 turnDirection;
    private float defaultYaw, lastMovement;
    private float initialTurnTimer, jumpTimer;
    private bool rotateTowards, isWalking, grounded;
    private bool doubleRotRate, initialTurn;
    private bool requestJump;

    public bool IsWalking { get { return isWalking; } set { } }

    public bool IsRotating { get { return rotateTowards; } set { } }

    public bool IsGrounded {
        get { return grounded; }
        set {
            playerControl.ConsiderGrounded(value);
            grounded = value;
        }
    }

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.terrainGlider = GetComponent<TerrainGlider>();
        this.playerControl = avatar.GetComponent<PlayerControl>();
        this.defaultYaw = avatar.transform.eulerAngles.y;
        this.lastMovement = 0;
        this.rotateTowards = false;
        this.turnDirection = Vector3.zero;
        this.initialTurnTimer = 0;
        this.jumpTimer = 0;
        this.feet = new GameObject[legs.transform.childCount];

        transform.forward = Vector3.zero;

        for (int i = 0; i < feet.Length; i++)
            feet[i] = legs.transform.GetChild(i).gameObject;
    }

    private void LateUpdate() {
        //get input from user
        float horInput = Input.GetAxis(HORIZONTAL_AXIS);
        float verInput = Input.GetAxis(VERTICAL_AXIS);
        float currentMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
        float inputVolume = (currentMovement > lastMovement) ? 1 : Mathf.Max(Mathf.Abs(verInput), Mathf.Abs(horInput));

        //calculate the movement speed
        float movementSpeed = 0;
        if (!playerControl.MovementLocked) {
            if (Input.GetKey(KeyCode.LeftShift)) movementSpeed = runSpeed; ///TEMP binding
            else if (Input.GetMouseButton(1)) movementSpeed = walkSpeed * creepingSpeedMultiplier; ///TEMP binding
            else movementSpeed = walkSpeed;
        }

        Move(horInput, verInput, inputVolume, movementSpeed);
        Turn(turnDirection);
        CompleteJumpRequest();

        //save movement parameters for later use
        isWalking = (verInput != 0 || horInput != 0) && currentMovement >= lastMovement;
        lastMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));

        if (!IsGrounded || requestJump) rigidBody.drag = 0;
        if (!initialTurn) initialTurnTimer += Time.deltaTime;
    }

    /// <summary>
    /// Request a jump.
    /// </summary>
    public void Jump() { requestJump = true; }

    private void CompleteJumpRequest() {
        if (!requestJump) return;

        if (jumpTimer >= jumpDelay) {
            jumpTimer = 0;
            requestJump = false;

            //perform jump
            float steepPercent = (terrainGlider != null) ? Mathf.Abs(terrainGlider.SteepPercent) : 50;
            float jumpForce = steepPercent * (maxJumpForce - minJumpForce) / 100 + minJumpForce;
            Vector3 direction = transform.up + (100 - steepPercent) * turnDirection / 100;
            rigidBody.AddForce(direction * jumpForce);
        }

        jumpTimer += Time.deltaTime;
    }

    /// <summary>
    /// Move the player towards a specific direction.
    /// </summary>
    /// <param name="x">The x direction (ranged -1 to 1)</param>
    /// <param name="y">The y direction (ranged -1 to 1)</param>
    /// <param name="volume">How hard is the specified direction (ranged 0 to 1)</param>
    /// <param name="speed">The movement speed</param>
    private void Move(float x, float y, float volume, float speed) {
        Vector3 groundNormal = CalcGroundNormal();
        Vector3 forward = Vector3.Cross(groundNormal, -cameraBase.transform.right);
        Vector3 right = Vector3.Cross(groundNormal, cameraBase.transform.forward);

        //compress the turn direction if it's too radical (180 degrees)
        if (IsOppositeDirection(transform.forward, (forward * y + right * x).normalized)) {
            //swap x and y values and compress the direction by 90 degrees
            float temp = x;
            x = y;
            y = temp;

            doubleRotRate = true; //fasten rotation
        }

        //asign correct and compressed value to the next turn direction
        if (!initialTurn) {
            turnDirection = (forward + right).normalized;
            rotateTowards = true;
        }
        else {
            turnDirection = (forward * y + right * x).normalized;
            if (turnDirection != Vector3.zero) rotateTowards = true;
        }

        //move towards the desired direction
        if (grounded && AbsoluteSmallerThan(rigidBody.velocity, 10)) {
            Vector3 moveDirection = (forward * y + right * x).normalized;
            rigidBody.AddForce(moveDirection * volume * speed);
        }
    }

    /// <summary>
    /// Check if a vector's x, y and z absolute values are smaller than a peremeter n (exclusive).
    /// </summary>
    /// <param name="n">Maximum allowed value (exclusive)</param>
    /// <returns>True is all absolute values of the vetor 'a' are smaller than 'n'.</returns>
    private bool AbsoluteSmallerThan(Vector3 a, float n) {
        float x = Mathf.Abs(a.x);
        float y = Mathf.Abs(a.y);
        float z = Mathf.Abs(a.z);

        return x < n && y < n && z < n;
    }

    /// <summary>
    /// Check if one vector's x, y and z values have the opposite sign of the other vector's values.
    /// Each value in vector 'a' has to have the opposite sign to its equivalent in vector 'b'.
    /// </summary>
    /// <param name="a">First vector to check</param>
    /// <param name="b">Second vector to check against</param>
    /// <returns>True if both vectors are opposite to eachother.</returns>
    private bool IsOppositeDirection(Vector3 a, Vector3 b) {
        bool xOppose = a.x == 0 || b.x == 0 || a.x * b.x < 0;
        bool yOppose = a.y == 0 || b.y == 0 || a.y * b.y < 0;
        bool zOppose = a.z == 0 || b.z == 0 || a.z * b.z < 0;
        return xOppose && yOppose && zOppose;
    }

    /// <summary>
    /// Rotate the player towards a specific direction.
    /// </summary>
    /// <param name="direction">The direction the player supposed to be facing</param>
    private void Turn(Vector3 direction) {
        //cancel rotation
        if (direction == Vector3.zero || !rotateTowards || initialTurnTimer >= INITIAL_ALIGNMENT_TIME) {
            doubleRotRate = false;
            rotateTowards = false;
            initialTurn = true;
            initialTurnTimer = 0;
            return;
        }

        //calculate step size
        Vector3 currentForward = transform.forward;
        Vector3 fullRotation = Vector3.RotateTowards(currentForward, direction, 2, 0);
        Quaternion fullDirection = Quaternion.LookRotation(fullRotation);
        float currentY = transform.eulerAngles.y - defaultYaw;
        float targetY = fullDirection.eulerAngles.y - defaultYaw;
        float angleDistance = Mathf.Abs(Mathf.DeltaAngle(currentY, targetY));
        float rotationPercent = Mathf.Clamp((angleDistance - 10) / (180 - 10) * 100, 0, 100);

        //check if the slope is very sharp
        float pitchAngle = transform.eulerAngles.x;
        pitchAngle = (pitchAngle > 180) ? pitchAngle - 360 : pitchAngle;
        bool slipperyFall = pitchAngle > SHARP_PITCH_ANGLE;
        float rotationRate;

        //rotation is insignificant
        if (rotationPercent < .1f && !slipperyFall) {
            rotationRate = CASUAL_ROTATION_RATE;
            doubleRotRate = false;
        }
        else {
            rotationRate = (rotationPercent * (maxRotationSpeed - minRotationSpeed) / 100) + minRotationSpeed;
            if (doubleRotRate) rotationRate = Mathf.Clamp(rotationRate * 2, minRotationSpeed, maxRotationSpeed);
        }

        //rotate
        float step = rotationRate * Time.deltaTime;
        Vector3 nextDirection = Vector3.RotateTowards(currentForward, direction, step, 0);
        Quaternion nextRotation = Quaternion.LookRotation(nextDirection);
        transform.rotation = nextRotation;
    }

    /// <summary>
    /// Calculate the normal vector of the exact area that the player is currently standing on.
    /// </summary>
    /// <returns>The normal vector of the current area.</returns>
    private Vector3 CalcGroundNormal() {
        RaycastHit[] hits = new RaycastHit[feet.Length];
        Vector3 normalizedHit = Vector3.zero;
        bool feetGrounded = false;

        for (int i = 0; i < hits.Length; i++) {
            Transform footTransform = feet[i].transform;
            Vector3 position = footTransform.position;
            Physics.Raycast(position, -footTransform.up, out hits[i], groundLayer);

            //check if current foot is touching the floor
            Foot footComponent = feet[i].GetComponent<Foot>();
            if (!feetGrounded && footComponent.IsGrounded(groundLayer)) feetGrounded = true;
        }

        //calculate the average value for all of the avatar's legs
        foreach (RaycastHit hit in hits) normalizedHit += hit.normal;

        IsGrounded = feetGrounded; //update IsGrounded public value
        return normalizedHit.normalized;
    }
}