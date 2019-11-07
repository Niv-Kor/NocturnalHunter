using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]

public class MinimapIcon : MonoBehaviour
{
    private enum MapLayer {
        Terrain, Player, NPC, Landmark, Entity
    }

    [Tooltip("The sprite to display on the minimap.")]
    [SerializeField] private Sprite sprite;

    [Tooltip("The layer that this icon belongs to.")]
    [SerializeField] private MapLayer layer;

    [Tooltip("The size of the icon as viewed over the minimap.")]
    [SerializeField] [Range(1, 60f)] private float size = 1;

    private static readonly float X_ROTATION = 90;
    private static readonly float Z_ROTATION = 0;

    private SpriteRenderer spriteRenderer;

    private void Start() {
        this.spriteRenderer = GetComponent<SpriteRenderer>();

        //relocate
        transform.position = transform.parent.position;

        //sort
        spriteRenderer.sortingLayerName = layer.ToString();
        spriteRenderer.sortingOrder = GetSortingOrder(layer);
    }

    private void Update() {
        //rotate
        Vector3 parentRotation = transform.parent.eulerAngles;
        transform.rotation = Quaternion.Euler(X_ROTATION, parentRotation.y, Z_ROTATION);

        //resize
        spriteRenderer.size = Vector2.one * size;

        //change icon
        spriteRenderer.sprite = sprite;
    }

    /// <param name="layer">The sorting layer</param>
    /// <returns>Sort number.</returns>
    private int GetSortingOrder(MapLayer layer) {
        switch (layer) {
            case MapLayer.Terrain: return 0;
            case MapLayer.Entity: return 1;
            case MapLayer.Landmark: return 2;
            case MapLayer.NPC: return 3;
            case MapLayer.Player: return 4;
            default: return 0;
        }
    }
}