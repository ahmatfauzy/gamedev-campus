using UnityEngine;
using UnityEngine.UI;

public class MissionUIController : MonoBehaviour
{
    [Header("Timer Display (during gameplay)")]
    [SerializeField] private Text timerText;

    [Header("Mission Complete Panel")]
    [SerializeField] private GameObject missionCompletePanel;
    [SerializeField] private Image[] completeStarImages;
    [SerializeField] private Text completeTimeText;

    [Header("Mission Failed Panel")]
    [SerializeField] private GameObject missionFailedPanel;

    [Header("Star Sprites")]
    [SerializeField] private Sprite starFilledSprite;
    [SerializeField] private Sprite starEmptySprite;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private bool wasComplete;
    private bool wasFailed;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(false);

        if (missionFailedPanel != null)
            missionFailedPanel.SetActive(false);
    }

    private void Update()
    {
        if (gameManager == null) return;

        if (!wasComplete && gameManager.IsMissionComplete())
        {
            wasComplete = true;
            ShowMissionComplete();
        }

        if (!wasFailed && gameManager.IsMissionFailed())
        {
            wasFailed = true;
            ShowMissionFailed();
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        if (timerText == null) return;
        if (gameManager.IsMissionComplete() || gameManager.IsMissionFailed()) return;

        float time = gameManager.GetCurrentTime();
        int min = Mathf.FloorToInt(time / 60f);
        int sec = Mathf.FloorToInt(time % 60f);

        timerText.text = string.Format("{0:00}:{1:00}", min, sec);
        timerText.color = time <= 10f ? Color.red : Color.white;
    }

    private void ShowMissionComplete()
    {
        if (missionCompletePanel != null)
            missionCompletePanel.SetActive(true);

        int stars = gameManager.GetStarsEarned();
        for (int i = 0; i < completeStarImages.Length; i++)
        {
            if (completeStarImages[i] != null)
                completeStarImages[i].sprite = i < stars ? starFilledSprite : starEmptySprite;
        }

        if (completeTimeText != null)
        {
            float elapsed = gameManager.GetTimeLimit() - gameManager.GetCurrentTime();
            int min = Mathf.FloorToInt(elapsed / 60f);
            int sec = Mathf.FloorToInt(elapsed % 60f);
            completeTimeText.text = "Waktu: " + string.Format("{0:00}:{1:00}", min, sec);
        }
    }

    private void ShowMissionFailed()
    {
        if (missionFailedPanel != null)
            missionFailedPanel.SetActive(true);
    }
}
