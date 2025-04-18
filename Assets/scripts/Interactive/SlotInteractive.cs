using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInteractive : BasicInteractive {
    FPSController fPSController;

    public override void InteractiveStart() {
            
    }

    public override void Take (GameObject player, Camera camera) {
        if (fPSController != null) return;

        fPSController = player.GetComponent<FPSController>();
        fPSController.enabled = false;
    }
    
    public override void Release () {
        if (fPSController == null) return;

        fPSController.enabled = true;
        fPSController = null;
    }

    public override void Act () {
        
    }
}
