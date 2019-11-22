using Constants;
using UnityEngine;

public class PlantsDistributer : MonoBehaviour
{
    [Tooltip("Upper water object.")]
    [SerializeField] private GameObject upperWaterLevel;

    [Tooltip("Amount of plants to be spawned in the pond.")]
    [SerializeField] private int amount;

    [Tooltip("All water plants prefabs.")]
    [SerializeField] private GameObject[] plants;

    private static readonly string PARENT_NAME = "Vegetation";
    private static readonly int COLLISION_ALLOCATION = 32;

    private GeoProperties geoProperties;
    private GameObject vegetationParent;

    private void Start() {
        this.geoProperties = GetComponent<GeoProperties>();
        this.vegetationParent = new GameObject(PARENT_NAME);
        vegetationParent.transform.SetParent(transform);
        Shuffle();
        Spread();
    }

    /// <summary>
    /// Shuffle the plants array.
    /// </summary>
    private void Shuffle() {
        for (int i = 0; i < plants.Length; i++) {
            int rng = Random.Range(0, plants.Length);
            GameObject temp = plants[rng];
            plants[rng] = plants[i];
            plants[i] = temp;
        }
    }

    /// <summary>
    /// Assign a random amount for each water plant prefab,
    /// so that they all together contain exacly the amount given.
    /// </summary>
    private void Spread() {
        int remainderAmount = amount;

        for (int i = 0; i < plants.Length; i++) {
            bool lastPlant = i == plants.Length - 1;
            int currentAmount = lastPlant ? remainderAmount : Random.Range(0, remainderAmount);
            remainderAmount -= currentAmount;
            Spawn(plants[i], currentAmount);
        }
    }

    /// <summary>
    /// Instantiate a prefab of a water plant, and place it as a child of the pond.
    /// </summary>
    /// <param name="prefab">The water plant prefab to instantiate</param>
    /// <param name="amount">Number of instances to create</param>
    private void Spawn(GameObject prefab, int amount) {
        float plantRadius = prefab.GetComponent<SphereCollider>().bounds.extents.x;
        Collider[] collisionResults = new Collider[COLLISION_ALLOCATION];

        for (int i = 0; i < amount; i++) {
            Vector3 position;

            //find a good random spot on the water
            while (true) {
                float randomDistance = Random.Range(0, geoProperties.Radius);
                float randomAngle = Random.Range(0, 360);
                Vector3 direction = Quaternion.AngleAxis(randomAngle, Vector3.up) * Vector3.forward;
                position = upperWaterLevel.transform.position + direction * randomDistance;
                position.y = geoProperties.WaterLevel;
                int cols = Physics.OverlapSphereNonAlloc(position, plantRadius, collisionResults, Layers.GROUND);

                if (cols > 0) continue;
                else break;
            }

            //instantiate
            GameObject instance = Instantiate(prefab, position, Quaternion.Euler(0, 0, 1));
            instance.transform.SetParent(vegetationParent.transform);
        }
    }
}