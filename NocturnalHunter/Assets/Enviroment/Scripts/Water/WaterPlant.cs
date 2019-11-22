using UnityEngine;

public class WaterPlant : MonoBehaviour
{
    [Tooltip("The speed of the lily on the water.")]
    [SerializeField] private float floatSpeed;

    [Tooltip("The angle in which the lily rotates per frame.")]
    [SerializeField] [Range(0, 360f)] private float rotationRate;

    [Tooltip("Change the direction of the plant upon collision with these layers.")]
    [SerializeField] private LayerMask collision;

    private Vector3 floatDirection;
    private float radius;

    private void Start() {
        this.floatDirection = Vector3.forward;
        this.radius = GetComponent<SphereCollider>().bounds.extents.x;
        ChangeDirection(0, 360);
    }

    private void Update() {
        Vector3 destination = transform.position + floatDirection;
        transform.position = Vector3.Lerp(transform.position, destination, floatSpeed * Time.deltaTime);
        transform.Rotate(Vector3.up, rotationRate);

        if (CheckCollision(collision)) ChangeDirection(135, 270);
    }

    /// <summary>
    /// Check if the lily touches a certain mask of layers.
    /// </summary>
    /// <param name="mask">The layer mask to check against</param>
    /// <returns>True if the lily touches one of the layers.</returns>
    private bool CheckCollision(LayerMask mask) {
        Ray ray = new Ray(transform.position, floatDirection);
        return Physics.Raycast(ray, radius, mask);
    }
    
    /// <summary>
    /// Change the floating direction of the lily.
    /// </summary>
    /// <param name="minAngle">Minimum angle of change</param>
    /// <param name="maxAngle">Maximum angle of change</param>
    private void ChangeDirection(float minAngle, float maxAngle) {
        float turnAngle = Random.Range(minAngle, maxAngle);
        floatDirection = Quaternion.AngleAxis(turnAngle, Vector3.up) * floatDirection;
    }
}