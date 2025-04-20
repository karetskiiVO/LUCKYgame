using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotator : MonoBehaviour {
    [SerializeField] 
    private Material skyboxMaterial;
    [SerializeField]
    private float rotationSpeed = 0.5f;

    private static readonly int Rotation = Shader.PropertyToID("_Rotation");

    void Update() {
        skyboxMaterial.SetFloat(Rotation, Time.time * rotationSpeed);
    }
}
