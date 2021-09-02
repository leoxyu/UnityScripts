using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class KeyDoor : MonoBehaviour {
    public bool keyCard;
    public Light2D lightIndicator;

    private void OnTriggerEnter2D(Collider2D collision) {
        KeyScript key = collision.GetComponent<KeyScript>();
        if (key != null) {
            Destroy(key.gameObject);
        }
    }
    public void OpenDoor() {
        Destroy(GetComponent<BoxCollider2D>());
    }

    public void StillLocked() {
     
        if (keyCard) {
           
            StartCoroutine(DoorLocked());
        } else {
            //play animation (locked sound?)
        }

    }
    public IEnumerator DoorLocked() {
        Color temp = lightIndicator.color;
        lightIndicator.color = new Color32(255, 0, 0, 255);
        yield return new WaitForSeconds(1f);
        lightIndicator.color = temp;

    }

}
