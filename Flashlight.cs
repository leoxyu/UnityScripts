using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class Flashlight : MonoBehaviour {
    private Light2D flashlight;
    public GameObject flashlightHitbox;

    public bool lightEnabled;
    // Start is called before the first frame update
    void Awake() {
        flashlight = GetComponent<Light2D>();
        SetLight(false);
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown(KeyCode.LeftShift)) {
            if (lightEnabled) {
                SetLight(false);
            } else {
                SetLight(true);
            }


        }
    }

    private void SetLight(bool status) {
        flashlight.enabled = status;
        lightEnabled = status;
        flashlightHitbox.GetComponent<PolygonCollider2D>().enabled = status;
    }

}
