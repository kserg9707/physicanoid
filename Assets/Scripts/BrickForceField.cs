using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickForceField : MonoBehaviour
{
    BoxCollider2D col;

    void OnTriggerStay2D(Collider2D other) {
        if (!other.CompareTag("Brick"))
            return;

        Rigidbody2D rb = other.GetComponentInParent<Rigidbody2D>();
        if (rb == null)
            return;

        float spread_y = col.size.y * transform.lossyScale.y * 0.5f;
        float mult = (1f - Mathf.Clamp01((rb.position.y - transform.position.y) / spread_y)) * 10f;
        rb.AddForce(Vector2.up * rb.mass * 10f, ForceMode2D.Force);
        Debug.Log("Forced " + (Vector2.up * rb.mass * 10f).ToString());
    }

    // Start is called before the first frame update
    void Start() {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
