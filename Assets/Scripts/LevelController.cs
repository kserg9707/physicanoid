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

        initial_ball_c.transform.position = bat_c.transform.position + new Vector3(0f, 10f, 0f);
        Vector3 ball_point = initial_ball_c.GetComponent<Collider>().ClosestPoint(bat_c.transform.position);  // XXX ?
        Vector3 ball_offset = initial_ball_c.transform.position - ball_point;
        //Debug.Log("ball collider size: " + ball_offset.ToString());
        //Debug.Log("bat point: " + bat_c.GetComponent<Collider>().ClosestPoint(initial_ball_c.transform.position).ToString());
        initial_ball_c.transform.position = bat_c.GetComponentInChildren<Collider>().ClosestPoint(initial_ball_c.transform.position) + ball_offset;

        initial_ball_c.transform.SetParent(bat_c.transform);
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
