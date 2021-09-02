using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using Pathfinding;

public class HallwayLights : MonoBehaviour {

    public Light2D light1;
    public Light2D light2;
    public Light2D light3;
    public GameObject doorLock;
    public GameObject enemy;
    AIPath aiPath;

    public Light2D doorLight1;
    public Light2D doorLight2;
    public Light2D doorLight3;
    public Light2D doorLight4;

    private Color red = new Color32(255, 0, 0, 255);
    private Color green;

    private bool firstTime = true;

    public void Start() {
        aiPath = enemy.GetComponent<AIPath>();
        green = doorLight1.color;
    }

    public void TurnOffLights() {
        if (firstTime) {
            StartCoroutine(FlickerLights());
            firstTime = false;
        }

    }

    private void LightState(bool b) {
        light1.enabled = b;
        light2.enabled = b;
        light3.enabled = b;

        if (b) {
            doorLight1.color = green;
            doorLight2.color = green;
            doorLight3.color = green;
            doorLight4.color = green;
        } else {
            doorLight1.color = red;
            doorLight2.color = red;
            doorLight3.color = red;
            doorLight4.color = red;
        }
    }

    private IEnumerator FlickerLights() {
        LightState(false);
        yield return new WaitForSeconds(0.03f);
        LightState(true);
        yield return new WaitForSeconds(1f);
        LightState(false);
        yield return new WaitForSeconds(0.1f);
        LightState(true);
        yield return new WaitForSeconds(0.6f);
        LightState(false);
        yield return new WaitForSeconds(0.03f);
        LightState(true);
        yield return new WaitForSeconds(0.2f);
        LightState(false);
        yield return new WaitForSeconds(0.05f);
        LightState(true);
        yield return new WaitForSeconds(1f);
        LightState(false);
        Events();
    }

    /*private IEnumerator Flicker(float waitOn, float waitOff) {

    }*/

    public void Events() {
        Destroy(doorLock);
        aiPath.canMove = true;
    }

}
