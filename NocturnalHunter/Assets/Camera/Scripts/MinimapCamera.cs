using UnityEngine;

public class MinimapCamera : MonoBehaviour
{
    [Tooltip("The player object that the minimap is following.")]
    [SerializeField] private Transform player;

    [Tooltip("The main terrain in the scene.")]
    [SerializeField] private GameObject terrain;

    private Camera camComponent;
    private float height;

    private void Start() {
        Terrain terrainComponent = terrain.GetComponent<Terrain>();
        TerrainData terrainData = terrainComponent.terrainData;
        this.camComponent = GetComponent<Camera>();
        this.height = terrainData.size.y * 1.5f;
        float terrainHeight = terrain.transform.position.y;
        camComponent.farClipPlane =  height * 1.5f - terrainHeight;
    }

    private void Update() {
        MoveToPlayer();
    }

    /// <summary>
    /// Move the camera to the player's x and z position.
    /// </summary>
    private void MoveToPlayer() {
        Vector3 newPos = player.position;
        newPos.y += height; //keep only player's x and z
        transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime);
    }
}