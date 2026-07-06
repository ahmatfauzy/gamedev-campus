using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Clips")]
    [SerializeField] private AudioClip bgmClip;

    private AudioSource bgmSource;
    private AudioSource sfxSource;

    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;

    private const string BGM_VOL_KEY = "BGMVolume";
    private const string SFX_VOL_KEY = "SFXVolume";

    private CarEngineAudioController engineController;

    private AudioClip engineStartupClip;
    private AudioClip engineIdleClip;
    private AudioClip engineLowOnClip;
    private AudioClip engineLowOffClip;
    private AudioClip engineMedOnClip;
    private AudioClip engineMedOffClip;
    private AudioClip engineHighOnClip;
    private AudioClip engineHighOffClip;
    private AudioClip engineMaxRPMClip;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoInitialize()
    {
        if (Instance == null)
        {
            GameObject go = new GameObject("AudioManager");
            go.AddComponent<AudioManager>();
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource = gameObject.AddComponent<AudioSource>();
        bgmSource.loop = true;
        bgmSource.playOnAwake = false;

        sfxSource = gameObject.AddComponent<AudioSource>();
        sfxSource.playOnAwake = false;

        LoadSettings();

        if (bgmClip == null)
            bgmClip = Resources.Load<AudioClip>("Audio/soundmainmenu");

        if (bgmClip != null)
        {
            bgmSource.clip = bgmClip;
            bgmSource.volume = bgmVolume;
        }

        LoadEngineClips();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void LoadEngineClips()
    {
        string dir = "Assets/AssetsUjian/Car Engine Sound - i6 German Free/Assets/Audio/i6_german_free/";
        string res = "Audio/Engine/";
        engineStartupClip = LoadClip(dir + "startup.wav", res + "startup");
        engineIdleClip = LoadClip(dir + "idle.wav", res + "idle");
        engineLowOnClip = LoadClip(dir + "low_on.wav", res + "low_on");
        engineLowOffClip = LoadClip(dir + "low_off.wav", res + "low_off");
        engineMedOnClip = LoadClip(dir + "med_on.wav", res + "med_on");
        engineMedOffClip = LoadClip(dir + "med_off.wav", res + "med_off");
        engineHighOnClip = LoadClip(dir + "high_on.wav", res + "high_on");
        engineHighOffClip = LoadClip(dir + "high_off.wav", res + "high_off");
        engineMaxRPMClip = LoadClip(dir + "maxRPM.wav", res + "maxRPM");
    }

    private AudioClip LoadClip(string editorPath, string resourcesPath)
    {
#if UNITY_EDITOR
        AudioClip clip = UnityEditor.AssetDatabase.LoadAssetAtPath<AudioClip>(editorPath);
        if (clip != null) return clip;
#endif
        return Resources.Load<AudioClip>(resourcesPath);
    }

    private void Start()
    {
        HandleSceneAudio(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleSceneAudio(scene.name);
    }

    private void HandleSceneAudio(string sceneName)
    {
        Debug.Log($"[AudioManager] Scene loaded: {sceneName}");

        if (sceneName == "MainMenu" || sceneName == "Level")
        {
            StopEngineAudio();

            if (bgmSource != null && bgmClip != null && !bgmSource.isPlaying)
            {
                bgmSource.volume = bgmVolume;
                bgmSource.Play();
                Debug.Log("[AudioManager] BGM started");
            }
        }
        else
        {
            if (bgmSource != null && bgmSource.isPlaying)
                bgmSource.Stop();

            SetupEngineAudio();
        }
    }

    private void SetupEngineAudio()
    {
        if (engineController != null) return;

        if (engineIdleClip == null)
        {
            Debug.LogError("[AudioManager] Engine clips not loaded");
            return;
        }

        GameObject engineGO = new GameObject("CarEngineAudio");
        engineGO.transform.SetParent(transform);

        engineController = engineGO.AddComponent<CarEngineAudioController>();
        engineController.startupClip = engineStartupClip;
        engineController.idleClip = engineIdleClip;
        engineController.lowOnClip = engineLowOnClip;
        engineController.lowOffClip = engineLowOffClip;
        engineController.medOnClip = engineMedOnClip;
        engineController.medOffClip = engineMedOffClip;
        engineController.highOnClip = engineHighOnClip;
        engineController.highOffClip = engineHighOffClip;
        engineController.maxRPMClip = engineMaxRPMClip;
    }

    private void StopEngineAudio()
    {
        if (engineController != null)
        {
            Destroy(engineController.gameObject);
            engineController = null;
        }
    }

    private void LoadSettings()
    {
        bgmVolume = PlayerPrefs.GetFloat(BGM_VOL_KEY, 0.5f);
        sfxVolume = PlayerPrefs.GetFloat(SFX_VOL_KEY, 0.5f);
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(BGM_VOL_KEY, bgmVolume);
        PlayerPrefs.SetFloat(SFX_VOL_KEY, sfxVolume);
        PlayerPrefs.Save();
    }

    public float BGMVolume
    {
        get => bgmVolume;
        set
        {
            bgmVolume = Mathf.Clamp01(value);
            if (bgmSource != null)
                bgmSource.volume = bgmVolume;
            SaveSettings();
        }
    }

    public float SFXVolume
    {
        get => sfxVolume;
        set
        {
            sfxVolume = Mathf.Clamp01(value);
            if (sfxSource != null)
                sfxSource.volume = sfxVolume;
            SaveSettings();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
        {
            sfxSource.volume = sfxVolume;
            sfxSource.PlayOneShot(clip);
        }
    }
}
