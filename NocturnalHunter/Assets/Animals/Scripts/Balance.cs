using Constants;
using System;
using UnityEngine;

public class Balance : MonoBehaviour
{
    [Serializable]
    public struct GroundInteractor
    {
        [Tooltip("The center of the interactor.\n"
               + "WARINIG: The parent object must be rotated to (0, 0, 0) before assigning this parameter.")]
        [SerializeField] public Vector3 center;

        [Tooltip("The radius of the interactor (it may not always be meaningful).")]
        [SerializeField] public float radius;
    }

    [Tooltip("The animal's feet.")]
    [SerializeField] private GroundInteractor[] feet;

    [Header("Debug")]

    [Tooltip("Draw sphere gizmos where the GroundInteractors are.")]
    [SerializeField] private bool visualize = true;

    private static readonly string BALANCE_PARENT_NAME = "Ground Interactors";
    private static readonly string FOOT_NAME = "Foot";
    private static readonly int MAX_COLLISION_RESULTS = 32;
    private static readonly float MIN_GROUND_DISTANCE = .15f;

    private GameObject interactorsParent;
    private GameObject[] feetObj;
    private Collider[] colResults;

    public Vector3 AverageGroundNormal {
        get {
            try {
                RaycastHit[] hits = new RaycastHit[feet.Length];
                Vector3 normalizedHit = Vector3.zero;

                for (int i = 0; i < feet.Length; i++) {
                    Transform footTransform = feetObj[i].transform;
                    Physics.Raycast(footTransform.position, -footTransform.up, out hits[i], Layers.GROUND);
                }

                foreach (RaycastHit hit in hits) normalizedHit += hit.normal;
                return normalizedHit.normalized;
            }
            catch (NullReferenceException) { return Vector3.up; }
        }
        set { }
    }

    private void Start() {
        this.colResults = new Collider[MAX_COLLISION_RESULTS];
        this.feetObj = new GameObject[feet.Length];
        this.interactorsParent = new GameObject(BALANCE_PARENT_NAME);
        interactorsParent.transform.SetParent(transform);
        interactorsParent.transform.localPosition = Vector3.zero;
        interactorsParent.transform.localRotation = Quaternion.Euler(Vector3.zero);

        //create an object for each foot
        for (int i = 0; i < feet.Length; i++) {
            GameObject foot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            foot.name = FOOT_NAME + " (" + i + ")";
            foot.GetComponent<MeshRenderer>().enabled = false;
            foot.GetComponent<SphereCollider>().enabled = false;
            foot.transform.SetParent(interactorsParent.transform);
            feetObj[i] = foot;

            //set position and radius
            foot.transform.localPosition = feet[i].center;
            foot.transform.localScale = Vector3.one * feet[i].radius;
        }
    }

    private void FixedUpdate() {
        UpdateInteractors();
    }

    private void OnDrawGizmos() {
        //only visualize during gameplay
        if (!visualize || !Application.isPlaying) return;

        //draw feet
        Gizmos.color = Color.cyan;
        for (int i = 0; i < feet.Length; i++) {
            Transform footTransform = feetObj[i].transform;
            Gizmos.DrawSphere(footTransform.position, feet[i].radius);
        }
    }

    /// <summary>
    /// Update the position property of the interactor parameters.
    /// </summary>
    private void UpdateInteractors() {
        for (int i = 0; i < feet.Length; i++)
            feet[i].center = feetObj[i].transform.localPosition;
    }

    /// <summary>
    /// Check if at least on of the animal's feet is touching at least one object of a certain layer.
    /// </summary>
    /// <param name="layer">The layer to the collision of the foot with</param>
    /// <returns>True if this foot is touching an object of that layer.</returns>
    public bool IsStandingOn(LayerMask layer) {
        for (int i = 0; i < feet.Length; i++) {
            GroundInteractor footInteractor = feet[i];
            Vector3 footPos = feetObj[i].transform.position;
            float extendedRadius = footInteractor.radius + MIN_GROUND_DISTANCE;
            int collisions = Physics.OverlapSphereNonAlloc(footPos, extendedRadius, colResults, layer);
            if (collisions > 0) return true;
        }

        return false;
    }
}