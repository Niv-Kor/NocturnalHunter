using UnityEngine;

public class UnderwaterReflection : MonoBehaviour
{
    [Tooltip("The main camera of the game.")]
    [SerializeField] private GameObject mainCamera;

    [Tooltip("Upper water object.")]
    [SerializeField] private GameObject upperWaterLevel;

    [Tooltip("Lower water object.")]
    [SerializeField] private GameObject lowerWaterLevel;

    [Tooltip("The light under the water.")]
    [SerializeField] private GameObject underwaterShader;

    [Tooltip("The bubbles under the water.")]
    [SerializeField] private GameObject bubbles;

    [Header("Depth")]

    [Tooltip("Distance from the deepest point of pond up to the water level.")]
    [SerializeField] private float depth;

    [Tooltip("Draw the depth scale.")]
    [SerializeField] private bool visualize = true;

    private GeoProperties geoProperties;
    private ParticleSystem bubbleParticles;

    private void Start() {
        this.geoProperties = GetComponent<GeoProperties>();
        this.bubbleParticles = bubbles.GetComponent<ParticleSystem>();

        Vector3 waterLevelPos = lowerWaterLevel.transform.position;
        Vector3 bubblesPos = new Vector3(waterLevelPos.x, waterLevelPos.y - depth, waterLevelPos.z);
        bubbles.transform.position = bubblesPos;
    }

    private void Update() {
        bool playerNearby = geoProperties.PlayerDistance <= 1f;
        bool aboveWater = mainCamera.transform.position.y >= geoProperties.WaterLevel + .3f;
        bool showUpperPond = aboveWater || !playerNearby;

        upperWaterLevel.SetActive(showUpperPond);
        lowerWaterLevel.SetActive(!showUpperPond);
        underwaterShader.SetActive(!showUpperPond);

        if (showUpperPond) bubbleParticles.Pause();
        else bubbleParticles.Play();
    }

    private void OnDrawGizmos() {
        if (!visualize) return;

        Vector3 origin = lowerWaterLevel.transform.position;
        Vector3 destination = origin + Vector3.down * depth;
        Gizmos.color = Color.red;
        Gizmos.DrawLine(origin, destination);
    }
}