using UnityEngine;

public class AnimalStats : MonoBehaviour
{
    [Tooltip("True if this character is playable by the user.\n" +
             "This causes the walk speed to be the average between min and max.")]
    [SerializeField] private bool playable;

    [Header("Speed")]

    [Tooltip("The minimum speed of regular walking.")]
    [SerializeField] private float minWalkSpeed = 5;

    [Tooltip("The maximum speed of regular walking.")]
    [SerializeField] private float maxWalkSpeed = 15;

    [Tooltip("Creeping speed as a percentage of the current walking speed.")]
    [SerializeField] public float creepSpeedMultiplier = .5f;

    [Tooltip("Running speed as a percentage of the current walking speed.")]
    [SerializeField] public float runSpeedMultiplier = 2;

    [Tooltip("Swimming speed as a percentage of the current walking speed.")]
    [SerializeField] public float swimSpeedMultiplier = .5f;

    [Tooltip("Minimum speed of rotation (on the 'y' axis).")]
    [SerializeField] public float minRotationSpeed = 2;

    [Tooltip("Maximum speed of rotation (on the 'y' axis).")]
    [SerializeField] public float maxRotationSpeed = 12;

    [Header("Jump")]

    [Tooltip("The minimum force being applied on the animal when jumping.")]
    [SerializeField] public float minJumpForce = 250;

    [Tooltip("The maximum force being applied on the animal when jumping.")]
    [SerializeField] public float maxJumpForce = 500;
    
    [Tooltip("Delay until the jump occurs.")]
    [SerializeField] public float jumpDelay = 0;

    [Header("Senses")]

    [Tooltip("The angle of the vision's sector from the animal point of view.")]
    [SerializeField] public float visionAngle = 90;

    [Tooltip("How far can the animal see objects from.")]
    [SerializeField] public float visionDistance = 50;

    private float walkSpeed;
    private bool isPlayable;

    private void Awake() {
        CalcSpeed();
    }

    public float WalkSpeed {
        get {
            if (isPlayable != playable) CalcSpeed();
            return walkSpeed;
        }
        set { }
    }

    /// <summary>
    /// Calculate the animal's speed according to its minimum and maximum speed values.
    /// The speed of a playbale animal is the average between the two values,
    /// while the speed of a non playable one is selected randomly between them.
    /// </summary>
    private void CalcSpeed() {
        if (playable) walkSpeed = (minWalkSpeed + maxWalkSpeed) / 2;
        else walkSpeed = Random.Range(minWalkSpeed, maxWalkSpeed);
        isPlayable = playable;
    }
}