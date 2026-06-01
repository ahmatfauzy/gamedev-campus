using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Timer Settings")]
    [SerializeField] private float timeLimit = 120f;

    [Header("Star Thresholds (% time remaining)")]
    [SerializeField] [Range(0f, 1f)] private float threeStarThreshold = 0.6f;
    [SerializeField] [Range(0f, 1f)] private float twoStarThreshold = 0.3f;

    [Header("Canvas Panels")]
    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private GameObject missionFailedPanel;

    [Header("Return to Menu")]
    [SerializeField] private string mainMenuScene = "MainMenu";
    [SerializeField] private string levelSelectScene = "LevelSelect";

    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip missionCompleteSound;
    [SerializeField] private AudioClip missionFailedSound;

    private AudioSource audioSource;
    private bool isMissionComplete;
    private bool isMissionFailed;
    private float currentTime;
    private int starsEarned;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void Start()
    {
        currentTime = timeLimit;
    }

    private void Update()
    {
        if (isMissionComplete || isMissionFailed) return;

        currentTime -= Time.deltaTime;

        if (currentTime <= 0f)
        {
            currentTime = 0f;
            OnMissionFailed();
        }
    }

    public void OnMissionComplete()
    {
        if (isMissionComplete || isMissionFailed) return;

        isMissionComplete = true;
        starsEarned = CalculateStars();

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(true);

        if (missionCompleteSound != null && audioSource != null)
            audioSource.PlayOneShot(missionCompleteSound);

        Time.timeScale = 0f;
    }

    public void OnMissionFailed()
    {
        if (isMissionFailed || isMissionComplete) return;

        isMissionFailed = true;

        if (missionFailedPanel != null)
            missionFailedPanel.SetActive(true);

        if (missionFailedSound != null && audioSource != null)
            audioSource.PlayOneShot(missionFailedSound);

        Time.timeScale = 0f;
    }

    private int CalculateStars()
    {
        float ratio = currentTime / timeLimit;
        if (ratio >= threeStarThreshold) return 3;
        if (ratio >= twoStarThreshold) return 2;
        return 1;
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuScene);
    }

    public void Retry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(levelSelectScene);
    }

    public bool IsMissionComplete() => isMissionComplete;
    public bool IsMissionFailed() => isMissionFailed;
    public float GetCurrentTime() => currentTime;
    public float GetTimeLimit() => timeLimit;
    public int GetStarsEarned() => starsEarned;

    private void OnGUI()
    {
        if (!isMissionComplete && !isMissionFailed)
            DrawTimer();

        if (isMissionComplete)
            DrawMissionComplete();

        if (isMissionFailed)
            DrawMissionFailed();
    }

    private void DrawTimer()
    {
        int min = Mathf.FloorToInt(currentTime / 60f);
        int sec = Mathf.FloorToInt(currentTime % 60f);
        string text = string.Format("{0:00}:{1:00}", min, sec);

        float boxW = 200;
        float boxH = 55;
        float x = Screen.width - boxW - 30;
        float y = 30;

        GUI.Box(new Rect(x, y, boxW, boxH), "");

        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 38,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };
        style.normal.textColor = currentTime <= 10f ? Color.red : Color.white;

        GUI.Label(new Rect(x, y, boxW, boxH), text, style);
    }

    private void DrawMissionComplete()
    {
        float boxW = 520;
        float boxH = 310;
        float x = (Screen.width - boxW) / 2f;
        float y = (Screen.height - boxH) / 2f - 50;

        GUI.Box(new Rect(x, y, boxW, boxH), "");

        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 44,
            alignment = TextAnchor.UpperCenter,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.green;
        GUI.Label(new Rect(x, y + 15, boxW, 50), "MISI SELESAI!", titleStyle);

        DrawStars(x, y + 75, boxW);

        int min = Mathf.FloorToInt((timeLimit - currentTime) / 60f);
        int sec = Mathf.FloorToInt((timeLimit - currentTime) % 60f);
        var timeStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 22,
            alignment = TextAnchor.MiddleCenter
        };
        timeStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y + 150, boxW, 50),
            "Waktu: " + string.Format("{0:00}:{1:00}", min, sec), timeStyle);

        float btnY = y + 215;
        float btnW = 200;
        float btnH = 45;
        float gap = 20;
        float totalBtnW = btnW * 2 + gap;
        float btnStartX = x + (boxW - totalBtnW) / 2f;

        if (GUI.Button(new Rect(btnStartX, btnY, btnW, btnH), "ULANGI"))
            Retry();

        if (GUI.Button(new Rect(btnStartX + btnW + gap, btnY, btnW, btnH), "KEMBALI KE MENU"))
            ReturnToMenu();
    }

    private void DrawStars(float panelX, float panelY, float panelW)
    {
        var starStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 52,
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        float starSize = 65;
        float spacing = 20;
        float totalW = starSize * 3 + spacing * 2;
        float startX = panelX + (panelW - totalW) / 2f;

        for (int i = 0; i < 3; i++)
        {
            Rect r = new Rect(startX + i * (starSize + spacing), panelY, starSize, starSize);
            bool earned = i < starsEarned;
            starStyle.normal.textColor = earned
                ? new Color(1f, 0.85f, 0f)
                : new Color(0.25f, 0.25f, 0.25f);
            GUI.Label(r, earned ? "\u2605" : "\u2606", starStyle);
        }
    }

    private void DrawMissionFailed()
    {
        float boxW = 520;
        float boxH = 270;
        float x = (Screen.width - boxW) / 2f;
        float y = (Screen.height - boxH) / 2f;

        GUI.Box(new Rect(x, y, boxW, boxH), "");

        var titleStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 44,
            alignment = TextAnchor.UpperCenter,
            fontStyle = FontStyle.Bold
        };
        titleStyle.normal.textColor = Color.red;
        GUI.Label(new Rect(x, y + 15, boxW, 50), "MISI GAGAL!", titleStyle);

        var subStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 24,
            alignment = TextAnchor.MiddleCenter
        };
        subStyle.normal.textColor = Color.white;
        GUI.Label(new Rect(x, y + 70, boxW, 50), "Waktu Habis!", subStyle);

        float btnY = y + 145;
        float btnW = 200;
        float btnH = 45;
        float gap = 20;
        float totalBtnW = btnW * 2 + gap;
        float btnStartX = x + (boxW - totalBtnW) / 2f;

        if (GUI.Button(new Rect(btnStartX, btnY, btnW, btnH), "ULANGI"))
            Retry();

        if (GUI.Button(new Rect(btnStartX + btnW + gap, btnY, btnW, btnH), "KEMBALI KE MENU"))
            ReturnToMenu();
    }
}
