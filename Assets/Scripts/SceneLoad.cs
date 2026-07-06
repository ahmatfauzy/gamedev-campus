using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{
    private void Start()
    {
        if (gameObject.scene.name == "MainMenu")
        {
            gameObject.AddComponent<SettingsPanelController>();
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
