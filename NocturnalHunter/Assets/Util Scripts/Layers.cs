using UnityEngine;

namespace Constants
{
    public static class Layers
    {
        public static readonly int DEFAULT = LayerMask.NameToLayer("Default");
        public static readonly int TRANSPARENT_FX = LayerMask.NameToLayer("TransparentFX");
        public static readonly int IGNORE_RAYCAST = LayerMask.NameToLayer("Ignore Raycast");
        public static readonly int WATER = LayerMask.NameToLayer("Water");
        public static readonly int UI = LayerMask.NameToLayer("UI");
        public static readonly int GROUND = LayerMask.NameToLayer("Ground");
        public static readonly int PLAYER = LayerMask.NameToLayer("Player");
        public static readonly int CAMERA = LayerMask.NameToLayer("Camera");
        public static readonly int MINIMAP = LayerMask.NameToLayer("Minimap");
    }
}