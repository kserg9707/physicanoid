using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField]
    TMP_Text w_text_score;
    [SerializeField]
    TMP_Text w_text_lives;
    [SerializeField]
    TMP_Text w_text_state;

    public void UpdateScoreCB(int score) {
        w_text_score.text = "Score: " + score.ToString();
    }

    public void SetLivesCount(int lives) {
        w_text_lives.text = "Lives: " + lives.ToString();
    }

    public void SetStateMessage(string message) {
        w_text_state.text = message;
    }

    public void ResetStateMessage() {
        SetStateMessage("");
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        
    }
}
