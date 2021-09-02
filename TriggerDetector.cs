using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDetector : MonoBehaviour {

    public bool hasKey;
    void Start() {
        hasKey = false;
    }
    public void SetTrue() {
        Debug.Log("picked up key");
        hasKey = true;
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        //for picking up basement door key
        KeyScript key = collision.GetComponent<KeyScript>();
        if (key != null) {
            SetTrue();
            Destroy(key.gameObject);
        }
        //for opening locked door
        KeyDoor keyDoor = collision.GetComponent<KeyDoor>();
        if (keyDoor != null) {
            if (hasKey) {
                keyDoor.OpenDoor(); 
            } else {
                keyDoor.StillLocked();
            }
        }
        //turning off/on lights in rooms
        RoomLight roomLight = collision.GetComponent<RoomLight>();
        if (roomLight != null) {
            roomLight.TurnOn();
            //play sound
        }
        //triggering lights to shut off in basement
        HallwayLights hallwayLights = collision.GetComponent<HallwayLights>();
        if (hallwayLights != null) {
            hallwayLights.TurnOffLights();
        }
        if (collision.CompareTag("LevelTransition")) {
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadNextLevel();
        }



    }

    private void OnTriggerExit2D(Collider2D collision) {
        RoomLight roomLight = collision.GetComponent<RoomLight>();
        if (roomLight != null) {
            roomLight.TurnOff();
            //play sound
        }

    }

}


