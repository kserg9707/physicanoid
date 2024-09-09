using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    TMP_Text w_text_level;
    [SerializeField]
    TMP_Text w_text_score;
    [SerializeField]
    TMP_Text w_text_lives;
    [SerializeField]
    Image w_bg_state;
    [SerializeField]
    TMP_Text w_text_state;
    [SerializeField]
    TMP_Text w_text_physics_timer;
    [SerializeField]
    TMP_Text w_text_cheats;

    public void UpdateScoreCB(int score) {
        w_text_score.text = "Score: " + score.ToString();
    }

    public void SetLivesCount(int lives) {
        w_text_lives.text = "Lives: " + lives.ToString();
    }

    public void SetPhysicsTimer(bool show, float seconds_left) {
        if (!show)
            w_text_physics_timer.text = "";
        else
            w_text_physics_timer.text = seconds_left > 0 ? "Phase 2 starts in:\n" + seconds_left.ToString("F2").Replace(',', '.') : "Phase 2";
    }

    public void SetBallForceTimer(bool show, float seconds_left) {
        if (!show)
            w_text_physics_timer.text = "";
        else
            w_text_physics_timer.text = seconds_left > 0 ? "Extra phase starts in:\n" + seconds_left.ToString("F2").Replace(',', '.') : "Extra phase";
    }

    public void SetStateMessage(string message) {
        w_bg_state.gameObject.SetActive(true);
        w_text_state.text = message;
    }

    public void ResetStateMessage() {
        SetStateMessage("");
        w_bg_state.gameObject.SetActive(false);
    }
    
    public void SetCheatsVisible(bool visible) {
        w_text_cheats.gameObject.SetActive(visible);
    }

    // Start is called before the first frame update
    void Start() {
        w_text_level.text = "Level no: " +
                (GameFlowController.Instance.GetCurrentLevelIdx() + 1).ToString() +
                " of " +
                GameFlowController.Instance.GetLevelsCount().ToString();
    }

    // Update is called once per frame
    void Update() {
        
    }
}
