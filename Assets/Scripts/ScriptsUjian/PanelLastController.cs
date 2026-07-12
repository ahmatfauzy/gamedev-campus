using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelLastController : MonoBehaviour
{
    [Header("Panel References")]
    [SerializeField] private GameObject successPanel;
    [SerializeField] private GameObject failedPanel;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI successText;

    private Button nextButton;
    private Button successRetryButton;
    private Button failRetryButton;
    private Transform starContainer;
    private Image[] starImages;
    private bool initialized;
    private Sprite starFilledSprite;
    private Sprite starEmptySprite;

    private void Awake()
    {
        starFilledSprite = GenerateStarSprite(new Color(1f, 0.85f, 0f));
        starEmptySprite = GenerateStarSprite(new Color(0.25f, 0.25f, 0.25f));
    }

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    public void Show(bool success, int stars)
    {
        if (success)
        {
            successPanel.SetActive(true);
            failedPanel.SetActive(false);
            SetupSuccess(stars);
        }
        else
        {
            successPanel.SetActive(false);
            failedPanel.SetActive(true);
            SetupFailed();
        }
    }

    private void SetupSuccess(int stars)
    {
        if (!initialized)
            InitSuccess();

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                bool earned = i < stars;
                starImages[i].sprite = earned ? starFilledSprite : starEmptySprite;
            }
        }

        if (successText != null)
            successText.text = "MISI SELESAI!";

        nextButton.gameObject.SetActive(true);
        successRetryButton.gameObject.SetActive(true);
    }

    private void InitSuccess()
    {
        initialized = true;

        starContainer = successPanel.transform.Find("Bintang");
        if (starContainer == null) return;

        nextButton = successPanel.transform.Find("PlayNext")?.GetComponent<Button>();
        successRetryButton = successPanel.transform.Find("PlayUlangi")?.GetComponent<Button>();

        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.onClick.AddListener(OnNext);
        }

        if (successRetryButton != null)
        {
            successRetryButton.onClick.RemoveAllListeners();
            successRetryButton.onClick.AddListener(OnRetry);
        }

        starImages = new Image[3];
        float starSize = 60f;
        float spacing = 20f;
        float totalW = starSize * 3 + spacing * 2;
        float startX = -totalW / 2f + starSize / 2f;

        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = new GameObject("Star" + (i + 1), typeof(RectTransform));
            starObj.AddComponent<CanvasRenderer>();
            starObj.AddComponent<Image>();

            RectTransform starRt = starObj.GetComponent<RectTransform>();
            starRt.SetParent(starContainer, false);

            starRt.anchorMin = new Vector2(0.5f, 0.5f);
            starRt.anchorMax = new Vector2(0.5f, 0.5f);
            starRt.sizeDelta = new Vector2(starSize, starSize);
            starRt.anchoredPosition = new Vector2(startX + i * (starSize + spacing), 0f);

            Image img = starObj.GetComponent<Image>();
            img.sprite = starEmptySprite;
            img.raycastTarget = false;
            starImages[i] = img;
        }
    }

    private void SetupFailed()
    {
        if (failRetryButton == null)
        {
            failRetryButton = failedPanel.transform.Find("PlayUlangi")?.GetComponent<Button>();
            if (failRetryButton != null)
            {
                failRetryButton.onClick.RemoveAllListeners();
                failRetryButton.onClick.AddListener(OnRetry);
            }
        }
    }

    private void OnNext()
    {
        Time.timeScale = 1f;
        string currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string nextLevel = LevelProgress.GetNextLevelName(currentScene);
        if (nextLevel != null)
            UnityEngine.SceneManagement.SceneManager.LoadScene(nextLevel);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene("Level");
    }

    private void OnRetry()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    private Vector2[] GetStarPoints(float cx, float cy, float outerR, float innerR)
    {
        Vector2[] pts = new Vector2[10];
        for (int i = 0; i < 5; i++)
        {
            float angle = -Mathf.PI * 0.5f + i * Mathf.PI * 0.4f;
            pts[i * 2] = new Vector2(cx + Mathf.Cos(angle) * outerR, cy + Mathf.Sin(angle) * outerR);
            float innerAngle = angle + Mathf.PI * 0.2f;
            pts[i * 2 + 1] = new Vector2(cx + Mathf.Cos(innerAngle) * innerR, cy + Mathf.Sin(innerAngle) * innerR);
        }
        return pts;
    }

    private bool PointInPolygon(Vector2 point, Vector2[] polygon)
    {
        bool inside = false;
        int j = polygon.Length - 1;
        for (int i = 0; i < polygon.Length; i++)
        {
            float xi = polygon[i].x, yi = polygon[i].y;
            float xj = polygon[j].x, yj = polygon[j].y;
            if ((yi > point.y) != (yj > point.y))
            {
                float xIntersect = xi + (point.y - yi) * (xj - xi) / (yj - yi);
                if (point.x < xIntersect)
                    inside = !inside;
            }
            j = i;
        }
        return inside;
    }

    private Sprite GenerateStarSprite(Color color)
    {
        int size = 64;
        Texture2D tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        Color clear = Color.clear;

        float cx = size * 0.5f;
        float cy = size * 0.5f;
        float outerR = size * 0.45f;
        float innerR = outerR * 0.4f;
        Vector2[] poly = GetStarPoints(cx, cy, outerR, innerR);

        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                bool inside = PointInPolygon(new Vector2(x + 0.5f, y + 0.5f), poly);
                tex.SetPixel(x, y, inside ? color : clear);
            }
        }

        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
    }
}
