using UnityEngine;

public class CarEngineAudioController : MonoBehaviour
{
    [Header("Engine Audio Clips")]
    public AudioClip startupClip;
    public AudioClip idleClip;
    public AudioClip lowOnClip;
    public AudioClip lowOffClip;
    public AudioClip medOnClip;
    public AudioClip medOffClip;
    public AudioClip highOnClip;
    public AudioClip highOffClip;
    public AudioClip maxRPMClip;

    [Header("RPM Tuning")]
    public float rpmSmoothSpeed = 3f;
    public float maxSpeedForRPM = 15f;

    [Header("Layer RPM Centers (0-1)")]
    public float idleCenter = 0f;
    public float lowCenter = 0.2f;
    public float medCenter = 0.5f;
    public float highCenter = 0.75f;
    public float maxCenter = 0.95f;

    [Header("Layer RPM Range")]
    public float layerRange = 0.25f;

    private AudioSource startupSource;
    private AudioSource idleSource;
    private AudioSource lowSource;
    private AudioSource medSource;
    private AudioSource highSource;
    private AudioSource maxSource;

    private CarController carController;
    private mobil mobilController;
    private Rigidbody carRigidbody;
    private float currentRPM = 0f;
    private bool startupPlayed = false;

    void Start()
    {
        carController = FindObjectOfType<CarController>();
        mobilController = FindObjectOfType<mobil>();
        if (carController == null && mobilController == null)
        {
            enabled = false;
            return;
        }
        Rigidbody rb = null;
        if (carController != null) rb = carController.GetComponent<Rigidbody>();
        if (rb == null && mobilController != null) rb = mobilController.GetComponent<Rigidbody>();
        carRigidbody = rb;

        startupSource = gameObject.AddComponent<AudioSource>();
        startupSource.playOnAwake = false;

        idleSource = CreateLoopSource(idleClip);
        lowSource = CreateLoopSource(lowOffClip);
        medSource = CreateLoopSource(medOffClip);
        highSource = CreateLoopSource(highOffClip);
        maxSource = CreateLoopSource(maxRPMClip);

        PlayAll();
    }

    private AudioSource CreateLoopSource(AudioClip clip)
    {
        AudioSource src = gameObject.AddComponent<AudioSource>();
        src.clip = clip;
        src.loop = true;
        src.playOnAwake = false;
        return src;
    }

    private void PlayAll()
    {
        if (idleSource != null && idleSource.clip != null) idleSource.Play();
        if (lowSource != null && lowSource.clip != null) lowSource.Play();
        if (medSource != null && medSource.clip != null) medSource.Play();
        if (highSource != null && highSource.clip != null) highSource.Play();
        if (maxSource != null && maxSource.clip != null) maxSource.Play();
    }

    void Update()
    {
        if (carController == null && mobilController == null) return;

        float moveValue = carController != null ? carController.moveInput : mobilController.verticalInput;
        float absInput = Mathf.Abs(moveValue);
        float speed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f;
        float normalizedSpeed = Mathf.Clamp01(speed / maxSpeedForRPM);

        float targetRPM = Mathf.Clamp01((absInput * 0.7f) + (normalizedSpeed * 0.3f));
        currentRPM = Mathf.Lerp(currentRPM, targetRPM, Time.deltaTime * rpmSmoothSpeed);

        bool isMoving = absInput > 0.05f || speed > 0.5f;
        if (!startupPlayed && isMoving && startupClip != null)
        {
            startupSource.PlayOneShot(startupClip);
            startupPlayed = true;
        }
        if (startupPlayed && !isMoving)
            startupPlayed = false;

        bool onThrottle = absInput > 0.05f;

        UpdateSourceClip(lowSource, onThrottle ? lowOnClip : lowOffClip);
        UpdateSourceClip(medSource, onThrottle ? medOnClip : medOffClip);
        UpdateSourceClip(highSource, onThrottle ? highOnClip : highOffClip);

        float sfxVolume = AudioManager.Instance != null ? AudioManager.Instance.SFXVolume : 0.5f;

        idleSource.volume = LayerWeight(currentRPM, idleCenter) * sfxVolume;
        lowSource.volume = LayerWeight(currentRPM, lowCenter) * sfxVolume;
        medSource.volume = LayerWeight(currentRPM, medCenter) * sfxVolume;
        highSource.volume = LayerWeight(currentRPM, highCenter) * sfxVolume;
        maxSource.volume = LayerWeight(currentRPM, maxCenter) * sfxVolume;
    }

    private void UpdateSourceClip(AudioSource src, AudioClip clip)
    {
        if (clip != null && src.clip != clip)
        {
            src.clip = clip;
            src.Play();
        }
    }

    private float LayerWeight(float rpm, float center)
    {
        float dist = Mathf.Abs(rpm - center);
        return Mathf.Clamp01(1f - (dist / layerRange));
    }
}
