using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactor : MonoBehaviour {
    [SerializeField]
    [Range(0, 5)]
    private float interactiveDistance = 1.6f;
    
    private Transform cameraTransform;
    GameObject cursor;
    
    private BasicInteractive currInteractive = null;

    void Start () {
        //Cursor.lockState = CursorMode.
        cursor = GameObject.Find("cursor");
        cameraTransform = Camera.main.transform;
    }

    void Update() {
        if (currInteractive == null) {
            NonActiveUpdate();
        } else {
            ActiveUpdate();
        }
    }

    private void ActiveUpdate () {
        if (Input.GetKeyUp(KeyCode.Mouse1)) {
            currInteractive.Release();
            currInteractive = null;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0)) {
            currInteractive.Act();
        }
    }

    private void NonActiveUpdate () {
        if (
            Physics.Raycast(
                cameraTransform.position, 
                cameraTransform.forward, 
                out var hit, 
                interactiveDistance
            ) &&
            (hit.collider.includeLayers & LayerMask.GetMask("Interactive")) != 0
        ) {
            cursor.SetActive(true);

            if (Input.GetKeyUp(KeyCode.Mouse1)) {
                currInteractive = hit.collider.gameObject.GetComponent<BasicInteractive>();
                currInteractive.Take(gameObject, Camera.main);
                Debug.Log("aboba");
                cursor.SetActive(false);
            }
        } else {
            cursor.SetActive(false);
        }
    }
}
