using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LevelController : MonoBehaviour {

    bool started = false;  // level started (first launch occured)
    bool launched = false;  // ball launched
    int player_score = 0;  // XXX not here?
    bool level_win = false;
    int player_lives = 3;  // initial player lives
    bool level_lose = false;

    // level config
    public float field_width = 18f;
    public float default_ball_speed = 15f;
    public bool physics_enabled = false;  // is physics enabled on level
    public float physics_enable_delay = 7f;  // seconds before physics enabling
    public float physics_enable_effect_delay = 6f;  // also from level start

    public ParticleSystem bricks_unfreeze_effect;
    public AudioSource sound_ball_fall;
    public AudioSource sound_win;
    public AudioSource sound_lose;

    public int fall_score_lost = 3;  // score lost at ball fall

    public float LevelBallSpeed { get { return (physics_enabled) ? default_ball_speed * 0.7f : default_ball_speed; } }

    GlobalGameSettings ggc;
    UIController ui_c;

    BallController initial_ball_c;
    BatController bat_c;
    BrickBase[] bricks;

    int bricks_left = 0;

    public float GetBatAllowedRange() {
        return field_width * 0.5f - initial_ball_c.GetColliderRadius();;
    }

    // call it from brick only on destruction, this method tracks win conditions and score
    public void BrickDestroyCallback(int score) {
        if (!launched)
            return;
        player_score += score;
        ui_c.UpdateScoreCB(player_score);
        bricks_left--;
        if (bricks_left <= 0)
            LevelWin();
    }

    // call it on ball fall, this method tracks lose conditions and resets ball
    public void BallFallCallback() {
        if (level_win)
            ResetBall();

        if (!launched)
            return;

        sound_ball_fall.Play();

        player_score -= fall_score_lost;
        ui_c.UpdateScoreCB(player_score);

        player_lives--;
        ui_c.SetLivesCount(player_lives);
        if (player_lives <= 0)
            LevelLose();

        if (!level_lose)
            ResetBall();
        else
            initial_ball_c.Freeze();
    }

    // suspend game and set ball to bat
    void ResetBall() {
        initial_ball_c.ResetVelocity();
        SetBallToBat();
        launched = false;
    }

    // set ball to bat and freeze it
    void SetBallToBat() {
        Vector3 target_pos = bat_c.transform.position + new Vector3(0f, bat_c.GetUpperPlaneY() + initial_ball_c.GetColliderRadius());

        initial_ball_c.transform.SetParent(bat_c.transform);
        initial_ball_c.Freeze();
        initial_ball_c.SetPosition(target_pos);
    }

    // freeze or unfreeze all blocks
    void SetBricksFreezed(bool freezed) {
        foreach (BrickBase b in bricks) {
            if (!b.IsDestroyed())
                b.GetComponent<Rigidbody2D>().constraints = (freezed) ? RigidbodyConstraints2D.FreezeAll : RigidbodyConstraints2D.None;
        }
    }

    // called on win condition
    void LevelWin() {
        if (!launched)
            return;
        level_win = true;
        launched = false;
        ui_c.SetStateMessage("Win");
        if (sound_win != null)
            sound_win.Play();
    }

    // called on lose condition
    void LevelLose() {
        if (!launched)
            return;
        level_lose = true;
        launched = false;
        ui_c.SetStateMessage("Lose");
        if (sound_lose != null)
            sound_lose.Play();
    }

    void ExitLevel() {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start() {
        ggc = FindObjectOfType<GlobalGameSettings>();
        ui_c = FindObjectOfType<UIController>();
        ui_c.UpdateScoreCB(player_score);
        ui_c.SetLivesCount(player_lives);
        ui_c.ResetStateMessage();

        initial_ball_c = FindObjectOfType<BallController>();
        bat_c = FindObjectOfType<BatController>();
        bricks = FindObjectsOfType<BrickBase>();
        bricks_left = bricks.Length;

        SetBallToBat();

        SetBricksFreezed(true);
    }

    // coroutine to enable physics  // XXX suspend while not launched?
    IEnumerator UnfreezeBricksAfterTime(float time, float effect_start_time) {
        if (effect_start_time < 0)
            effect_start_time = 0;
        if (effect_start_time > time)
            effect_start_time = time;
        time -= effect_start_time;

        yield return new WaitForSeconds(effect_start_time);
        if (bricks_unfreeze_effect != null)
            bricks_unfreeze_effect.Play();

        yield return new WaitForSeconds(time);
        SetBricksFreezed(false);
        foreach (BrickBase b in bricks)
            if (!b.IsDestroyed()) {
                b.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitSphere * ggc.brick_base_mass * 10f);
            }
    }

    // Update is called once per frame
    void Update() {
        if (!launched && !level_lose && !level_win) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                if (!started) {
                    if (physics_enabled)
                        StartCoroutine(UnfreezeBricksAfterTime(physics_enable_delay, physics_enable_effect_delay));  // XXX params
                    started = true;
                }
                launched = true;
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
