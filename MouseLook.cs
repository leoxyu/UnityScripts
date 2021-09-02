using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basics made following this tut https://www.youtube.com/watch?v=E5zNi_SSP_w

public class MouseLook : MonoBehaviour {

    [SerializeField] WallRun wallRun;

    [SerializeField] private float sensX = 100;
    [SerializeField] private float sensY = 100;

    [SerializeField] Transform cam;
    [SerializeField] Transform orientation;

    private float mouseX;
    private float mouseY;

    private float multiplier = 0.01f;

    private float xRotation;
    private float yRotation;

    // Start is called before the first frame update
    void Start() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update() {
        GetInput();

        cam.transform.rotation = Quaternion.Euler(xRotation, yRotation, wallRun.tilt);
        orientation.transform.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    private void GetInput() {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensX * multiplier;
        xRotation -= mouseY * sensY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);
    }
}
