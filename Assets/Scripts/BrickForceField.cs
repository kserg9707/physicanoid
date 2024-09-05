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

        float spread_y = col.size.y * 0.5f * transform.lossyScale.y;
        float mult = (rb.position.y - transform.position.y) / spread_y;
        rb.AddForce(Vector2.up * rb.mass * mult, ForceMode2D.Force);
    }

    // Start is called before the first frame update
    void Start() {
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
