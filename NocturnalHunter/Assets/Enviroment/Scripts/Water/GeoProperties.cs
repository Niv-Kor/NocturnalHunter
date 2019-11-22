using UnityEngine;

public class GeoProperties : MonoBehaviour
{
    [Tooltip("The main camera of the game.")]
    [SerializeField] private GameObject mainCamera;

    [Tooltip("Upper water object.")]
    [SerializeField] private GameObject upperWaterLevel;

    [Tooltip("Lower water object.")]
    [SerializeField] private GameObject lowerWaterLevel;

    private float waterLevel, radius;
    
    public float WaterLevel {
        get { return waterLevel; }
        set { }
    }

    public float Radius {
        get { return radius; }
        set { }
    }

    public float PlayerDistance {
        get {
            Vector3 camPosition = mainCamera.transform.position;
            Vector3 midPondPosition = upperWaterLevel.transform.position;
            return Vector3.Distance(camPosition, midPondPosition) - Radius;
        }
        set { }
    }

    private void Start() {
        this.radius = upperWaterLevel.GetComponent<MeshRenderer>().bounds.extents.x;
        float upperLevel = upperWaterLevel.transform.position.y;
        float lowerLevel = lowerWaterLevel.transform.position.y;
        this.waterLevel = (upperLevel + lowerLevel) / 2;
    }
}