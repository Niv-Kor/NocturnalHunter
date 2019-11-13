using UnityEngine;

public class UnderwaterReflection : MonoBehaviour
{
    [Tooltip("The main camera rig of the game.")]
    [SerializeField] private GameObject cameraRig;

    [Tooltip("Upper water object.")]
    [SerializeField] private GameObject upperWaterLevel;

    [Tooltip("Lower water object.")]
    [SerializeField] private GameObject lowerWaterLevel;

    [Tooltip("The light under the water.")]
    [SerializeField] private GameObject underwaterShader;

    [Tooltip("The bubbles under the water.")]
    [SerializeField] private GameObject bubbles;

    private float waterLevel;

    private void Start() {
        float upperLevel = upperWaterLevel.transform.position.y;
        float lowerLevel = lowerWaterLevel.transform.position.y;
        this.waterLevel = (upperLevel + lowerLevel) / 2;
    }

    private void Update() {
        bool aboveWater = cameraRig.transform.position.y > waterLevel;
        upperWaterLevel.SetActive(aboveWater);
        lowerWaterLevel.SetActive(!aboveWater);
        underwaterShader.SetActive(!aboveWater);
        bubbles.SetActive(!aboveWater);
    }
}