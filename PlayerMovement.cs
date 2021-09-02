using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basic movement made following this tut https://www.youtube.com/watch?v=LqnPeqoJRFY



public class PlayerMovement : MonoBehaviour {

    //-----GENERAL-----
    private float playerHeight = 2f;
    private Vector3 moveDirection;
    private Rigidbody rb;
    private bool isMoving = false;
    public bool hasGun = false;

    //Vector3 velocity;
    //public float gravity = -10f;
    //-----CAMERA-----
    [Header("Camera")]
    [SerializeField] Transform orientation;
    [SerializeField] Camera cam;
    float baseFov;
    float sprintFov;

    [Header("Ground Detection")]
    [SerializeField] LayerMask groundMask;

    //-----MOVEMENT-----
    [Header("Movement")]
    [SerializeField] private float baseSpeed = 4f;
    public float moveSpeed;


    //multipliers
    [Header("Multipliers")]
    [SerializeField] private float movementMultiplier = 10f;
    [SerializeField] private float airMultiplier = 0.4f;

    //drag
    [Header("Drag")]
    [SerializeField] private float groundDrag = 6f;
    [SerializeField] private float airDrag = 2f;

    private float horizontalMovement;
    private float verticalMovement;

    //-----JUMP-----
    [Header("Jumping")]
    public bool isGrounded;
    private float jumpSphereRadius = 0.3f;
    [SerializeField] private float jumpForce = 15f;

    //-----BINDS-----
    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprint = KeyCode.LeftShift;
    [SerializeField] KeyCode movingForwards = KeyCode.W;
    [SerializeField] KeyCode crouch = KeyCode.LeftControl;

    //-----SPRINT-----
    private float sprintMultiplier = 1.5f;

    //-----CROUCH-----
    private Vector3 crouchScale;
    private Vector3 baseScale;
    private bool isCrouching = false;
    private float crouchSpeed;
    private bool canStand = true;

    //-----SLOPE-----
    RaycastHit slopeHit;
    Vector3 slopeMoveDirection;


    private bool OnSlope() {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.5f)) {
            if (slopeHit.normal != Vector3.up) {
                return true;
            } else {
                return false;
            }
        }
        return false;
    }


    public void Start() {
        //cam = GetComponentInChildren<Camera>();
        rb = GetComponent<Rigidbody>();
        baseFov = cam.fieldOfView;
        sprintFov = baseFov + 30;
        moveSpeed = baseSpeed;
        crouchSpeed = baseSpeed * .7f;
        baseScale = transform.localScale;
        crouchScale = new Vector3(1, 0.5f, 1);

        rb.freezeRotation = true;
    }

    public void FixedUpdate() {
        MovePlayer();
    }

    void OnDrawGizmosSelected() {
        // Draw a yellow sphere at the transform's position
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position - new Vector3(0, playerHeight / 2, 0), jumpSphereRadius);
    }

    public void Update() {

        //ApplyGravity();

        isGrounded = Physics.CheckSphere(transform.position - new Vector3(0, playerHeight / 2, 0), jumpSphereRadius, groundMask);



        GetInput();
        ControlDrag();
        CheckMoving();

        //jump
        if (Input.GetKeyDown(jumpKey) && isGrounded) {
            Jump();
        }

        //shoot
        if (Input.GetButtonDown("Fire1") && hasGun) {
            Kickback();
        }

        //sprint
        if (Input.GetKey(sprint) && isMoving && !isCrouching && Input.GetKey(movingForwards)) {
            //change fov
            if (cam.fieldOfView < sprintFov) {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintFov, 10 * Time.deltaTime);
            }

            SetSpeed(baseSpeed * sprintMultiplier);
        } else {
            //change fov
            if (cam.fieldOfView > baseFov) {
                cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, baseFov, 10 * Time.deltaTime);
                if (!isCrouching) {
                    SetSpeed(baseSpeed);
                }
            }
        }

        //crouch
        if (Input.GetKeyDown(crouch) && !isCrouching) {
            StartCrouch();
        }
        if (Input.GetKeyUp(crouch)) {
            StopCrouch();
        }
        //stand up after coming out from low object
        if (!Input.GetKey(crouch) && isCrouching) {
            StopCrouch();
        }

        slopeMoveDirection = Vector3.ProjectOnPlane(moveDirection, slopeHit.normal);




    }

    /*private void ApplyGravity() {

        if (isGrounded && velocity.y < 0) {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        rb.AddForce(velocity * Time.deltaTime, ForceMode.Acceleration);
    }*/

    private void Kickback() {
        rb.AddForce(-cam.transform.forward.normalized * 20, ForceMode.Impulse);
    }
    private void StartCrouch() {

        isCrouching = true;
        playerHeight = 1f;
        SetSpeed(crouchSpeed);
        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - (playerHeight * .25f), transform.position.z);

    }
    private void StopCrouch() {
        //check for object above

        canStand = !(Physics.Raycast(transform.position, Vector3.up, playerHeight * 0.75f + 0.1f));
        if (canStand) {
            isCrouching = false;
            playerHeight = 2f;
            SetSpeed(baseSpeed);
            transform.localScale = baseScale;
            transform.position = new Vector3(transform.position.x, transform.position.y + (playerHeight * .25f), transform.position.z);
        }

    }

    public void SetSpeed(float speed) {
        moveSpeed = speed;
    }

    public void Jump() {
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }


    public void ControlDrag() {
        if (isGrounded) {

            rb.drag = groundDrag;
        } else {
            rb.drag = airDrag;
        }
    }

    public void CheckMoving() {
        if (rb.velocity.magnitude > 0) {
            isMoving = true;
        } else {
            isMoving = false;
        }
    }


    public void MovePlayer() {
        if (isGrounded && !OnSlope()) {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        } else if (isGrounded && OnSlope()) {
            rb.AddForce(slopeMoveDirection.normalized * moveSpeed * 1.5f * movementMultiplier, ForceMode.Acceleration);
        } else if (!isGrounded) {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }
    }

    public void GetInput() {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");
        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
    }

}


