using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class BallController : MonoBehaviour {

    LevelController lc;
    public Rigidbody2D rb;

    float speed = 10f;  // keep ball velocity magnitude equal to this
    float cur_speed = 10f;  // keep ball velocity magnitude equal to this
    bool use_accel = false;
    float accel = 7f;  // keep ball velocity magnitude equal to this
    public float min_vertical_speed_mult = 0.125f;  // fraction of speed that is minimal vertical speed
    float vertical_speed_min { get { return speed * min_vertical_speed_mult; } }

    public bool brick_force_field = false;
    public float brick_force_field_force = 2.25f;
    public float brick_force_field_radius = 2.25f;
    public ParticleSystem force_enable_effect;
    public float force_enable_effect_len = 0.5f;
    public float ForceEnableEffectLen { get { return force_enable_effect_len; } }
    public ParticleSystem force_effect;

    bool freezed = true;  // is ball movement suspended (disables rigidbody simulation)
    Vector2 freezed_vel;  // velocity before freeze

    Vector2 last_vel;

    public Vector2 DEBUG_velocity;

    // Just get radius of ball collider
    public float GetColliderRadius() {
        CircleCollider2D col = GetComponent<CircleCollider2D>();
        return col.radius * col.transform.lossyScale.y;
    }

    // Set reigidbody velocity to (1,1) * speed
    public void ResetVelocity() {
        cur_speed = speed;
        rb.velocity = new Vector2(1f, 1f) * cur_speed;  // XXX good for initial ball but not spawned during play
    }

    public void ForceEnableEffectPlay() {
        if (force_enable_effect != null) {
            force_enable_effect.Play();
            AudioSource aus = force_enable_effect.GetComponent<AudioSource>();
            if (aus != null)
                aus.Play();
        }
    }

    public void ForceEnable() {
        brick_force_field = true;
        if (force_effect != null) {
            force_effect.Play();
            AudioSource aus = force_effect.GetComponent<AudioSource>();
            if (aus != null)
                aus.Play();
        }
    }

    // copied from BrickBase
    void Explode() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, 3f);
        foreach (Collider2D hit in colliders) {
            if (!hit.CompareTag("Brick"))  // XXX replace with layer mask?
                continue;
            Rigidbody2D rbo = hit.GetComponentInParent<Rigidbody2D>();

            if (rbo != null) {
                Vector2 force_vec = rbo.position - rb.position;
                float koef = Mathf.Clamp01((brick_force_field_radius * brick_force_field_radius - force_vec.sqrMagnitude) / (brick_force_field_radius * brick_force_field_radius));
                rbo.AddForce(force_vec.normalized * brick_force_field_force * koef, ForceMode2D.Impulse);
                // rbo.AddForceAtPosition((rbo.position - rb.position).normalized * destroy_explosion_force, rb.position, ForceMode2D.Impulse);
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        lc = FindObjectOfType<LevelController>();
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        speed = lc.LevelBallSpeed;

        GlobalGameSettings ggc = FindObjectOfType<GlobalGameSettings>();
        rb.mass = ggc.ball_base_mass;
        ResetVelocity();
    }

    void FixedUpdate() {
        KeepSpeed();
        if (!freezed && brick_force_field)
            Explode();
    }

    // Update is called once per frame
    void Update() {
        DEBUG_velocity = rb.velocity;
    }

    void OnTriggerEnter2D(Collider2D other) {
        if (!other.CompareTag("Border"))
            return;
        // Debug.Log("trigger overlap with " + other.gameObject.name);
        // XXX temp

        lc.BallFallCallback();
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
        if (!lc.PhysicsEnabled && other.collider.CompareTag("Brick")) {
            // Debug.Log("enter: last vel: " + last_vel.ToString() + ", bounced: " + rb.velocity.ToString());
            Vector2 p = other.GetContact(0).point; //.collider.ClosestPoint(rb.position);
            Vector2 delta = rb.position - p;
            // Debug.Log("enter: delta: " + delta.ToString() + ", col pos: " + p.ToString() + ", ball pos: " + rb.position.ToString());
            Vector2 res;
            if (Mathf.Approximately(delta.x, 0f))
                // res = new Vector2(last_vel.x, -last_vel.y);
                res = new Vector2(last_vel.x, Mathf.Abs(last_vel.y) * Mathf.Sign(delta.y));
            else if (Mathf.Approximately(delta.y, 0f))
                // res = new Vector2(-last_vel.x, last_vel.y);
                res = new Vector2(Mathf.Abs(last_vel.x) * Mathf.Sign(delta.x), last_vel.y);
            else if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                res = new Vector2(Mathf.Abs(last_vel.x) * Mathf.Sign(delta.x), last_vel.y);
            else
                res = new Vector2(last_vel.x, Mathf.Abs(last_vel.y) * Mathf.Sign(delta.y));
            rb.velocity = res;
            // Debug.Log("enter: corrected: " + rb.velocity.ToString() + ", res: " + res.ToString());
        }
        KeepSpeed();
    }

    void OnCollisionStay2D(Collision2D other) {
    }

    void OnCollisionExit2D(Collision2D other) {
        // order may be messed and exit does not provide contact points so use only enter
    }

    // Update rigidbody velocity to keep specified magnitude
    void KeepSpeed() {
        if (!freezed) {
            if (use_accel)
                cur_speed = Mathf.MoveTowards(rb.velocity.magnitude, speed, accel * Time.fixedDeltaTime);
            // else
            //     cur_speed = speed;
            rb.velocity = rb.velocity.normalized * cur_speed;
            if (Mathf.Abs(rb.velocity.y) < vertical_speed_min) {
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Sign(rb.velocity.normalized.y) * vertical_speed_min).normalized * cur_speed;
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
        freezed_vel = rb.velocity.normalized * cur_speed;
        rb.velocity = Vector3.zero;
        rb.simulated = false;
        GetComponent<Collider2D>().enabled = false;
        if (force_effect != null) {
            force_effect.Stop();
            force_effect.SetParticles(new ParticleSystem.Particle[0]);
            AudioSource aus = force_effect.GetComponent<AudioSource>();
            if (aus != null)
                aus.Stop();
        }
    }

    // enable collision and rigidbody simulation and restore velocity
    public void Unfreeze() {
        freezed = false;
        rb.velocity = freezed_vel;
        rb.simulated = true;
        GetComponent<Collider2D>().enabled = true;
        if (brick_force_field && force_effect != null) {
            force_effect.Play();
            AudioSource aus = force_effect.GetComponent<AudioSource>();
            if (aus != null)
                aus.Play();
        }
    }
}
