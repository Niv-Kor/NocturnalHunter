using UnityEngine;

public class CompassRotator : MonoBehaviour
{
    [Tooltip("The camera rig object, containing the main camera as a child.")]
    [SerializeField] private GameObject cameraRig;

    private RectTransform rect;
    private Transform camTransform;

    private void Start() {
        this.rect = GetComponent<RectTransform>();
        this.camTransform = cameraRig.transform;
    }

    private void Update() {
        rect.rotation = Quaternion.Euler(0, 0, camTransform.eulerAngles.y);
    }
}