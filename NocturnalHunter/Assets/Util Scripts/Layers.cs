using UnityEngine;

namespace Constants
{
    public static class Layers
    {
        public static readonly LayerMask DEFAULT = LayerMask.GetMask("Default");
        public static readonly LayerMask TRANSPARENT_FX = LayerMask.GetMask("TransparentFX");
        public static readonly LayerMask IGNORE_RAYCAST = LayerMask.GetMask("Ignore Raycast");
        public static readonly LayerMask WATER = LayerMask.GetMask("Water");
        public static readonly LayerMask PLANT = LayerMask.GetMask("Plant");
        public static readonly LayerMask UI = LayerMask.GetMask("UI");
        public static readonly LayerMask GROUND = LayerMask.GetMask("Ground");
        public static readonly LayerMask PLAYER = LayerMask.GetMask("Player");
        public static readonly LayerMask CAMERA = LayerMask.GetMask("Camera");
        public static readonly LayerMask MINIMAP = LayerMask.GetMask("Minimap");

        /// <summary>
        /// Check if a certain layer is contained in a layer mask.
        /// </summary>
        /// <param name="layer">The layer to check (as is 'gameObject.layer')</param>
        /// <param name="mask">A mask that may contain the layer</param>
        /// <returns>True if the mask contains the layer.</returns>
        public static bool ContainedInMask(int layer, LayerMask mask) {
            return (mask & 1 << layer) == 1 << layer;
        }
    }
}