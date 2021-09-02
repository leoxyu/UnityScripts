using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class CharacterShooting : MonoBehaviour {
    public Transform firePoint;
    public GameObject bulletPrefab;
    public GameObject shellPrefab;
    public Camera cam;
    public Light2D muzzleFlash;

    public int ammo = 6; // TODO: implement reload
    private bool reloading = false;

    public bool canShoot = true;
    public float bulletForce = 300f;
    private readonly float cooldown = 0.5f;

    private void Start() {
        muzzleFlash.enabled = false;
    }
    // Update is called once per frame
    void Update() {
        if (Input.GetMouseButtonDown(0) && canShoot && !reloading) {
            if (ammo > 0) {
                Shoot();
                canShoot = false;
                ShakeCamera(.1f, .05f);
                StartCoroutine(ShootCooldown(cooldown));
            }
        }
        if (Input.GetKey(KeyCode.R) && (ammo < 6)) {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload() {
        if (!reloading) {
            Debug.Log("reloading");
            reloading = true;
            yield return new WaitForSeconds(1f);
            ammo = 6;
            reloading = false;
        }
    }

    IEnumerator ShootCooldown(float s) {
        yield return new WaitForSeconds(s);
        canShoot = true;
    }

    private void Shoot() {

        ammo--;
        //muzzle flash

        StartCoroutine(MuzzleFlash());

        //create bullet and add force
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);

        //create shell and add force
        GameObject shell = Instantiate(shellPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb2 = shell.GetComponent<Rigidbody2D>();


    }

    IEnumerator MuzzleFlash() {
        muzzleFlash.enabled = true;
        yield return new WaitForSeconds(0.05f);
        muzzleFlash.enabled = false;
    }

    // Screen Shake 
    private static void ShakeCamera(float intensity, float timer) {
        Vector3 lastCameraMovement = Vector3.zero;
        FunctionUpdater.Create(delegate () {
            timer -= Time.unscaledDeltaTime;
            Vector3 randomMovement = new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized * intensity;
            Camera.main.transform.position = Camera.main.transform.position - lastCameraMovement + randomMovement;
            lastCameraMovement = randomMovement;
            return timer <= 0f;
        }, "CAMERA_SHAKE");
    }
}
