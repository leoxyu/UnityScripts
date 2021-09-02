using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    public GameObject hitEffect;

    public void Awake() {
        Destroy(gameObject, 3f);
    }

    public void OnCollisionEnter2D(Collision2D collision) {

        if (collision.gameObject.CompareTag("Enemy")) {

            Enemy tempEnemy = collision.gameObject.GetComponent<Enemy>();
            tempEnemy.TakeDamage(10); 
        }

        GameObject effect = Instantiate(hitEffect, transform.position, Quaternion.identity);
        Debug.Log("hit");
        Destroy(effect);
        Destroy(gameObject);

    }


}
