using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour {

    Rigidbody2D rb;

    public float speed = 10f;
    float vertical_speed_min { get { return speed * 0.1f; } }

    bool freezed = true;
    Vector2 freezed_vel;

    Vector2 last_vel;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        freezed_vel = new Vector2(1f, 1f) * speed;  // XXX good for initial ball but not spawned during play
    }

    void FixedUpdate() {
        if (!freezed) {
            rb.velocity = rb.velocity.normalized * speed;
            if (Mathf.Abs(rb.velocity.y) < vertical_speed_min)
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.normalized.y) * vertical_speed_min).normalized;
            last_vel = rb.velocity;
        }
    }

    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter2D(Collider2D other) {
        Debug.Log("trigger overlap with " + other.gameObject.name);
    }

    void OnTriggerStay2D(Collider2D other) {
        Debug.Log("trigger stay with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Border")) {  // XXX class with tags?
            Vector2 pos = transform.position;
            Vector2 collisionPoint = other.ClosestPoint(transform.position);
            // ContactPoint cp = other.GetContact(0);
            Vector2 normal = (collisionPoint - pos).normalized;

            if (Vector2.Dot(rb.velocity, normal) > 0f) {
                Debug.Log("velocity change: " + rb.velocity.ToString() + " -> " + Vector3.Reflect(rb.velocity, normal).ToString() + "; normal " + normal.ToString());
                rb.velocity = Vector3.Reflect(rb.velocity, normal);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Border")) {
            if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(last_vel.x))
                rb.velocity.Set(Mathf.Abs(last_vel.x) * Mathf.Sign(rb.velocity.x), rb.velocity.y);
            last_vel = rb.velocity;
        }
    }

    public void Freeze() {
        freezed = true;
        freezed_vel = rb.velocity;
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
    }
    public void Unfreeze() {
        freezed = false;
        rb.velocity = freezed_vel;
        rb.simulated = true;
        GetComponent<Collider2D>().enabled = true;
    }
}
