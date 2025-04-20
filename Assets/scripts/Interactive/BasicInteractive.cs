using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasicInteractive : MonoBehaviour {
    // Start is called before the first frame update
    void Start() {
        GetComponent<Collider>().includeLayers |= LayerMask.GetMask("Interactive");
        
        InteractiveStart();
    }

    public virtual void InteractiveStart () {}

    public abstract BasicInteractive Take (GameObject player, Camera camera);
    public abstract void Release ();

    public virtual void Act () {}
}
