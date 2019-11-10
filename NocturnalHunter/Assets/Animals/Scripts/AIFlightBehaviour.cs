using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIFlightBehaviour : MonoBehaviour
{
    [SerializeField] private float runningSpeed = 10; /// TEST

    private RouteManager routeManager;
    private RouteNodeID nextNode;
    private bool inFlight;

    private void Start() {
        this.routeManager = FindObjectOfType<RouteManager>();
        this.nextNode = null;
        this.inFlight = false;
    }

    private void Update() {
        if (!inFlight) return;

        Vector3 direction = Vector3.Normalize(transform.position - nextNode.Point);
        transform.position += direction * runningSpeed;
    }

    public void Run() {
        if (!inFlight) nextNode = routeManager.NearestRoute(transform.position);
        else nextNode = routeManager.NextNode(transform.position);

        inFlight = true;
    }
}