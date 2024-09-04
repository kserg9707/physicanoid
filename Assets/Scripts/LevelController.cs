using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelController : MonoBehaviour {

    bool started = false;

    BallController initial_ball_c;
    BatController bat_c;

    // Start is called before the first frame update
    void Start() {
        initial_ball_c = FindObjectOfType<BallController>();
        bat_c = FindObjectOfType<BatController>();
        Rigidbody2D ball_rb = initial_ball_c.GetComponent<Rigidbody2D>();
        Rigidbody2D bat_rb = bat_c.GetComponent<Rigidbody2D>();

        Vector2 KEK = ball_rb.position + new Vector2(0f, -5f);
        Collider2D KEKEK = ball_rb.GetComponent<Collider2D>();
        Vector2 ball_point = KEKEK.ClosestPoint(KEK);
        Vector2 ball_offset = ball_rb.position - ball_point;
        Debug.Log("KEK: " + KEK.ToString()); //ball_offset.ToString());
        Debug.Log("ball collider point: " + ball_point.ToString()); //ball_offset.ToString());
        Debug.Log("ball collider size: " + ball_offset.ToString());
        Debug.Log("bat point: " + bat_c.GetComponentInChildren<Collider2D>().ClosestPoint(bat_rb.position + new Vector2(0f, 5f)).ToString());
        ball_rb.MovePosition(bat_c.GetComponentInChildren<Collider2D>().ClosestPoint(ball_rb.position + new Vector2(0f, 5f)) + ball_offset);

        initial_ball_c.transform.SetParent(bat_c.transform);
        initial_ball_c.Freeze();
        initial_ball_c.transform.position = ball_rb.position;
    }

    // Update is called once per frame
    void Update() {
        if (!started) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                started = true;
                initial_ball_c.transform.SetParent(null);
                initial_ball_c.Unfreeze();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}
