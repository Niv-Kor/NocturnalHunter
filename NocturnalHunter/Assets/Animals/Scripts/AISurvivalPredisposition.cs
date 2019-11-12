using System.Collections.Generic;
using UnityEngine;

public class AISurvivalPredisposition : MonoBehaviour
{
    [SerializeField] public List<GameObject> predators;

    private AIFlightBehaviour flightBehaviour;
    private AnimalStats animalStats;

    private void Start() {
        this.flightBehaviour = GetComponent<AIFlightBehaviour>();
        this.animalStats = GetComponent<AnimalStats>();
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
            float visionAngle = animalStats.visionAngle;
            float visionRadius = animalStats.visionDistance;

            //predator is close by and in front of prey
            if (predatorAngle <= visionAngle && predatorDistance <= visionRadius) return true;
        }

        //no predator detected
        return false;
    }
}