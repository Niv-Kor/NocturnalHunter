﻿using UnityEngine;

public class CamBaseRelocator : MonoBehaviour
{
    [Tooltip("The target that's being followed by the camera.")]
    [SerializeField] private Transform followTarget;

    private void Update() {
        transform.position = followTarget.position;
    }
}