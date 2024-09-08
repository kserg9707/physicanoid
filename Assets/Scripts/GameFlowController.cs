using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class LevelsSet {
    public int[] levels;
    public int this[int index] {
        get => levels[index];
    }
    public int Length { get { return levels.Length; } }
}

// singleton
public class GameFlowController : MonoBehaviour
{
    public static GameFlowController Instance { get; private set; }

    public bool cheats = false;
    bool is_menu = true;
    public int menu_scene_idx;
    public LevelsSet[] level_sets = new LevelsSet[0];
    int cur_level_set_idx = 0;
    int cur_level_idx = 0;

    AsyncOperation async_op;

    public int GetCurrentLevelBuildIdx() {
        return level_sets[cur_level_set_idx][cur_level_idx];
    }

    private Scene GetCurScene() {
        if (cur_level_set_idx < 0 || cur_level_set_idx >= level_sets.Length)
            return SceneManager.GetSceneByBuildIndex(menu_scene_idx);
        if (cur_level_idx < 0 || cur_level_idx >= level_sets[cur_level_set_idx].Length)
            return SceneManager.GetSceneByBuildIndex(menu_scene_idx);
        Scene res = SceneManager.GetSceneByBuildIndex(level_sets[cur_level_set_idx][cur_level_idx]);
        if (!res.IsValid())
            return SceneManager.GetSceneByBuildIndex(menu_scene_idx);
        return res;
    }

    void CheckBounds() {
        if (cur_level_set_idx >= level_sets.Length || cur_level_idx >= level_sets[cur_level_set_idx].Length)
            is_menu = true;
    }

    public bool LoadLevel() {
        CheckBounds();

        if (is_menu) {
            SceneManager.LoadScene(menu_scene_idx);
            return true;
        }

        SceneManager.LoadScene(GetCurrentLevelBuildIdx());
        return true;
    }

    public void LoadMenu() {
        is_menu = true;
        LoadLevel();
    }

    public void LoadMenuAsync() {
        is_menu = true;
        LoadLevelAsync();
    }

    public bool LoadNextLevel() {
        if (!is_menu)
            cur_level_idx++;

        LoadLevel();
        return true;
    }

    public bool LoadLevelAsync() {
        CheckBounds();

        if (is_menu) {
            async_op = SceneManager.LoadSceneAsync(menu_scene_idx);
            async_op.allowSceneActivation = false;
            return true;
        }

        async_op = SceneManager.LoadSceneAsync(GetCurrentLevelBuildIdx());
        async_op.allowSceneActivation = false;
        return true;
    }
    public bool LoadNextLevelAsync() {
        if (!is_menu)
            cur_level_idx++;
        LoadLevelAsync();
        return true;
    }

    public void WaitLoadLevelAsync() {
        if (async_op == null) {
            Debug.LogError("No async operation");
            return;
        }
        async_op.allowSceneActivation = true;
        async_op = null;
    }

    public bool StartLevelSet(int level_set_idx) {
        if (!SetLevelSet(level_set_idx))
            return false;
        is_menu = false;
        cur_level_idx = 0;
        LoadLevel();
        return true;
    }

    public bool SetLevelSet(int level_set_idx) {
        if (level_set_idx < 0 || level_set_idx >= level_sets.Length)
            return false;
        cur_level_set_idx = level_set_idx;
        return true;
    }

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
        } else {
            Instance = this;
        }
    }
    void Start () {
    	DontDestroyOnLoad(gameObject);

        is_menu = true;
        LoadNextLevel();
    }
}
