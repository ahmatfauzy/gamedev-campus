using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    [Header("Back Button")]
    [SerializeField] private Button backButton;

    [Header("Scene Name untuk tombol Back")]
    [SerializeField] private string mainMenuScene = "MainMenu";

    [Header("Level Buttons & Scene Names")]
    [SerializeField] private Button[] levelButtons;
    [SerializeField] private string[] levelSceneNames;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            int index = i;
            if (levelButtons[i] != null)
                levelButtons[i].onClick.AddListener(() => OnLevelClicked(index));
        }
    }

    private void OnBackClicked()
    {
        SceneManager.LoadScene(mainMenuScene);
    }

    private void OnLevelClicked(int index)
    {
        if (index >= 0 && index < levelSceneNames.Length)
            SceneManager.LoadScene(levelSceneNames[index]);
    }
}
