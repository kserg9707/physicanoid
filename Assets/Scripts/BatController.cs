using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BatController : MonoBehaviour {

    Rigidbody2D rb;

    public float move_speed = 10f;

    public float GetUpperPlaneY() {
        BoxCollider2D col = GetComponentInChildren<BoxCollider2D>();
        return col.size.y * 0.5f * col.transform.lossyScale.y;
        // return rb.ClosestPoint(rb.position + Vector2.up * transform.lossyScale.y * 10f).y;
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        // XXX axis? + mouse?
        if (Input.GetKey(KeyCode.LeftArrow)) {
            rb.MovePosition(rb.position + new Vector2(-move_speed * Time.fixedDeltaTime, 0f));
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            rb.MovePosition(rb.position + new Vector2(move_speed * Time.fixedDeltaTime, 0f));
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ball")) {
            float x_size = GetComponentInChildren<Collider2D>().bounds.size.x;
            Vector2 pos = transform.position;
            float offset_percent = (other.GetContact(0).point - pos).x * 2 / x_size;
            Rigidbody2D ball_rb = other.gameObject.GetComponent<Rigidbody2D>();
            float magn = ball_rb.velocity.magnitude;
            ball_rb.velocity += new Vector2(ball_rb.velocity.magnitude * offset_percent, 0f);
            ball_rb.velocity = ball_rb.velocity.normalized * magn;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
