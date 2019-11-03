using UnityEngine;

public class Foot : MonoBehaviour
{
    public enum FootYPosition {
        Middle, Front, Rear
    }

    public enum FootXPosition {
        Middle, Left, Right
    }

    [SerializeField] public FootYPosition yPosition;
    [SerializeField] public FootXPosition xPosition;
    [SerializeField] private bool visualize = true;

    private readonly int MAX_COLLISION_RESULTS = 64;
    private readonly float MIN_GROUND_DISTANCE = .15f;

    private Collider[] colResults;
    private float radius;

    private void Start() {
        Renderer renderer = GetComponent<Renderer>();
        this.radius = renderer.bounds.extents.magnitude;
        this.colResults = new Collider[MAX_COLLISION_RESULTS];
    }

    private void OnDrawGizmos() {
        if (!visualize) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, transform.localScale.x);
    }

    /// <param name="groundLayer">Layer of the solid objects the player can stand on.</param>
    /// <returns>True if this foot is touching the ground.</returns>
    public bool IsGrounded(LayerMask groundLayer) {
        float extendedRadius = radius + MIN_GROUND_DISTANCE;
        int collisions = Physics.OverlapSphereNonAlloc(transform.position, extendedRadius, colResults, groundLayer);
        return collisions > 0;
    }
}