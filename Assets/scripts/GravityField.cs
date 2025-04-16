using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityField : MonoBehaviour {
    [SerializeField]private Vector3 g = new(0f, -9.81f, 0f);
    [SerializeField]private bool active = false;
    [SerializeField]private int collisionsSize = 1024;
    private float cooldown = 0;
    private Collider field = null;

    Collider[] collisions;

    private void Start () {
        field = gameObject.GetComponent<Collider>();
        collisions ??= new Collider[collisionsSize];
    }

    // Update is called once per frame
    private void Update () {
        if (Input.GetButton("Jump") && !(cooldown > 0)) {
            active = !active;
            cooldown = 0.2f;
        }

        cooldown = Mathf.Max(0, cooldown - Time.deltaTime);
    }

    private void FixedUpdate () {
        var count = Physics.OverlapBoxNonAlloc(field.bounds.center, field.bounds.extents / 2, collisions);

        for (var i = 0; i < count; i++) {
            if (collisions[i] == gameObject) continue;
            var rigidbody = collisions[i].attachedRigidbody;
            if (rigidbody == null) continue;

            if (active) {
                rigidbody.AddForce(rigidbody.mass * g);
            } else {
                if ((collisions[i].includeLayers & LayerMask.GetMask("RandomFlow")) == 0) continue;

                rigidbody.AddForce(
                    0.25f * rigidbody.mass * Random.onUnitSphere             
                );

                rigidbody.AddTorque(
                    0.25f * rigidbody.inertiaTensor.magnitude * Random.onUnitSphere
                );
            }
        }
    }
}
