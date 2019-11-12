using Constants;
using UnityEngine;

public class RouteNodeID : MonoBehaviour
{
    [Tooltip("The path that this node belongs to.")]
    [SerializeField] public char routePath;

    [Tooltip("The numerical order of this node in its path.")]
    [SerializeField] public int routeOrder;

    [Tooltip("The alternative paths that can be accessed through this node.")]
    [SerializeField] public char[] junctionOf;

    private static readonly int initHeight = 5_000;

    private void Start() {
        transform.Translate(0, initHeight, 0);
        Point = CalcPoint();
    }

    public Vector3 Point {
        get { return transform.position; }
        set { transform.position = value; }
    }

    /// <returns>The point of the node on the terrain's surface.</returns>
    private Vector3 CalcPoint() {
        Vector3 rayHit = GetRayHit(Vector3.down);
        Vector3 position = transform.position;
        return new Vector3(position.x, rayHit.y, position.z);
    }

    /// <summary>
    /// Send a raycast from the node, only capable of hitting object under ground layer.
    /// </summary>
    /// <param name="direction">The direction of the ray</param>
    /// <returns>The hit point (or infinity if it didn't hit).</returns>
    private Vector3 GetRayHit(Vector3 direction) {
        Ray ray = new Ray(transform.position, direction);
        bool validRaycast = Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, Layers.GROUND);
        return validRaycast ? hit.point : Vector3.positiveInfinity;
    }
}