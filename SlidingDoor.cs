using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

    [SerializeField] GameObject door;

    [SerializeField] Transform closedPos;
    [SerializeField] Transform openPos;

    public float doorSpeed = 20f;
    public bool isOpen = false;
    

    void Update() {
        if (isOpen) {
            door.transform.position = Vector3.MoveTowards(door.transform.position, openPos.position, doorSpeed * Time.deltaTime);
        } else {
            door.transform.position = Vector3.MoveTowards(door.transform.position, closedPos.position, doorSpeed * Time.deltaTime);
        }

    }


    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            if (!isOpen) {
                isOpen = true;


            }
        }
    }


    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            if (isOpen) {
                isOpen = false;

            }
        }

    }



}
