using UnityEngine;

namespace Constants
{
    public static class Layers
    {
        public static readonly LayerMask DEFAULT = LayerMask.GetMask("Default");
        public static readonly LayerMask TRANSPARENT_FX = LayerMask.GetMask("TransparentFX");
        public static readonly LayerMask IGNORE_RAYCAST = LayerMask.GetMask("Ignore Raycast");
        public static readonly LayerMask WATER = LayerMask.GetMask("Water");
        public static readonly LayerMask UI = LayerMask.GetMask("UI");
        public static readonly LayerMask GROUND = LayerMask.GetMask("Ground");
        public static readonly LayerMask PLAYER = LayerMask.GetMask("Player");
        public static readonly LayerMask CAMERA = LayerMask.GetMask("Camera");
        public static readonly LayerMask MINIMAP = LayerMask.GetMask("Minimap");
    }
}