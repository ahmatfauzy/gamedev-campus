using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoad : MonoBehaviour
{
    private void Start()
    {
        if (gameObject.scene.name == "MainMenu")
        {
            gameObject.AddComponent<SettingsPanelController>();
        }
        else if (gameObject.scene.name == "Level")
        {
            SetupLevelButtons();
        }
    }

    private void SetupLevelButtons()
    {
        GameObject levelContainer = GameObject.Find("level");
        if (levelContainer == null) return;

        foreach (Transform child in levelContainer.transform)
        {
            Button btn = child.GetComponent<Button>();
            if (btn == null) continue;

            string levelName = child.gameObject.name;
            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => SceneManager.LoadScene(levelName));

            bool alwaysUnlocked = levelName == "Level1";
            btn.interactable = alwaysUnlocked || LevelProgress.IsLevelUnlocked(levelName);
        }
    }

    public void LoadSceneBaru(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

    public void OnApplicationQuit(){
        Debug.Log("aplikasi keluar");
        Application.Quit();
    }
}
