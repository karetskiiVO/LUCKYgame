using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour {
    [SerializeField]
    GravityField field; 
    [SerializeField]
    FPSController fPSController;

    public void Open () {
        field.FlushToPoint(transform.position);
        field.enabled = false;
        fPSController.enabled = false;
        gameObject.SetActive(false); // TODO: красивше
    }
}
