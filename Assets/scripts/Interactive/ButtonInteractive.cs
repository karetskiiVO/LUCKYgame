using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonInteractive : BasicInteractive {
    [SerializeField]
    UnityEvent actions;

    public override BasicInteractive Take (GameObject player, Camera camera) {
        return null;
    }
    
    public override void Release () {
    }

    public override void Act () {
        actions?.Invoke();
    }
}