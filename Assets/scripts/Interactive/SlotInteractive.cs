using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotInteractive : BasicInteractive {
    private SlotMachine slotMachine;

    public override void InteractiveStart() {
        slotMachine = GetComponent<SlotMachine>();
    }

    public override BasicInteractive Take (GameObject player, Camera camera) {
        return null;
    }
    
    public override void Release () {}

    public override void Act () {
        slotMachine.Rotate();
    }
}
