using Constants;
using UnityEngine;

public class RigidbodyMovement : MonoBehaviour
{
    [Tooltip("The avatars's legs object (containing all its legs as children).")]
    [SerializeField] private GameObject legs;

    private static readonly float SHARP_PITCH_ANGLE = 70;
    private static readonly float CASUAL_ROTATION_RATE = .5f;
    private static readonly float INITIAL_ALIGNMENT_TIME = .5f;
    private static readonly float WALK_AFTERTIME = .1f;

    private Rigidbody rigidBody;
    private GameObject[] feet;
    private Transform parentTransform;
    private AnimalStats animalStats;
    private TerrainGlider terrainGlider;
    private Vector3 turnDirection;
    private float defaultYaw;
    private float initialTurnTimer, jumpTimer, walkTimer;
    private bool rotateTowards, isWalking;
    private bool doubleRotRate, initialTurn;
    private bool requestJump;

    public bool IsWalking {
        get { return isWalking; }
        set { }
    }

    public bool IsRotating {
        get { return rotateTowards; }
        set { }
    }

    public bool IsGrounded { get; set; }

    public Vector3 GroundNormal {
        get { return CalcGroundNormal(); }
        set { }
    }

    private void Start() {
        this.rigidBody = GetComponentInParent<Rigidbody>();
        this.animalStats = GetComponent<AnimalStats>();
        this.terrainGlider = FindObjectOfType<TerrainGlider>();
        this.defaultYaw = transform.eulerAngles.y;
        this.jumpTimer = 0;
        this.parentTransform = transform.parent;
        parentTransform.forward = Vector3.zero;

        this.feet = new GameObject[legs.transform.childCount];
        for (int i = 0; i < feet.Length; i++)
            feet[i] = legs.transform.GetChild(i).gameObject;

        //initial terrain hugging
        this.turnDirection = Vector3.Cross(GroundNormal, -parentTransform.right);
        this.rotateTowards = true;
        this.initialTurnTimer = 0;
    }

    private void Update() {
        CalcGroundNormal();

        //make it easier to jump with 0 gravity drag value
        if (!IsGrounded || requestJump) rigidBody.drag = 0;

        //count until the first turn ends
        if (!initialTurn) initialTurnTimer += Time.deltaTime;

        //decide if the animal is walking now
        if (walkTimer < WALK_AFTERTIME) walkTimer += Time.deltaTime;
        else isWalking = false;

        //make it harder to climb terrain steeps
        if (IsGrounded) terrainGlider.ChangeDragValue(transform, rigidBody, IsWalking);

        Turn();
        CompleteJumpRequest();
    }

    /// <summary>
    /// Move the animal avatar towards a specific direction.
    /// </summary>
    /// <param name="direction">Normalized direction to move towards</param>
    /// <param name="speed">Movement speed</param>
    public void Move(Vector3 direction, float speed) {
        Vector3 groundNormal = GroundNormal;

        //reset walk timer
        walkTimer = 0;
        isWalking = true;

        //compress the turn direction if it's too radical (180 degrees)
        if (IsOppositeDirection(parentTransform.forward, direction)) {
            direction = Quaternion.AngleAxis(90, groundNormal) * direction;
            doubleRotRate = true; //fasten rotation
        }

        //asign correct and compressed value to the next turn direction
        if (!initialTurn) {
            turnDirection = Vector3.Cross(groundNormal, -parentTransform.right);
            rotateTowards = true;
        }
        //first frames of the game
        else {
            turnDirection = direction;
            if (turnDirection != Vector3.zero) rotateTowards = true;
        }

        //apply force against the animal's rigidbody
        if (IsGrounded && AbsoluteSmallerThan(rigidBody.velocity, 10))
            rigidBody.AddForce(direction * speed);
    }

    /// <summary>
    /// Check if a vector's x, y and z absolute values are smaller than a peremeter n (exclusive).
    /// </summary>
    /// <param name="vec">The vector to check</param>
    /// <param name="n">Maximum allowed value (exclusive)</param>
    /// <returns>True is all absolute values of the vetor are smaller than n.</returns>
    private bool AbsoluteSmallerThan(Vector3 vec, float n) {
        float x = Mathf.Abs(vec.x);
        float y = Mathf.Abs(vec.y);
        float z = Mathf.Abs(vec.z);

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
    public void Turn() {
        //cancel rotation
        if (turnDirection == Vector3.zero || !rotateTowards || initialTurnTimer >= INITIAL_ALIGNMENT_TIME) {
            doubleRotRate = false;
            rotateTowards = false;
            initialTurn = true;
            initialTurnTimer = 0;
            return;
        }

        //calculate step size
        Vector3 currentForward = parentTransform.forward;
        Vector3 fullRotation = Vector3.RotateTowards(currentForward, turnDirection, 2, 0);
        Quaternion fullDirection = Quaternion.LookRotation(fullRotation);
        float currentY = parentTransform.eulerAngles.y - defaultYaw;
        float targetY = fullDirection.eulerAngles.y - defaultYaw;
        float angleDistance = Mathf.Abs(Mathf.DeltaAngle(currentY, targetY));
        float rotationPercent = Mathf.Clamp((angleDistance - 10) / (180 - 10) * 100, 0, 100);

        //check if the slope is very sharp
        float pitchAngle = parentTransform.eulerAngles.x;
        pitchAngle = (pitchAngle > 180) ? pitchAngle - 360 : pitchAngle;
        bool slipperyFall = pitchAngle > SHARP_PITCH_ANGLE;
        float rotationRate;

        //rotation is insignificant
        if (rotationPercent < .1f && !slipperyFall) {
            rotationRate = CASUAL_ROTATION_RATE;
            doubleRotRate = false;
        }
        else {
            float rotationDiff = animalStats.maxRotationSpeed - animalStats.minRotationSpeed;
            rotationRate = (rotationPercent * rotationDiff / 100) + animalStats.minRotationSpeed;

            if (doubleRotRate)
                rotationRate = Mathf.Clamp(rotationRate * 2, animalStats.minRotationSpeed,
                                           animalStats.maxRotationSpeed);
        }

        //rotate
        float step = rotationRate * Time.deltaTime;
        Vector3 nextDirection = Vector3.RotateTowards(currentForward, turnDirection, step, 0);
        Quaternion nextRotation = Quaternion.LookRotation(nextDirection);
        parentTransform.rotation = nextRotation;
    }

    /// <summary>
    /// Request a jump.
    /// </summary>
    public void Jump() { requestJump = true; }

    /// <summary>
    /// Complete the jump request made earlier, to fulfill the delay condition.
    /// </summary>
    private void CompleteJumpRequest() {
        if (!requestJump) return;

        if (jumpTimer >= animalStats.jumpDelay) {
            jumpTimer = 0;
            requestJump = false;

            //perform jump
            float steepPercent = Mathf.Abs(terrainGlider.GetSteepPercent(transform, IsWalking));
            float forceDiff = animalStats.maxJumpForce - animalStats.minJumpForce;
            float jumpForce = steepPercent * forceDiff / 100 + animalStats.minJumpForce;
            Vector3 direction = parentTransform.up + (100 - steepPercent) * turnDirection / 100;
            rigidBody.AddForce(direction * jumpForce);
        }

        jumpTimer += Time.deltaTime;
    }

    /// <summary>
    /// Calculate the normal vector of the exact area that the player is currently standing on.
    /// </summary>
    /// <returns>The normal vector of the current area.</returns>
    public Vector3 CalcGroundNormal() {
        RaycastHit[] hits = new RaycastHit[feet.Length];
        Vector3 normalizedHit = Vector3.zero;
        bool feetGrounded = false;

        for (int i = 0; i < hits.Length; i++) {
            Transform footTransform = feet[i].transform;
            Vector3 position = footTransform.position;
            Physics.Raycast(position, -footTransform.up, out hits[i], Layers.GROUND);

            //check if current foot is touching the floor
            Foot footComponent = feet[i].GetComponent<Foot>();
            if (!feetGrounded && footComponent.IsGrounded(Layers.GROUND)) feetGrounded = true;
        }

        //calculate the average value for all of the avatar's legs
        foreach (RaycastHit hit in hits) normalizedHit += hit.normal;

        IsGrounded = feetGrounded; //update IsGrounded public value
        return normalizedHit.normalized;
    }
}