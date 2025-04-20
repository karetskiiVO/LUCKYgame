using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourseDB : MonoBehaviour {
    public static Dictionary<string, GameObject> resources = new();


    [SerializeField]
    private GameObject[] resourcesRaw;

    private void Awake () {
        foreach (var resource in resourcesRaw) {
            resources.Add(resource.GetComponent<InteractiveTag>().id, resource);
        }
    }
}
