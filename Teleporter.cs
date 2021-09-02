using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Teleporter : MonoBehaviour {

    public bool lvlTeleporter;
    private void OnTriggerEnter(Collider collision) {

        if (collision.CompareTag("Player") && lvlTeleporter) {
            GameObject.Find("LevelLoader").GetComponent<LevelLoader>().LoadNextLevel();
        }
    }



}





