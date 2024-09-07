using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatAI : MonoBehaviour {

    public bool active = true;
    public float max_offset_koef = 0.7f;  // offset from centre to hit ball in different direction
    public bool may_fail = false;
    public float fail_chance = 0.1f;
    public float fail_movement_max_delay = 0.5f;

    LevelController lc;
    BatController bat_c;
    BallController ball_c;
    Rigidbody2D rb;
    Rigidbody2D ball_rb;

    float target_pos_offset;
    float fail_suspend_time_left = 0f;

    // Start is called before the first frame update
    void Start() {
        lc = FindObjectOfType<LevelController>();
        bat_c = FindObjectOfType<BatController>();
        ball_c = FindObjectOfType<BallController>();
        rb = GetComponent<Rigidbody2D>();
        ball_rb = ball_c.GetComponent<Rigidbody2D>();
    }

    void FixedUpdate() {
        if (!active)
            return;

        if (!lc.IsLaunched()) {
            lc.Launch();
        }

        if (fail_suspend_time_left > 0f) {
            fail_suspend_time_left -= Time.fixedDeltaTime;
            return;
        }

        float target_pos_x = ball_rb.position.x + target_pos_offset;
        if (ball_rb.velocity.x > ball_rb.velocity.y)
            target_pos_x = ball_rb.position.x + Mathf.Abs(target_pos_offset) * Mathf.Sign(ball_rb.velocity.x);

        bat_c.MovementInput(target_pos_x);
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (!other.collider.CompareTag("Ball"))
            return;

        BoxCollider2D box_col = GetComponentInChildren<BoxCollider2D>();
        float bat_half_size = box_col.size.x * box_col.transform.lossyScale.x * 0.5f;
        target_pos_offset = Random.Range(-bat_half_size, bat_half_size) * max_offset_koef;

        if (may_fail && Random.Range(0f, 1f) <= fail_chance)
            fail_suspend_time_left = Random.Range(0f, fail_movement_max_delay);
    }
}
