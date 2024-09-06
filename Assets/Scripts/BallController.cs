using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour {

    Rigidbody2D rb;

    public float speed = 10f;  // keep ball velocity magnitude equal to this
    public float min_vertical_speed_mult = 0.125f;  // fraction of speed that is minimal vertical speed
    float vertical_speed_min { get { return speed * min_vertical_speed_mult; } }

    bool freezed = true;  // is ball movement suspended (disables rigidbody simulation)
    Vector2 freezed_vel;  // velocity before freeze

    Vector2 last_vel;

    public Vector2 DEBUG_velocity;

    // Just get radius of ball collider
    public float GetColliderRadius() {
        CircleCollider2D col = rb.GetComponent<CircleCollider2D>();
        return col.radius * col.transform.lossyScale.y;
    }

    // Set reigidbody velocity to (1,1) * speed
    public void ResetVelocity() {
        rb.velocity = new Vector2(1f, 1f) * speed;  // XXX good for initial ball but not spawned during play
    }

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();

        rb.mass = FindObjectOfType<GlobalGameSettings>().ball_base_mass;  // XXX ?
        ResetVelocity();
    }

    void FixedUpdate() {
        KeepSpeed();
    }

    // Update is called once per frame
    void Update() {
        DEBUG_velocity = rb.velocity;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Border"))
            return;
        Debug.Log("trigger overlap with " + other.gameObject.name);
        // XXX temp

        FindObjectOfType<LevelController>().BallFallCallback();
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
        KeepSpeed();
    }
    void OnCollisionStay2D(Collision2D other) {
        KeepSpeed();
    }
    void OnCollisionExit2D(Collision2D other) {
        KeepSpeed();
    }

    // Update rigidbody velocity to keep specified magnitude
    void KeepSpeed() {
        if (!freezed) {
            rb.velocity = rb.velocity.normalized * speed;
            if (Mathf.Abs(rb.velocity.y) < vertical_speed_min) {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.normalized.y) * vertical_speed_min).normalized * speed;
            }
            last_vel = rb.velocity;
        }
    }

    // set rigidbody position or transform position according to freezed state
    public void SetPosition(Vector3 pos) {
        if (freezed)
            transform.position = pos;
        else
            rb.MovePosition(pos);
    }

    // disable collision and rigidbody simulation and remember velocity
    public void Freeze() {
        freezed = true;
        freezed_vel = rb.velocity;
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
    }

    // enable collision and rigidbody simulation and restore velocity
    public void Unfreeze() {
        freezed = false;
        rb.velocity = freezed_vel;
        rb.simulated = true;
        GetComponent<Collider2D>().enabled = true;
    }
}
