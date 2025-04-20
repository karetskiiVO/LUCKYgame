using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComandPost : BasicInteractive {
    public override void Release() {}

    public override BasicInteractive Take(GameObject player, Camera camera) => null;

    public override void Act () {
    }

    
    void OnCollisionEnter (Collision collision) {
        if (collision.gameObject.TryGetComponent<InteractiveTag>(out var idComponent)) {
            if (idComponent.id == "cup") {
                GameObject.Find("door").GetComponent<Door>().Open(); // Ты не хочешь играть...
            } else {
                // для игры нужны трофеи
            }
        }
    }

}
