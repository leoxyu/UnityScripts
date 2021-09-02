
using Pathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public bool affectedByLight = false;
    private bool inLight = false;
    public int health;
    AIPath aiPath;

    // Start is called before the first frame update
    void Start() {
        aiPath = GetComponent<AIPath>();
    }

    public void Update() {
        if (health <= 0) {
            Die();
        }
    }

    private void Die() {
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D other) {
        Debug.Log("somethign hit enemy");

        Collider2D tempCollider = other.collider;
        switch (tempCollider.tag) {
            case "Bullet":
                break;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        inLight = true;
        if (affectedByLight) {
            Debug.Log("trigger hit enemy (cant move)");
            aiPath.canMove = false;
        }
    }
    private void OnTriggerExit2D(Collider2D other) {
        inLight = false;
        if (affectedByLight) {
            Debug.Log("trigger left enemy (wait to move)");
            StartCoroutine(WaitBeforeMove(0.5f));
        }
    }

    private IEnumerator WaitBeforeMove(float time) {
        Debug.Log("can move");
        yield return new WaitForSeconds(time);
        if (!inLight) {
            aiPath.canMove = true;
        }

    }

    public void TakeDamage(int i) {
        Debug.Log("taken " + i + " damage");
        health -= i;
    }

}
