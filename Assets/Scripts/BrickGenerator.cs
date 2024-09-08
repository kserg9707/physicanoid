using System.Collections;
using System.Collections.Generic;
using System.Threading;
#if UNITY_EDITOR
using UnityEditorInternal;
#endif
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class BrickGenerator : MonoBehaviour {
#if UNITY_EDITOR
    public GameObject prefab;

    public Vector2Int size = new Vector2Int(11, 9);
    public Vector2 brick_offset = new Vector2(1.75f, 0.75f);
    public bool clear_on_generate = false;
    public bool mark_to_clear = false;
    public bool mark_to_generate = false;
    public List<Color> lines_colors = new List<Color>();  // from top
    public bool repeat_colors = false;

    List<GameObject> GetAllBricks() {
        List<GameObject> res = new List<GameObject>();
        for (int i = 0; i < transform.childCount; ++i) {
            GameObject go = transform.GetChild(i).gameObject;
            if (go.CompareTag("Brick"))
                res.Add(go);
        }
        return res;
    }

    void ClearBricks() {
        foreach (GameObject go in GetAllBricks()) {
            DestroyImmediate(go);
        }
    }

    void GenerateBricks() {
        if (clear_on_generate)
            ClearBricks();

        Vector2 left_top = new Vector2(transform.position.x - brick_offset.x * (size.x / 2) + (1 - (size.x % 2)) * brick_offset.x * 0.5f, transform.position.y);
        for (int y = 0; y < size.y; ++y) {
            for (int x = 0; x < size.x; ++x) {
                GameObject go = Instantiate(prefab, left_top + brick_offset * new Vector2(x, -y), Quaternion.identity, transform);
                if (lines_colors.Count > 0 && (y < lines_colors.Count || repeat_colors)) {
                    BrickBase br = go.GetComponent<BrickBase>();
                    br.color = lines_colors[y % lines_colors.Count];
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start() {
        if (Application.isPlaying)
            enabled = false;
    }

    // Update is called once per frame
    void Update() {
        if (mark_to_generate) {
            mark_to_generate = false;
            GenerateBricks();
        }
        if (mark_to_clear) {
            mark_to_clear = false;
            ClearBricks();
        }
    }
#endif
}
