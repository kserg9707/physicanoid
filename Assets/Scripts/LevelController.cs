using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class LevelController : MonoBehaviour {

    bool started = false;  // level started (first launch occured)
    bool launched = false;  // ball launched
    bool bricks_freezed = true;
    int player_score = 0;  // XXX not here?
    bool level_win = false;
    int player_lives = 3;  // initial player lives
    bool level_lose = false;

    // level config
    public float field_width = 18f;
    public float default_ball_speed = 15f;
    public bool physics_enabled = false;  // is physics enabled on level
    public float physics_enable_delay = 7f;  // seconds before physics enabling
    public float physics_enable_effect_len = 1f;  // also from level start
    public bool ball_force_enabled = false;
    public float ball_force_enable_delay = 15f;  // seconds before ball force enabling, AFTER physics!
    public float ball_force_enable_effect_len { get { return initial_ball_c.ForceEnableEffectLen; }}
    // public float physics_enable_effect_len = 1f;  // also from level start

    public ParticleSystem bricks_unfreeze_effect;
    public AudioSource sound_ball_fall;
    public AudioSource sound_win;
    public AudioSource sound_lose;

    public int fall_score_lost = 3;  // score lost at ball fall

    public bool PhysicsEnabled { get { return !bricks_freezed; } }
    public float LevelBallSpeed { get { return (physics_enabled) ? default_ball_speed * 0.7f : default_ball_speed; } }

    GlobalGameSettings ggc;
    UIController ui_c;

    BallController initial_ball_c;
    BatController bat_c;
    BrickBase[] bricks;

    int bricks_left = 0;

    public bool IsLaunched() {
        return launched;
    }

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
        ui_c.SetPhysicsTimer(physics_enabled, physics_enable_delay);

        initial_ball_c = FindObjectOfType<BallController>();
        bat_c = FindObjectOfType<BatController>();
        bricks = FindObjectsOfType<BrickBase>();
        bricks_left = bricks.Length;

        SetBallToBat();

        SetBricksFreezed(true);
    }

    // coroutine to enable physics  // suspend while not launched? no
    IEnumerator UnfreezeBricksAfterTime(float time, float effect_len) {
        effect_len = Mathf.Clamp(effect_len, 0f, time);
        // time -= effect_start_time;
        float effect_start_time = time - effect_len;

        float time_passed = 0f;
        while (time_passed < effect_start_time) {
            yield return new WaitForEndOfFrame();
            time_passed += Time.deltaTime;
            ui_c.SetPhysicsTimer(physics_enabled, Mathf.Clamp(physics_enable_delay - time_passed, 0f, physics_enable_delay));
        }

        //yield return new WaitForSeconds(effect_start_time);
        if (bricks_unfreeze_effect != null)
            bricks_unfreeze_effect.Play();

        while (time_passed < physics_enable_delay) {
            yield return new WaitForEndOfFrame();
            time_passed += Time.deltaTime;
            ui_c.SetPhysicsTimer(physics_enabled, Mathf.Clamp(physics_enable_delay - time_passed, 0f, physics_enable_delay));
        }

        // yield return new WaitForSeconds(time);
        bricks_freezed = false;
        SetBricksFreezed(false);
        foreach (BrickBase b in bricks)
            if (!b.IsDestroyed()) {
                b.GetComponent<Rigidbody2D>().AddForce(Random.insideUnitSphere * ggc.brick_base_mass * 10f);
            }

        if (ball_force_enabled) {
            StartCoroutine(BallForceAfterTime(ball_force_enable_delay, ball_force_enable_effect_len));
        }
    }

    // coroutine to enable ball force
    IEnumerator BallForceAfterTime(float time, float effect_len) {
        effect_len = Mathf.Clamp(effect_len, 0f, time);
        // time -= effect_start_time;
        float effect_start_time = time - effect_len;

        float time_passed = 0f;
        while (time_passed < effect_start_time) {
            yield return new WaitForEndOfFrame();
            time_passed += Time.deltaTime;
            ui_c.SetBallForceTimer(ball_force_enabled, Mathf.Clamp(ball_force_enable_delay - time_passed, 0f, ball_force_enable_delay));
        }

        //yield return new WaitForSeconds(effect_start_time);
        initial_ball_c.ForceEnableEffectPlay();

        while (time_passed < ball_force_enable_delay) {
            yield return new WaitForEndOfFrame();
            time_passed += Time.deltaTime;
            ui_c.SetBallForceTimer(ball_force_enabled, Mathf.Clamp(ball_force_enable_delay - time_passed, 0f, ball_force_enable_delay));
        }

        // yield return new WaitForSeconds(time);
        initial_ball_c.ForceEnable();
    }

    public void Launch() {
        if (!started) {
            if (physics_enabled)
                StartCoroutine(UnfreezeBricksAfterTime(physics_enable_delay, physics_enable_effect_len));  // XXX params
            started = true;
        }
        launched = true;
        initial_ball_c.transform.SetParent(null);
        initial_ball_c.Unfreeze();
    }

    // Update is called once per frame
    void Update() {
        if (!launched && !level_lose && !level_win) {
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0)) {
                Launch();
            }
        }

        if (Input.GetKeyDown(KeyCode.Backspace)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            ExitLevel();
    }
}
