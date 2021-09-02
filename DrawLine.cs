using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawLine : MonoBehaviour {

    public GameObject player;

    LineRenderer line;

    private void Start() {
        //For creating line renderer object
        line = new GameObject("Line").AddComponent<LineRenderer>();

        line.material.color = Color.white;
        line.startColor = Color.red;
        line.endColor = Color.red;
        line.startWidth = 0.1f;
        line.endWidth = 0.1f;
        line.positionCount = 2;
        line.useWorldSpace = true;

    }
    // Update is called once per frame
    void Update() {

        //For drawing line in the world space, provide the x,y,z values
        line.SetPosition(0, gameObject.transform.position); //x,y and z position of the starting point of the line
        line.SetPosition(1, player.transform.position); //x,y and z position of the end point of the line
    }

    private void OnDestroy() {
        Destroy(line);
    }
}
