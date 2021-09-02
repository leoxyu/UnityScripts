using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//source : https://www.youtube.com/watch?v=NbvcfMjAlQ4

public class Edge : MonoBehaviour {
    private void Awake() {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly == null) {
            poly = gameObject.AddComponent<PolygonCollider2D>();
        }
        Vector2[] points = poly.points;
        EdgeCollider2D edge = gameObject.AddComponent<EdgeCollider2D>();
        edge.points = points;
        Destroy(poly);
    }
}