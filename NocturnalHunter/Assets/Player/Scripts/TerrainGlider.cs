using UnityEngine;

public class TerrainGlider : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float hoverHeight = .1f;

    [SerializeField] private float minResistance = 10;
    [SerializeField] private float maxResistance = 0;
    [SerializeField] private float maxSlopeAngle = 90;

    private Rigidbody rigidBody;
    private RigidbodyPlayerMovement playerMovement;

    private void Start() {
        this.rigidBody = GetComponent<Rigidbody>();
        this.playerMovement = GetComponent<RigidbodyPlayerMovement>();
    }

    private void Update() {
        //Hover();
        //if (!playerMovement.IsRotating()) AlignWithTerrain();
        rigidBody.drag = Mathf.Lerp(rigidBody.drag, CalcGlideResistance(), 10 * Time.deltaTime);
    }

    private float CalcGlideResistance() {
        float pitchAngle = transform.eulerAngles.x;
        pitchAngle = (pitchAngle > 180) ? pitchAngle - 360 : pitchAngle;
        float absAngle = Mathf.Abs(pitchAngle);
        float pitchPercent = Mathf.Clamp(absAngle / maxSlopeAngle * 100, 0, 100);
        float resistance = (pitchPercent * (maxResistance - minResistance) / 100) + minResistance;

        if (playerMovement.IsWalking() && pitchAngle < 0) return (100 + pitchPercent) * resistance / 100;
        else return (maxResistance + minResistance) - resistance;
    }

    /*private void AlignWithTerrain() {
        RaycastHit[] hits = new RaycastHit[feet.Length];

        for (int i = 0; i < hits.Length; i++) {
            Vector3 position = feet[i].transform.position;
            Physics.Raycast(position, Vector3.down, out hits[i], groundLayer);
        }

        for (int i = 0; i < hits.Length; i++) {
            float groundDistY = hits[i].distance;

            if (groundDistY > hoverHeight) {
                Foot footComponent = feet[i].GetComponent<Foot>();
                Vector3 direction = footComponent.GetGroundedVector();
                float groundDistX = footComponent.RaycastOppositeDirection().distance;
                float alpha = Mathf.Atan(groundDistY / groundDistX) * Mathf.Rad2Deg;
                Vector3 currentRot = transform.eulerAngles;
                Vector3 targetRot = currentRot + direction * alpha * 2;
                Vector3 stepRot = Vector3.Lerp(currentRot, targetRot, Time.deltaTime);

                transform.rotation = Quaternion.Euler(stepRot);
            }
        }

        //align down
        *//*Vector3 normalizedHit = Vector3.zero;
        foreach (RaycastHit hit in hits) normalizedHit += hit.normal;

        *//*Vector3 backupForward = transform.forward;
        Vector3 backupUp = transform.up;
        Vector3 upTransform = Vector3.Lerp(backupUp, normalizedHit.normalized, Time.deltaTime);

        transform.up = new Vector3(upTransform.x, backupUp.y, upTransform.z);*//*
        //transform.forward = backupForward;

        Vector3 unitVector = normalizedHit.normalized;
        transform.up = unitVector;*//*
    }*/
}
