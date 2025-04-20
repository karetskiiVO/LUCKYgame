using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour {
    [SerializeField]private Vector3 g = new(0f, -9.81f, 0f);
    [SerializeField]private bool active = false;
    [SerializeField]private int collisionsSize = 1024;
    private Collider field = null;

    Collider[] collisions;

    private void Start () {
        field = gameObject.GetComponent<Collider>();
        collisions ??= new Collider[collisionsSize];
    }

    public void ChangeMode () {
        active = !active;
    }

    private void FixedUpdate () {
        var count = Physics.OverlapBoxNonAlloc(field.bounds.center, field.bounds.extents, collisions, Quaternion.identity);

        for (var i = 0; i < count; i++) {
            if (collisions[i] == gameObject) continue;

            var rigidbody = collisions[i].attachedRigidbody;
            if (rigidbody == null) continue;

            if (active) {
                rigidbody.AddForce(g, ForceMode.Acceleration);
            } else {
                if ((collisions[i].includeLayers & LayerMask.GetMask("RandomFlow")) == 0) continue;

                rigidbody.AddForce(0.25f * Random.onUnitSphere, ForceMode.Acceleration);
                rigidbody.AddTorque(0.25f * rigidbody.inertiaTensor.magnitude * Random.onUnitSphere);
            }
        }
    }

    public void FlushToPoint (Vector3 point) {
        var count = Physics.OverlapBoxNonAlloc(field.bounds.center, field.bounds.extents, collisions, Quaternion.identity);

        for (var i = 0; i < count; i++) {
            if (collisions[i] == gameObject) continue;

            var rigidbody = collisions[i].attachedRigidbody;
            if (rigidbody == null) continue;

            rigidbody.AddForce((rigidbody.gameObject.transform.position - point).normalized * 10f, ForceMode.VelocityChange);
        }
    }

    public bool isActive { get => active; }
}
