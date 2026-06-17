using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoad : MonoBehaviour
{

    public void LoadSceneBaru(string sceneName){
        SceneManager.LoadScene(sceneName);
    }

    public void OnApplicationQuit(){
        Debug.Log("aplikasi keluar");
        Application.Quit();
    }


    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
