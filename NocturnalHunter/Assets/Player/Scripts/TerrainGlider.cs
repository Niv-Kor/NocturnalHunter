using UnityEngine;

public class TerrainGlider : MonoBehaviour
{
    [Tooltip("Minimum resistance to the floor on relatively steep slopes.")]
    [SerializeField] private float minResistance = 0;

    [Tooltip("Maximum resistance to the floor when climbing slopes.")]
    [SerializeField] private float maxResistance = 10;

    [Tooltip("An angle that's considered to be the most steep.")]
    [SerializeField] [Range(1, 90f)] private float maxSlopeAngle = 90;
    
    private static readonly float LERP_STEP_MULTIPLIER = 10;

    private Rigidbody rigidBody;
    private RigidbodyPlayerMovement playerMovement;

    public float SteepPercent {
        get {
            //get current pitch angle of the player
            float pitchAngle = transform.eulerAngles.x;
            pitchAngle = (pitchAngle > 180) ? pitchAngle - 360 : pitchAngle;

            //if the player is climbing he is less vulnerable to slipping, and more resistance is being applied
            bool climbing = pitchAngle < 0 && playerMovement.IsWalking;
            int pitchDirection = climbing ? 1 : -1;
            float absAngle = Mathf.Abs(pitchAngle);
            return pitchDirection * Mathf.Clamp(absAngle / maxSlopeAngle * 100, 0, 100);
        }
    }

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.playerMovement = GetComponent<RigidbodyPlayerMovement>();
    }

    private void Update() {
        if (playerMovement.IsGrounded) ChangeDragValue();
    }

    /// <summary>
    /// Change the drag value according to the player's position.
    /// </summary>
    private void ChangeDragValue() {
        float step = LERP_STEP_MULTIPLIER * Time.deltaTime;
        float resistance = CalcGlideResistance();
        rigidBody.drag = Mathf.Lerp(rigidBody.drag, resistance, step);
    }

    /// <summary>
    /// Calculate the drag value of the player's rigidbody,
    /// relative to his position on the terrain's steep slopes.
    /// Low drag value results in slipping down the slope,
    /// while a higher one results in climbing more slowly upwards.
    /// </summary>
    /// <returns>The player's correct drag value relative to his position.</returns>
    private float CalcGlideResistance() {
        //calculate the position of the player's pitch on a -100% to 100% specturm
        float steepPercent = SteepPercent;
        int spectrumMinimum = -100, spectrumMaximum = 100;
        float percentOnSpectrum = (steepPercent - spectrumMinimum) / (spectrumMaximum - spectrumMinimum) * 100;

        //calculate the actual resistance value relative to the point on the spectrum
        return percentOnSpectrum * (maxResistance - minResistance) / 100 + minResistance;
    }
}