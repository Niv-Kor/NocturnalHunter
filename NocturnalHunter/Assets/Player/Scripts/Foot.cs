using UnityEngine;

public class Foot : MonoBehaviour
{
    public enum FootYPosition {
        Middle, Front, Rear
    }

    public enum FootXPosition
    {
        Middle, Left, Right
    }

    [SerializeField] public FootYPosition yPosition;
    [SerializeField] public FootXPosition xPosition;
    [SerializeField] private bool visualize = true;

    private void OnDrawGizmos() {
        if (!visualize) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(transform.position, transform.localScale.x);
    }

    public Vector3 GetGroundedVector() {
        Vector3 yVector, xVector;

        switch (yPosition) {
            case FootYPosition.Front: yVector = new Vector3(1, 0, 0); break;
            case FootYPosition.Rear: yVector = new Vector3(-1, 0, 0); break;
            default: yVector = Vector3.zero; break;
        }

        switch (xPosition) {
            case FootXPosition.Left: xVector = new Vector3(0, 0, 1); break;
            case FootXPosition.Right: xVector = new Vector3(0, 0, -1); break;
            default: xVector = Vector3.zero; break;
        }

        return yVector + xVector;
    }

    public RaycastHit RaycastOppositeDirection() {
        Vector3 yDirection, xDirection;

        switch (yPosition) {
            case FootYPosition.Front: yDirection = -transform.forward; break;
            case FootYPosition.Rear: yDirection = transform.forward; break;
            default: yDirection = Vector3.zero; break;
        }

        switch (xPosition) {
            case FootXPosition.Right: xDirection = -transform.right; break;
            case FootXPosition.Left: xDirection = transform.right; break;
            default: xDirection = Vector3.zero; break;
        }

        Physics.Raycast(transform.position, yDirection + xDirection, out RaycastHit hit);
        return hit;
    }
}