using Constants;
using UnityEngine;

public class RigidbodyMovement : MonoBehaviour
{
    public enum SpeedMultiplier {
        Run, Creep, Swim
    }

    public class SummedSpeed
    {
        private AnimalStats stats;
        private bool[] flags;
        private float value, nextValue;

        public float Value {
            get { return value; }
            set { }
        }

        /// <param name="stats">The stats of the animal</param>
        public SummedSpeed(AnimalStats stats) {
            this.stats = stats;
            this.value = stats.WalkSpeed;
            this.nextValue = stats.WalkSpeed;
            this.flags = new bool[3];
        }

        /// <summary>
        /// Lerp from the current value to the next value smoothly.
        /// </summary>
        /// <param name="build">The speed of increase in the acceleration</param>
        /// <param name="decay">The speed of decrease in the acceleration</param>
        public void UpdateValue(float build, float decay) {
            float accelaration = (value < nextValue) ? build : decay;
            value = Mathf.Lerp(value, nextValue, accelaration * Time.deltaTime);
        }

        /// <summary>
        /// Apply a multiplier on the current speed (run / swim / creep).
        /// </summary>
        /// <param name="mul">The multiplier to apply</param>
        /// <param name="flag">True to apply of false to cancel</param>
        public void ApplyMultiplier(SpeedMultiplier mul, bool flag) {
            switch (mul) {
                case SpeedMultiplier.Run:
                    ChangeSummed(flags[0], flag, stats.runSpeedMultiplier);
                    flags[0] = flag;
                    break;
                case SpeedMultiplier.Creep:
                    ChangeSummed(flags[1], flag, stats.creepSpeedMultiplier);
                    flags[1] = flag;
                    break;
                case SpeedMultiplier.Swim:
                    ChangeSummed(flags[2], flag, stats.swimSpeedMultiplier);
                    flags[2] = flag;
                    break;
            }
        }

        /// <summary>
        /// Reset and return to normal walking speed.
        /// </summary>
        public void Reset() {
            ApplyMultiplier(SpeedMultiplier.Run, false);
            ApplyMultiplier(SpeedMultiplier.Creep, false);
            ApplyMultiplier(SpeedMultiplier.Swim, false);
        }

        /// <summary>
        /// Manually multiply the speed value by a specified amount,
        /// until it reaches the original value again.
        /// </summary>
        /// <param name="multiplier">The amount to multiply the speed value by</param>
        public void TempMultiply(float multiplier) { value *= multiplier; }

        /// <summary>
        /// Apply the multiplier on the current value and update its flag.
        /// </summary>
        /// <param name="isOn">True if the multiplier is already applied</param>
        /// <param name="flag">True to apply or false to cancel</param>
        /// <param name="multiplier">The value of the multiplier</param>
        private void ChangeSummed(bool isOn, bool flag, float multiplier) {
            if (isOn && !flag) nextValue /= multiplier;
            else if (!isOn && flag) nextValue *= multiplier;
        }
    }

    [Header("Acceleration")]

    [Tooltip("The maximum velocity magnitude is determined by current speed divided by relativeMaxMagnitude.")]
    [SerializeField] [Range(1, 100f)] private float relativeMaxMagnitude = 30;

    [Tooltip("The speed of increase in the acceleration.")]
    [SerializeField] private float accelerationBuild = 1;

    [Tooltip("The speed of decrease in the acceleration.")]
    [SerializeField] private float accelerationDecay = 10;

    private static readonly float SHARP_SLOPE_ANGLE = 70;
    private static readonly float MAX_PITCH_ANGLE = 70;
    private static readonly float MIN_PITCH_TO_JUMP_FORWARD = -20;
    private static readonly float FALL_SPEED = 1.5f;
    private static readonly float CASUAL_ROTATION_RATE = .5f;
    private static readonly float WALK_AFTERTIME = .1f;
    private static readonly float SOFTEST_SPEED_REDUCE = 1.3f;
    private static readonly float HARDEST_SPEED_REDUCE = .1f;

    private Rigidbody rigidBody;
    private Transform parentTransform;
    private AnimalStats animalStats;
    private TerrainGlider terrainGlider;
    private SummedSpeed speed;
    private Balance balance;
    private Vector3 turnDirection;
    private float defaultYaw;
    private float jumpTimer, walkTimer;
    private bool rotateTowards, isWalking;
    private bool doubleRotRate;
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

    public bool IsJumping { get; set; }

    private void Start() {
        this.parentTransform = transform.parent;
        this.rigidBody = GetComponentInParent<Rigidbody>();
        this.animalStats = GetComponent<AnimalStats>();
        this.balance = GetComponent<Balance>();
        this.terrainGlider = FindObjectOfType<TerrainGlider>();
        this.defaultYaw = parentTransform.eulerAngles.y;
        this.speed = new SummedSpeed(animalStats);
        this.jumpTimer = 0;

        //initial terrain hugging
        this.turnDirection = Vector3.Cross(balance.AverageGroundNormal, -parentTransform.right);
        this.rotateTowards = true;
    }

    private void Update() {
        bool underwater = balance.IsStandingOn(Layers.WATER);
        bool onWayUp = IsJumping && rigidBody.velocity.y > 0;
        IsGrounded = !onWayUp && balance.IsStandingOn(Layers.GROUND);
        IsJumping = !IsGrounded;
        AlignPosture(IsGrounded);

        //make it easier to move with 0 gravity drag value
        if (!IsGrounded || requestJump || underwater) rigidBody.drag = 0;
        //adjust drag value to the terrain
        else if (IsGrounded) terrainGlider.ChangeDragValue(transform, rigidBody, IsWalking);

        //apply swim speed multiplier if the animal is underwater
        ApplySpeedMultiplier(SpeedMultiplier.Swim, underwater);

        //decide if the animal is walking now
        if (walkTimer < WALK_AFTERTIME) walkTimer += Time.deltaTime;
        else {
            isWalking = false;
            ResetSpeedMultipliers();
        }

        Turn();
        CompleteJumpRequest();

        if (IsGrounded) {
            rigidBody.velocity = Vector3.zero; //reset velocity
            speed.UpdateValue(accelerationBuild, accelerationDecay);
        }
    }

    /// <returns>The angle in which the animal is pitched (positive is forward).</returns>
    public float GetPitchAngle() {
        float pitchAngle = parentTransform.eulerAngles.x;
        return AngleUtils.TangentiateAngle(pitchAngle);
    }

    /// <summary>
    /// Reset all speed multipliers and return to normal walking speed.
    /// </summary>
    public void ResetSpeedMultipliers() {
        speed.Reset();
    }

    /// <summary>
    /// Apply a multiplier on the current speed (run / swim / creep).
    /// </summary>
    /// <param name="mul">The multiplier to apply</param>
    /// <param name="flag">True to apply of false to cancel</param>
    public void ApplySpeedMultiplier(SpeedMultiplier mul, bool flag) {
        speed.ApplyMultiplier(mul, flag);
    }

    /// <summary>
    /// Move the animal avatar towards a specific direction.
    /// </summary>
    /// <param name="direction">Normalized direction to move towards</param>
    /// <param name="speed">Movement speed</param>
    public void Move(Vector3 direction) {
        //tune the direction and make it hug the terrain
        Vector3 groundNormal = balance.AverageGroundNormal;
        Vector3 left = Quaternion.AngleAxis(-90f, groundNormal) * direction;
        Vector3 forward = Vector3.Cross(groundNormal, left);
        direction = forward;

        //reset walk timer
        walkTimer = 0f;
        isWalking = true;

        //compress the turn direction if it's too radical (180 degrees)
        if (IsOppositeDirection(parentTransform.forward, direction)) {
            direction = Quaternion.AngleAxis(90f, groundNormal) * direction;
            doubleRotRate = true; //fasten rotation
        }

        //rotate towards the correct direction
        turnDirection = direction;
        if (turnDirection.sqrMagnitude > Mathf.Epsilon) rotateTowards = true;

        //apply force against the animal's rigidbody
        float maxMagnitude = speed.Value / relativeMaxMagnitude;
        if (IsGrounded && rigidBody.velocity.magnitude < maxMagnitude)
            rigidBody.AddForce(direction * speed.Value);
    }

    /// <summary>
    /// Straighten the animal's posture according to the ground its standing on,
    /// or pitch forward if the animal is in mid air.
    /// </summary>
    /// <param name="grounded">True if the animal is on the ground</param>
    private void AlignPosture(bool grounded) {
        //align posture to the ground
        if (grounded) {
            Vector3 groundNormal = balance.AverageGroundNormal;
            Vector3 forwardRot = Vector3.Cross(groundNormal, -parentTransform.right);
            float deltaTime = Time.deltaTime * 5;
            parentTransform.forward = Vector3.Lerp(parentTransform.forward, forwardRot, deltaTime);
        }
        //pitch forward while in mid air
        else {
            Vector3 originalRot = parentTransform.eulerAngles;
            int direction = (GetPitchAngle() >= MIN_PITCH_TO_JUMP_FORWARD) ? 1 : -1;
            Vector3 pitchedRotVector3 = new Vector3(MAX_PITCH_ANGLE * direction, originalRot.y, originalRot.z);
            Quaternion pitchedRot = Quaternion.Euler(pitchedRotVector3);
            float deltaTime = Time.deltaTime * FALL_SPEED;
            parentTransform.rotation = Quaternion.Slerp(parentTransform.rotation, pitchedRot, deltaTime);
        }
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
        if (turnDirection.sqrMagnitude < Mathf.Epsilon || !rotateTowards ) {
            doubleRotRate = false;
            rotateTowards = false;
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
        bool slipperyFall = GetPitchAngle() > SHARP_SLOPE_ANGLE;
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

        if (jumpTimer < animalStats.jumpDelay) jumpTimer += Time.deltaTime;
        else {
            jumpTimer = 0f;
            requestJump = false;

            //perform jump
            float steepPercent = terrainGlider.GetSteepPercent(transform, IsWalking); //from -100 to 100
            float normalPercent = (steepPercent + 100) / 2; //from 0 to 100
            float forceDiff = animalStats.maxJumpForce - animalStats.minJumpForce;
            float jumpForce = (100 - normalPercent) * forceDiff / 100 + animalStats.minJumpForce;
            rigidBody.AddForce(parentTransform.up * jumpForce);

            //reduce speed according to the pitch of the animal
            float reduceDiff = Mathf.Abs(HARDEST_SPEED_REDUCE - SOFTEST_SPEED_REDUCE);
            float multiplier = SOFTEST_SPEED_REDUCE - normalPercent * reduceDiff / 100;
            speed.TempMultiply(multiplier);
            IsJumping = true;
        }
    }
}