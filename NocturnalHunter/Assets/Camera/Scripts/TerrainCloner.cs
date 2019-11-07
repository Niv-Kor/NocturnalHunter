using UnityEngine;
using Constants;

public class TerrainCloner : MonoBehaviour
{
    [Tooltip("The main terrain of the scene.")]
    [SerializeField] private GameObject terrain;

    private Terrain terrainComponent;
    private TerrainData data;
    private GameObject clone;

    private void Start() {
        this.terrainComponent = terrain.GetComponent<Terrain>();
        this.data = terrainComponent.terrainData;
        this.clone = Clone();
        clone.layer = Layers.MINIMAP;
    }

    /// <summary>
    /// Clone the main terrain and make it a child of itself.
    /// </summary>
    /// <returns>The clone game object.</returns>
    private GameObject Clone() {
        Quaternion rotation = terrain.transform.rotation;
        float maxHeight = data.size.y / 2;
        Vector3 position = terrain.transform.position - Vector3.up * maxHeight;
        return Instantiate(terrain, position, rotation, terrain.transform);
    }
}