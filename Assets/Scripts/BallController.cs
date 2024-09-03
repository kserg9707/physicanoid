using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour {

    Rigidbody rb;

    public float speed = 10f;
    float vertical_speed_min { get { return speed * 0.1f; } }

    bool freezed = true;
    Vector3 freezed_vel;

    Vector3 last_vel;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        Freeze();
        freezed_vel = new Vector3(1f, 1f, 0f) * speed;  // XXX good for initial ball but not spawned during play
    }

    void FixedUpdate() {
        if (!freezed) {
            rb.velocity = rb.velocity.normalized * speed;
            if (Mathf.Abs(rb.velocity.y) < vertical_speed_min)
                rb.velocity = new Vector3(rb.velocity.x, Mathf.Sign(rb.velocity.normalized.y) * vertical_speed_min, rb.velocity.z).normalized;
            last_vel = rb.velocity;
        }
    }

    // Update is called once per frame
    void Update() {
    }

    void OnTriggerEnter(Collider other) {
        Debug.Log("trigger overlap with " + other.gameObject.name);
    }

    void OnTriggerStay(Collider other) {
        Debug.Log("trigger stay with " + other.gameObject.name);
        if (other.gameObject.CompareTag("Border")) {  // XXX class with tags?
            Vector3 collisionPoint = other.ClosestPoint(transform.position);
            // ContactPoint cp = other.GetContact(0);
            Vector3 normal = (collisionPoint - transform.position).normalized;

            if (Vector3.Dot(rb.velocity, normal) > 0f) {
                Debug.Log("velocity change: " + rb.velocity.ToString() + " -> " + Vector3.Reflect(rb.velocity, normal).ToString() + "; normal " + normal.ToString());
                rb.velocity = Vector3.Reflect(rb.velocity, normal);
            }
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Border")) {
            if (Mathf.Abs(rb.velocity.x) < Mathf.Abs(last_vel.x))
                rb.velocity.Set(Mathf.Abs(last_vel.x) * Mathf.Sign(rb.velocity.x), rb.velocity.y, rb.velocity.z);
            last_vel = rb.velocity;
        }
    }

    public void Freeze() {
        freezed = true;
        freezed_vel = rb.velocity;
        rb.velocity = Vector3.zero;
        GetComponent<Collider>().enabled = false;
    }
    public void Unfreeze() {
        freezed = false;
        rb.velocity = freezed_vel;
        GetComponent<Collider>().enabled = true;
    }
}
