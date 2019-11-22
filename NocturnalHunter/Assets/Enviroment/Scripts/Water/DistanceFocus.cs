using UnityEngine;
using UnityStandardAssets.Water;

public class DistanceFocus : MonoBehaviour
{
    [Tooltip("Upper water object.")]
    [SerializeField] private GameObject upperWaterLevel;

    [Tooltip("The maximum distance from which the player can see water details.")]
    [SerializeField] private float maxDetailDistance;

    [Tooltip("The maximum distance from which the player can see water details.")]
    [SerializeField] private int minTextureSize;

    [Tooltip("The maximum distance from which the player can see water details.")]
    [SerializeField] private int maxTextureSize;

    private GeoProperties geoProperties;
    private Water waterComponent;

    private void Start() {
        this.geoProperties = GetComponent<GeoProperties>();
        waterComponent = upperWaterLevel.GetComponent<Water>();
    }

    private void Update() {
        float distancePercent = geoProperties.PlayerDistance / maxDetailDistance * 100;
        distancePercent = 100 - Mathf.Clamp(distancePercent, 0, 100);

        if (distancePercent == 0) waterComponent.enabled = false;
        else {
            waterComponent.enabled = true;
            int textureSize = (int) (distancePercent * (maxTextureSize - minTextureSize) / 100) + minTextureSize;
            waterComponent.textureSize = textureSize;
        }
    }
}