using Constants;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPlants : MonoBehaviour
{
    [Tooltip("Upper water object.")]
    [SerializeField] private GameObject upperWaterLevel;

    [Tooltip("Amount of plants in the pond.")]
    [SerializeField] private int amount;

    [Tooltip("All water plants prefabs.")]
    [SerializeField] private GameObject[] plants;

    private static readonly string PARENT_NAME = "Vegetation";

    private GameObject vegetationParent;
    private float waterLevel;
    private float pondRadius;

    private void Start() {
        this.waterLevel = upperWaterLevel.transform.position.y;
        this.pondRadius = upperWaterLevel.GetComponent<MeshRenderer>().bounds.extents.x;
        this.vegetationParent = new GameObject(PARENT_NAME);
        vegetationParent.transform.SetParent(transform);

        Spread();
    }

    private void Spread() {
        int remainderAmount = amount;

        for (int i = 0; i < plants.Length; i++) {
            bool lastPlant = i == plants.Length - 1;
            int currentAmount = lastPlant ? remainderAmount : Random.Range(0, remainderAmount);
            remainderAmount -= currentAmount;
            Spawn(plants[i], currentAmount);
            print("Spawned " + currentAmount + " of " + plants[i].name);
        }
    }

    private void Spawn(GameObject prefab, int amount) {
        float plantRadius = prefab.GetComponent<SphereCollider>().bounds.extents.x;
        Collider[] collisionResults = new Collider[64];

        for (int i = 0; i < amount; i++) {
            Vector3 position;

            //find a good random spot on the water
            while (true) {
                float randomDistance = Random.Range(0, pondRadius);
                float randomAngle = Random.Range(0, 360);
                Vector3 direction = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward;
                position = upperWaterLevel.transform.position + direction * randomDistance;
                position.y = waterLevel;
                Physics.OverlapSphereNonAlloc(position, plantRadius, collisionResults, Layers.GROUND);

                if (TouchesGround(collisionResults)) continue;
                else break;
            }

            //instantiate
            GameObject instance = Instantiate(prefab, position, Quaternion.Euler(0, 0, 1));
            instance.transform.SetParent(vegetationParent.transform);
        }
    }

    private bool TouchesGround(Collider[] collisionResults) {
        foreach (Collider col in collisionResults) {
            if (col == null) return false;
            else if (Layers.ContainedInMask(col.gameObject.layer, Layers.GROUND)) return true;
        }

        return false;
    }
}