using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class TakableIntaractive : BasicInteractive {
    [SerializeField]
    [Range(0, 2)]
    float scale = 0.4f;

    [SerializeField]
    Vector3 screenCoords = new(0.45f, -0.34f, 0.6f);

    private Rigidbody rb;
    private new Collider collider;

    public override void InteractiveStart() {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    private Transform prevParent;

    private GameObject player;
    private new Camera camera;
    public override BasicInteractive Take (GameObject player, Camera camera) {
        if (this.player != null) return null;
        prevParent = transform.parent;

        this.player = player;
        this.camera = camera;

        rb.isKinematic = true;
        collider.enabled = false;
        transform.position = camera.ScreenToWorldPoint(
            Vector3.Scale(screenCoords + Vector3.one / 2f, new Vector3(Screen.width, Screen.height, 1))
        );
        transform.localScale = transform.localScale * scale;
        transform.parent = camera.transform;
    
        return this;
    }

    public override void Release () {
        if (player == null) return;
        player = null;

        transform.position += camera.transform.forward;

        transform.parent = prevParent;
        rb.isKinematic = false;
        collider.enabled = true;
        transform.localScale = transform.localScale / scale;

        rb.AddForce(camera.transform.forward * 7f, ForceMode.Impulse);
    }
}
