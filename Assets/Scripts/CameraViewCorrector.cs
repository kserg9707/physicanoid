using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// should be attached to camera
public class CameraViewCorrector : MonoBehaviour
{
    // Start is called before the first frame update
    void Start() {
        UpdateView();
        Display.onDisplaysUpdated += UpdateView;
    }

    // game field always fits vertically so need to correct if ratio < 16/9 (ex. 5/4)
    void UpdateView() {
        Camera cam = GetComponent<Camera>();
        float default_ratio = 16f / 9f;
        float ratio = Display.main.renderingWidth / Display.main.renderingHeight;
        if (ratio >= default_ratio)
            return;
        float fov = cam.fieldOfView;
        Ray r = cam.ScreenPointToRay(Vector3.zero);
        RaycastHit hit;
        // Debug.Log("ray: " + r.origin.ToString() + ", " + r.direction.ToString());
        if (!Physics.Raycast(r, out hit, cam.farClipPlane *10f, ~LayerMask.NameToLayer("screen_test"))) {
            Debug.LogError("Failed to adjust camera: no raycast hit");
            return;
        }
        // Debug.Log(hit.point);
        Vector3 delta = hit.point - cam.transform.position;
        float target_x = delta.y * 16 / 9;
        // float target_angle = Mathf.Atan2(target_x, delta.z);
        float current_tg = delta.x / delta.z;
        float target_distance = target_x / current_tg;
        // Debug.Log(target_distance);

        cam.transform.position = new Vector3(cam.transform.position.x, cam.transform.position.y, hit.point.z + -target_distance * Mathf.Sign(delta.z));
    }

    // Update is called once per frame
    void Update() {
        
    }
}
