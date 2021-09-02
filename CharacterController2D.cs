using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;

public class CharacterController2D : MonoBehaviour {

    public Camera cam;

    private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Vector2 mousePos;



    private void Awake() {
        rb = GetComponent<Rigidbody2D>();

    }

    // Update is called once per frame
    void Update() {
        mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        HandleMovement();
    }


    public void HandleMovement() {

        float moveX = 0f;
        float moveY = 0f;

        if (Input.GetKey(KeyCode.W)) {
            moveY = +1f;
        }
        if (Input.GetKey(KeyCode.S)) {
            moveY = -1f;
        }
        if (Input.GetKey(KeyCode.A)) {
            moveX = -1f;
        }
        if (Input.GetKey(KeyCode.D)) {
            moveX = +1f;

        }

        if (Input.GetKey(KeyCode.P)) {
            moveX = +1f;
            moveSpeed = 20f;
        }


        moveDir = new Vector2(moveX, moveY).normalized;

    }


    private void FixedUpdate() {

        Vector2 lookDir = mousePos - rb.position;
        float angle = Mathf.Atan2(lookDir.y, lookDir.x) * Mathf.Rad2Deg - 90f;
        rb.rotation = angle;

        rb.velocity = moveDir * moveSpeed;


    }




}
