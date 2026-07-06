using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PanelLastController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TextMeshProUGUI messageText;

    private Button nextButton;
    private Button retryButton;
    private Image[] starImages;
    private bool initialized;
    private Sprite starFilledSprite;
    private Sprite starEmptySprite;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();

        if (messageText == null)
            messageText = GetComponentInChildren<TextMeshProUGUI>();

        starFilledSprite = GenerateStarSprite(new Color(1f, 0.85f, 0f));
        starEmptySprite = GenerateStarSprite(new Color(0.25f, 0.25f, 0.25f));
    }

    private void EnsureUI()
    {
        if (initialized) return;
        initialized = true;

        RectTransform panelRect = GetComponent<RectTransform>();

        starImages = new Image[3];
        for (int i = 0; i < 3; i++)
        {
            GameObject starObj = new GameObject("Star" + (i + 1), typeof(RectTransform));
            starObj.AddComponent<CanvasRenderer>();
            starObj.AddComponent<Image>();

            RectTransform starRt = starObj.GetComponent<RectTransform>();
            starRt.SetParent(panelRect, false);

            float starSize = 50f;
            float spacing = 15f;
            float totalW = starSize * 3 + spacing * 2;
            float startX = -totalW / 2f + starSize / 2f;

            starRt.anchorMin = new Vector2(0.5f, 0.5f);
            starRt.anchorMax = new Vector2(0.5f, 0.5f);
            starRt.sizeDelta = new Vector2(starSize, starSize);
            starRt.anchoredPosition = new Vector2(startX + i * (starSize + spacing), 50f);

            Image img = starObj.GetComponent<Image>();
            img.sprite = starEmptySprite;
            img.raycastTarget = false;
            starImages[i] = img;
        }

        float btnY = -60f;
        nextButton = CreateButton(panelRect, "NextButton", "LEVEL NEXT", new Vector2(65, btnY), OnNext);
        retryButton = CreateButton(panelRect, "RetryButton", "ULANGI", new Vector2(-65, btnY), OnRetry);
    }

    private Button CreateButton(RectTransform parent, string name, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btnObj = new GameObject(name, typeof(RectTransform));
        btnObj.AddComponent<CanvasRenderer>();
        btnObj.AddComponent<Image>();
        btnObj.AddComponent<Button>();

        RectTransform btnRt = btnObj.GetComponent<RectTransform>();
        btnRt.SetParent(parent, false);
        btnRt.anchorMin = new Vector2(0.5f, 0.5f);
        btnRt.anchorMax = new Vector2(0.5f, 0.5f);
        btnRt.sizeDelta = new Vector2(200, 45);
        btnRt.anchoredPosition = pos;

        Image bg = btnObj.GetComponent<Image>();
        bg.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);

        GameObject txtObj = new GameObject("Text", typeof(RectTransform));
        txtObj.AddComponent<CanvasRenderer>();
        txtObj.AddComponent<TextMeshProUGUI>();

        RectTransform txtRt = txtObj.GetComponent<RectTransform>();
        txtRt.SetParent(btnRt, false);
        txtRt.anchorMin = Vector2.zero;
        txtRt.anchorMax = Vector2.one;
        txtRt.sizeDelta = Vector2.zero;
        txtRt.anchoredPosition = Vector2.zero;

        TextMeshProUGUI tmp = txtObj.GetComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 22;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;

        Button btn = btnObj.GetComponent<Button>();
        btn.targetGraphic = bg;
        btn.onClick.AddListener(onClick);
        return btn;
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

    public void Show(bool success, int stars)
    {
        gameObject.SetActive(true);
        EnsureUI();

        if (messageText != null)
            messageText.text = success ? "MISI SELESAI!" : "MISI GAGAL!";

        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] != null)
            {
                if (success)
                {
                    bool earned = i < stars;
                    starImages[i].sprite = earned ? starFilledSprite : starEmptySprite;
                }
                else
                {
                    starImages[i].sprite = starEmptySprite;
                }
            }
        }

        if (nextButton != null)
            nextButton.gameObject.SetActive(success);

        if (retryButton != null)
            retryButton.gameObject.SetActive(true);
    }

    private void OnNext()
    {
        Time.timeScale = 1f;
        int next = SceneManager.GetActiveScene().buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings)
            SceneManager.LoadScene(next);
        else
            SceneManager.LoadScene("LevelSelect");
    }

    private void OnRetry()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
