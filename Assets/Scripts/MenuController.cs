using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuController : MonoBehaviour
{

    public void BtnStart() {
        if (!GameFlowController.Instance.StartLevelSet(0)) {
            Debug.LogError("Cannot start level set 0");
            Application.Quit(1);
        }
    }
    public void BtnStartExtra() {
        if (GameFlowController.Instance.StartLevelSet(1)) {
            Debug.LogError("Cannot start level set 1");
            Application.Quit(1);
        }
    }
    public void BtnExit() {
        Application.Quit();
    }

    // Start is called before the first frame update
    void Start() {
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update() {
        
    }
}
