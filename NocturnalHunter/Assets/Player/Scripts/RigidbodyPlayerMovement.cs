using UnityEngine;

public class RigidbodyPlayerMovement : MonoBehaviour
{
    private class HardVector
    {
        public static HardVector NONE = new HardVector(0);
        public static HardVector NEGATIVE = new HardVector(-1);
        public static HardVector POSITIVE = new HardVector(1);

        private int vector;

        private HardVector(int vector) {
            this.vector = vector;
        }

        public void Oppose() { vector *= -1; }

        public int Vector {
            get { return vector; }
            set { vector = Mathf.Clamp(value, -1, 1); }
        }

        public static HardVector Harden(float vector) {
            if (vector > 0) return POSITIVE;
            else if (vector < 0) return NEGATIVE;
            else return NONE;
        }
    }

    [SerializeField] private GameObject avatar, cameraBase;
    [SerializeField] private GameObject legs;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float walkSpeed = 15;
    [SerializeField] private float runSpeed = 30;
    [SerializeField] private float minRotation = 2;
    [SerializeField] private float maxRotation = 12;

    private readonly string HORIZONTAL_AXIS = "Horizontal";
    private readonly string VERTICAL_AXIS = "Vertical";
    private readonly float SHARP_PITCH_ANGLE = 30;
    private readonly float CASUAL_ROTATION_RATE = .5f;

    private Rigidbody rigidBody;
    private GameObject[] feet;
    private Vector3 turnDirection;
    private float defaultYaw, lastMovement;
    private bool rotateTowards, isWalking, doubleRotRate;

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.defaultYaw = avatar.transform.eulerAngles.y;
        this.lastMovement = 0;
        this.rotateTowards = false;
        this.turnDirection = Vector3.zero;
        this.feet = new GameObject[legs.transform.childCount];

        for (int i = 0; i < feet.Length; i++)
            feet[i] = legs.transform.GetChild(i).gameObject;
    }

    private void LateUpdate() {
        //get input from user
        float horInput = Input.GetAxis(HORIZONTAL_AXIS);
        float verInput = Input.GetAxis(VERTICAL_AXIS);
        float currentMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
        float inputVolume = (currentMovement > lastMovement) ? 1 : Mathf.Max(Mathf.Abs(verInput), Mathf.Abs(horInput));

        float movementSpeed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
        Move(horInput, verInput, inputVolume, movementSpeed);
        Turn(turnDirection);

        isWalking = (verInput != 0 || horInput != 0) && currentMovement >= lastMovement;
        lastMovement = Mathf.Max(Mathf.Abs(horInput), Mathf.Abs(verInput));
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

        //compress the turn direction if it's too radical (180 degrees) - swap x and y values
        if (IsOppositeDirection(transform.forward, (forward * y + right * x).normalized)) {
            float temp = x;
            x = y;
            y = temp;

            doubleRotRate = true; //fasten rotation
        }

        //asign correct and compressed value to the next turn direction
        turnDirection = (forward * y + right * x).normalized;
        if (turnDirection != Vector3.zero) rotateTowards = true;

        //move towards the desired direction
        if (VelocitySmallerThan(10)) {
            Vector3 moveDirection = (forward * y + right * x).normalized;
            rigidBody.AddForce(moveDirection * volume * speed);
        }
    }

    private bool VelocitySmallerThan(float max) {
        float x = Mathf.Abs(rigidBody.velocity.x);
        float y = Mathf.Abs(rigidBody.velocity.y);
        float z = Mathf.Abs(rigidBody.velocity.z);

        return x < max && y < max && z < max;
    }

    private bool IsOppositeDirection(Vector3 a, Vector3 b) {
        return a.x * b.x < 0 && a.y * b.y < 0 && a.z * b.z < 0;
    }

    /// <summary>
    /// Rotate the player towards a specific direction.
    /// </summary>
    /// <param name="direction">The direction the player supposed to be facing</param>
    private void Turn(Vector3 direction) {
        //cancel rotation
        if (direction == Vector3.zero || !rotateTowards) {
            doubleRotRate = false;
            rotateTowards = false;
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
        float rotationRate;

        //rotation is insignificant
        if (rotationPercent < .1f && Mathf.Abs(transform.eulerAngles.x) < SHARP_PITCH_ANGLE) {
            rotationRate = CASUAL_ROTATION_RATE;
            doubleRotRate = false;
        }
        else {
            rotationRate = (rotationPercent * (maxRotation - minRotation) / 100) + minRotation;
            if (doubleRotRate) rotationRate = Mathf.Clamp(rotationRate * 2, minRotation, maxRotation);
        }

        //rotate
        float step = rotationRate * Time.deltaTime;
        Vector3 nextDirection = Vector3.RotateTowards(currentForward, direction, step, 0);
        Quaternion nextRotation = Quaternion.LookRotation(nextDirection);
        transform.rotation = nextRotation;
    }

    private Vector3 CalcGroundNormal() {
        RaycastHit[] hits = new RaycastHit[feet.Length];
        Vector3 normalizedHit = Vector3.zero;

        for (int i = 0; i < hits.Length; i++) {
            Transform footTransform = feet[i].transform;
            Vector3 position = footTransform.position;
            Physics.Raycast(position, -footTransform.up, out hits[i], groundLayer);
        }

        foreach (RaycastHit hit in hits) normalizedHit += hit.normal;
        return normalizedHit.normalized;
    }

    public bool IsWalking() { return isWalking; }

    public bool IsRotating() { return rotateTowards; }
}