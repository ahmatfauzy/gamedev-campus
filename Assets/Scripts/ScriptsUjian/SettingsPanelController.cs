using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsPanelController : MonoBehaviour
{
    private GameObject panelSetting;
    private Slider bgmSlider;
    private Slider sfxSlider;
    private TextMeshProUGUI bgmLabel;
    private TextMeshProUGUI sfxLabel;

    private void Start()
    {
        GameObject[] all = GameObject.FindObjectsOfType<GameObject>(true);
        foreach (GameObject go in all)
        {
            if (go.name == "PanelSetting" && go.transform.Find("BtnClose") != null)
            {
                panelSetting = go;
                break;
            }
        }

        if (panelSetting == null)
        {
            foreach (GameObject go in all)
            {
                if (go.name == "PanelSetting")
                {
                    panelSetting = go;
                    break;
                }
            }
        }

        if (panelSetting != null && AudioManager.Instance != null)
        {
            SetupVolumeControls();
        }
    }

    private void SetupVolumeControls()
    {
        if (AudioManager.Instance == null) return;

        Transform contentArea = panelSetting.transform.Find("Image");
        if (contentArea == null) return;

        float startY = -20;
        float spacing = 100;

        float initBgm = AudioManager.Instance.BGMVolume;
        float initSfx = AudioManager.Instance.SFXVolume;

        bgmLabel = CreateLabel(contentArea, "MusicLabel", "Music Volume: " + Mathf.RoundToInt(initBgm * 100) + "%", new Vector2(0, startY));
        bgmSlider = CreateSlider(contentArea, "BGMSlider", initBgm, new Vector2(0, startY - 45), val =>
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.BGMVolume = val;
            if (bgmLabel != null)
                bgmLabel.text = "Music Volume: " + Mathf.RoundToInt(val * 100) + "%";
        });

        sfxLabel = CreateLabel(contentArea, "SFXLabel", "SFX Volume: " + Mathf.RoundToInt(initSfx * 100) + "%", new Vector2(0, startY - spacing));
        sfxSlider = CreateSlider(contentArea, "SFXSlider", initSfx, new Vector2(0, startY - spacing - 45), val =>
        {
            if (AudioManager.Instance != null)
                AudioManager.Instance.SFXVolume = val;
            if (sfxLabel != null)
                sfxLabel.text = "SFX Volume: " + Mathf.RoundToInt(val * 100) + "%";
        });
    }

    private TextMeshProUGUI CreateLabel(Transform parent, string name, string text, Vector2 anchoredPos)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(400, 40);
        rt.anchoredPosition = anchoredPos;

        TextMeshProUGUI tmp = go.GetComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        tmp.fontStyle = FontStyles.Bold;
        return tmp;
    }

    private Slider CreateSlider(Transform parent, string name, float initialValue, Vector2 anchoredPos, System.Action<float> onValueChanged)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Slider));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(400, 30);
        rt.anchoredPosition = anchoredPos;

        GameObject bg = new GameObject("Background", typeof(RectTransform), typeof(Image));
        bg.transform.SetParent(go.transform, false);
        RectTransform bgRt = bg.GetComponent<RectTransform>();
        bgRt.anchorMin = Vector2.zero;
        bgRt.anchorMax = Vector2.one;
        bgRt.sizeDelta = Vector2.zero;
        Image bgImage = bg.GetComponent<Image>();
        bgImage.color = new Color(0.2f, 0.2f, 0.2f, 0.8f);

        GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
        fillArea.transform.SetParent(go.transform, false);
        RectTransform fillRt = fillArea.GetComponent<RectTransform>();
        fillRt.anchorMin = new Vector2(0, 0.15f);
        fillRt.anchorMax = new Vector2(1, 0.85f);
        fillRt.sizeDelta = new Vector2(-10, 0);
        fillRt.anchoredPosition = Vector2.zero;

        GameObject fill = new GameObject("Fill", typeof(RectTransform), typeof(Image));
        fill.transform.SetParent(fillArea.transform, false);
        RectTransform fillImageRt = fill.GetComponent<RectTransform>();
        fillImageRt.anchorMin = Vector2.zero;
        fillImageRt.anchorMax = Vector2.one;
        fillImageRt.sizeDelta = Vector2.zero;
        Image fillImage = fill.GetComponent<Image>();
        fillImage.color = Color.white;
        fillImage.type = Image.Type.Sliced;

        Slider slider = go.GetComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = initialValue;
        slider.wholeNumbers = false;
        slider.fillRect = fillImageRt;
        slider.direction = Slider.Direction.LeftToRight;

        slider.onValueChanged.AddListener(new UnityEngine.Events.UnityAction<float>(onValueChanged));
        return slider;
    }

}
