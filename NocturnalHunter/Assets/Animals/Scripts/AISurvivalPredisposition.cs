using System.Collections.Generic;
using UnityEngine;

public class AISurvivalPredisposition : MonoBehaviour
{
    [SerializeField] public List<GameObject> predators;
    [SerializeField] private float visionTheta = 30; /// TEST
    [SerializeField] private float visionRadius = 60; /// TEST

    private AIFlightBehaviour flightBehaviour;

    private void Start() {
        this.flightBehaviour = GetComponent<AIFlightBehaviour>();
    }

    private void Update() {
        if (SeePredator()) flightBehaviour.Run();
    }

    /// <returns>True if this animal sees a predator nearby.</returns>
    private bool SeePredator() {
        Vector3 startVec = transform.position;
        Vector3 startVecFwd = transform.forward;

        foreach (GameObject predator in predators) {
            Vector3 predatorPosition = predator.transform.position;
            Vector3 predatorDir = predatorPosition - startVec;
            float predatorAngle = Vector3.Angle(predatorDir, startVecFwd);
            float predatorDistance = Vector3.Distance(startVec, predatorPosition);

            //predator is close by and in front of prey
            if (predatorAngle <= visionTheta && predatorDistance <= visionRadius) return true;
        }

        //no predator detected
        return false;
    }
}