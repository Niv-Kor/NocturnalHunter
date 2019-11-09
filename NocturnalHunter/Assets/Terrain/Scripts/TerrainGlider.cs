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

    /// <param name="avatarTransform">The transform component of the animal's avatar</param>
    /// <param name="isWalking">True if the animal is currently walking</param>
    /// <returns>Percentage of the animal's steepness on the terrain.</returns>
    public float GetSteepPercent(Transform avatarTransform, bool isWalking) {
        //get current pitch angle of the player
        float pitchAngle = avatarTransform.eulerAngles.x;
        pitchAngle = (pitchAngle > 180) ? pitchAngle - 360 : pitchAngle;

        //if the player is climbing he is less vulnerable to slipping, and more resistance is being applied
        bool climbing = pitchAngle < 0 && isWalking;
        int pitchDirection = climbing ? 1 : -1;
        float absAngle = Mathf.Abs(pitchAngle);
        return pitchDirection * Mathf.Clamp(absAngle / maxSlopeAngle * 100, 0, 100);
    }

    /// <summary>
    /// Get the linear change of the animal's drag value, relative to its position on the terrain.
    /// </summary>
    /// <param name="avatarTransform">The transform component of the animal's avatar</param>
    /// <param name="rigidBody">The animal's rigidbody component</param>
    /// <param name="isWalking">True if the animal is currently walking</param>
    public void ChangeDragValue(Transform avatarTransform, Rigidbody rigidBody, bool isWalking) {
        float step = LERP_STEP_MULTIPLIER * Time.deltaTime;
        float resistance = CalcGlideResistance(avatarTransform, isWalking);
        rigidBody.drag = Mathf.Lerp(rigidBody.drag, resistance, step);
    }

    /// <summary>
    /// Calculate the drag value of the animal's rigidbody,
    /// relative to his position on the terrain's steep slopes.
    /// Low drag value results in slipping down the slope,
    /// while a higher one results in climbing more slowly upwards.
    /// </summary>
    /// <param name="avatarTransform">The transform component of the animal's avatar</param>
    /// <param name="isWalking">True if the animal is currently walking</param>
    /// <returns>The animal's correct drag value relative to its position.</returns>
    private float CalcGlideResistance(Transform avatarTransform, bool isWalking) {
        //calculate the position of the player's pitch on a -100% to 100% specturm
        float steepPercent = GetSteepPercent(avatarTransform, isWalking);
        int spectrumMinimum = -100, spectrumMaximum = 100;
        float percentOnSpectrum = (steepPercent - spectrumMinimum) / (spectrumMaximum - spectrumMinimum) * 100;

        //calculate the actual resistance value relative to the point on the spectrum
        return percentOnSpectrum * (maxResistance - minResistance) / 100 + minResistance;
    }
}