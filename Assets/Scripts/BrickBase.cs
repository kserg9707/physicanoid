using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBase : MonoBehaviour {

    Rigidbody2D rb;
    LevelController lc;
    AudioSource sound_ball_hit;
    [SerializeField]
    AudioSource sound_brick_hit;

    public float mass_multiplier = 1f;  // mass read from GlobalGameSettings and set at start

    public int hits_left = 1;  // hits until destrucion
    public int score = 1;  // score on destruction
    public float destroy_explosion_force = 10f;  // apply force to other bricks on destruction (ignore mass)
    public float destroy_explosion_raduis = 3f;  // radius of force appliance
    public Color color = Color.green;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.mass = FindObjectOfType<GlobalGameSettings>().brick_base_mass * mass_multiplier;
        lc = FindObjectOfType<LevelController>();
        sound_ball_hit = GetComponent<AudioSource>();

        ApplyColor();
    }

    // Update is called once per frame
    void Update() {
        
    }

    void ApplyColor() {
        MeshRenderer mr = GetComponentInChildren<MeshRenderer>();
        mr.material.color = color;
    }

    // apply force to nearest bricks
    void Explode() {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(rb.position, destroy_explosion_raduis);
        foreach (Collider2D hit in colliders) {
            if (!hit.CompareTag("Brick"))  // XXX replace with layer mask?
                continue;
            Rigidbody2D rbo = hit.GetComponentInParent<Rigidbody2D>();

            if (rbo != null) {
                Vector2 force_vec = rbo.position - rb.position;
                float koef = Mathf.Clamp01((destroy_explosion_raduis * destroy_explosion_raduis - force_vec.sqrMagnitude) / (destroy_explosion_raduis * destroy_explosion_raduis));
                rbo.AddForce(force_vec.normalized * destroy_explosion_force * koef, ForceMode2D.Impulse);
                // rbo.AddForceAtPosition((rbo.position - rb.position).normalized * destroy_explosion_force, rb.position, ForceMode2D.Impulse);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ball")) {
            // ball hit sound
            if (sound_ball_hit != null)
                sound_ball_hit.Play();
            // reduce health
            if ((--hits_left) == 0) {
                // explode and deactivate mesh and collider (via child to keep sound playing)
                Explode();
                GetComponentInChildren<Collider2D>().gameObject.SetActive(false); //Destroy(gameObject);
                lc.BrickDestroyCallback(score);
            }
        } else if (other.gameObject.CompareTag("Brick")) {
            // bricks hit sound
            if (sound_brick_hit != null)
                sound_brick_hit.Play();
        }
    }
}
