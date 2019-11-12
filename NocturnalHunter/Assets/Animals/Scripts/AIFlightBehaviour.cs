using UnityEngine;

public class AIFlightBehaviour : MonoBehaviour
{
    private static readonly float MIN_NODE_DISTANCE = 5;

    private RouteManager routeManager;
    private AnimalStats animalStats;
    private AIStateController stateController;
    private RigidbodyMovement rigidbodyMovement;
    private RouteNodeID nextNode;
    private bool inFlight;

    private void Start() {
        this.routeManager = FindObjectOfType<RouteManager>();
        this.animalStats = GetComponent<AnimalStats>();
        this.stateController = GetComponent<AIStateController>();
        this.rigidbodyMovement = GetComponent<RigidbodyMovement>();
        this.nextNode = null;
        this.inFlight = false;
    }

    private void Update() {
        if (!inFlight) {
            stateController.RequestMovement(AIStateController.AIMovementMode.Walk);
            return;
        }

        FindRunningPath();
        stateController.RequestMovement(AIStateController.AIMovementMode.Run);
        Vector3 direction = (nextNode.Point - transform.position).normalized;
        float runSpeed = animalStats.walkSpeed * animalStats.runSpeedMultiplier;
        rigidbodyMovement.Move(direction, runSpeed);
    }

    public void Run() {
        FindRunningPath();
        inFlight = true;
    }

    private void FindRunningPath() {
        if (!inFlight) nextNode = routeManager.NearestRoute(transform.position);
        else {
            float nodeDistance = Vector3.Distance(transform.position, nextNode.Point);
            if (nodeDistance <= MIN_NODE_DISTANCE) nextNode = routeManager.NextNode(transform.position);
        }
    }
}