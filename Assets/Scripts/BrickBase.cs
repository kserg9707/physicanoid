using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBase : MonoBehaviour {

    int hits_left = 1;
    // Start is called before the first frame update
    void Start() {
        if (Random.Range(1,5) != 1)
            hits_left = 2;
    }

    // Update is called once per frame
    void Update() {
        
    }

    void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Ball"))
            if ((--hits_left) == 0)
                Destroy(gameObject);
    }
    void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.CompareTag("Ball"))
            if ((--hits_left) == 0)
                Destroy(gameObject);
    }
}
