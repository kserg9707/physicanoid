using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LevelController : MonoBehaviour {

    bool started = false;
    int player_score = 0;  // XXX not here?

    public int fall_score_lost = 3;  // score lost at ball fall

    UIController ui_c;

    BallController initial_ball_c;
    BatController bat_c;
    BrickBase[] bricks;

    public void BrickDestroyCallback(int score) {
        player_score += score;
        ui_c.UpdateScoreCB(player_score);
    }

    public void BallFallCallback() {
        player_score -= fall_score_lost;
        ui_c.UpdateScoreCB(player_score);
        ResetBall();
    }

    void ResetBall() {
        initial_ball_c.ResetVelocity();
        SetBallToBat();
        started = false;
    }

    void SetBallToBat() {
        Vector3 target_pos = bat_c.transform.position + new Vector3(0f, bat_c.GetUpperPlaneY() + initial_ball_c.GetColliderRadius());

        initial_ball_c.transform.SetParent(bat_c.transform);
        initial_ball_c.Freeze();
        initial_ball_c.SetPosition(target_pos);
    }

    void SetBricksFreezed(bool freezed) {
        foreach (BrickBase b in bricks) {
            if (!b.IsDestroyed())
                b.GetComponent<Rigidbody2D>().constraints = (freezed) ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
        }
    }

    void ExitLevel() {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start() {
        ui_c = FindObjectOfType<UIController>();
        ui_c.UpdateScoreCB(player_score);

        initial_ball_c = FindObjectOfType<BallController>();
        bat_c = FindObjectOfType<BatController>();
        bricks = FindObjectsOfType<BrickBase>();

        SetBallToBat();

        SetBricksFreezed(true);
        StartCoroutine(UnfreezeBricksAfterTime(5f));
    }

    IEnumerator UnfreezeBricksAfterTime(float time) {
        yield return new WaitForSeconds(time);
        SetBricksFreezed(false);
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

        if (Input.GetKeyDown(KeyCode.Escape))
            ExitLevel();
    }
}
