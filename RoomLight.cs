using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
public class RoomLight : MonoBehaviour {
    public Light2D roomLight;
    public bool turnLightBackOff = true;
    public void Start() {
        roomLight.enabled = false;
    }
    public void TurnOn() {
        roomLight.enabled = true;
    }

    public void TurnOff() {
        if (turnLightBackOff) {
            roomLight.enabled = false;
        }
    }

}
