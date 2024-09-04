using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBase : MonoBehaviour {

    LevelController lc;

    public int hits_left = 1;
    public int score = 1;

    // Start is called before the first frame update
    void Start() {
        lc = FindObjectOfType<LevelController>();

        if (Random.Range(1,5) != 1) {
            hits_left = 2;
            score = 2;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ball"))
            if ((--hits_left) == 0) {
                gameObject.SetActive(false); //Destroy(gameObject);
                lc.BrickDestroyCallback(score);
            }
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Ball"))
            if ((--hits_left) == 0) {
                gameObject.SetActive(false); //Destroy(gameObject);
                lc.BrickDestroyCallback(score);
            }
    }
}
