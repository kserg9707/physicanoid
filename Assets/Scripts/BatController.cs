using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BatController : MonoBehaviour {

    Rigidbody rb;

    public float move_speed = 10f;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate() {
        // XXX axis? + mouse?
        if (Input.GetKey(KeyCode.LeftArrow)) {
            rb.MovePosition(rb.position + new Vector3(-move_speed * Time.fixedDeltaTime, 0f, 0f));
        }
        if (Input.GetKey(KeyCode.RightArrow)) {
            rb.MovePosition(rb.position + new Vector3(move_speed * Time.fixedDeltaTime, 0f, 0f));
        }
    }

    void OnCollisionEnter(Collision other) {
        if (other.gameObject.CompareTag("Ball")) {
            float x_size = GetComponentInChildren<Collider>().bounds.size.x;
            float offset_percent = (other.GetContact(0).point - transform.position).x * 2 / x_size;
            Rigidbody ball_rb = other.gameObject.GetComponent<Rigidbody>();
            float magn = ball_rb.velocity.magnitude;
            ball_rb.velocity += new Vector3(ball_rb.velocity.magnitude * offset_percent, 0f, 0f);
            ball_rb.velocity = ball_rb.velocity.normalized * magn;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
