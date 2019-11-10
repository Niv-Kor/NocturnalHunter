using UnityEngine;

public class AnimalStats : MonoBehaviour
{
    [Header("Speed")]

    [Tooltip("The speed of regular walking.")]
    [SerializeField] public float walkSpeed = 15;

    [Tooltip("Creeping speed as a percentage of the current walking speed.")]
    [SerializeField] public float creepSpeedMultiplier = .5f;

    [Tooltip("The speed of running.")]
    [SerializeField] public float runSpeedMultiplier = 2;

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
}
