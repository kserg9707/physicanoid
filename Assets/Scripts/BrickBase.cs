using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBase : MonoBehaviour {

    Rigidbody2D rb;
    LevelController lc;
    AudioSource sound_ball_hit;
    [SerializeField]
    AudioSource sound_brick_hit;

    public float mass_multiplier = 1f;

    public int hits_left = 1;
    public int score = 1;
    public float destroy_explosion_force = 10f;
    public float destroy_explosion_raduis = 3f;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = FindObjectOfType<GlobalGameSettings>().brick_base_mass * mass_multiplier;
        lc = FindObjectOfType<LevelController>();
        sound_ball_hit = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void Explode() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, destroy_explosion_raduis);
        foreach (Collider2D hit in colliders) {
            if (hit.CompareTag("Ball"))
                continue;
            Rigidbody2D rbo = hit.GetComponentInParent<Rigidbody2D>();

            if (rbo != null) {
                Vector2 force_vec = rbo.position - rb.position;
                float koef = Mathf.Clamp01((destroy_explosion_raduis * destroy_explosion_raduis - force_vec.sqrMagnitude) / (destroy_explosion_raduis * destroy_explosion_raduis));
                rbo.AddForce(force_vec.normalized * destroy_explosion_force * koef*5f, ForceMode2D.Impulse);
                // rbo.AddForceAtPosition((rbo.position - rb.position).normalized * destroy_explosion_force, rb.position, ForceMode2D.Impulse);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ball")) {
            if (sound_ball_hit != null)
                sound_ball_hit.Play();
            if ((--hits_left) == 0) {
                Explode();
                GetComponentInChildren<Collider2D>().gameObject.SetActive(false); //Destroy(gameObject);
                lc.BrickDestroyCallback(score);
            }
        } else if (other.gameObject.CompareTag("Brick")) {
            if (sound_brick_hit != null)
                sound_brick_hit.Play();
        }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Ball")) {
            if (sound_ball_hit != null)
                sound_ball_hit.Play();
            if ((--hits_left) == 0) {
                Explode();
                GetComponentInChildren<Collider2D>().gameObject.SetActive(false); //Destroy(gameObject);
                lc.BrickDestroyCallback(score);
            }
        }
    }
}
