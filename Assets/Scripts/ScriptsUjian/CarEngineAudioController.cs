using UnityEngine;

public class CarEngineAudioController : MonoBehaviour
{
    [Header("Engine Audio")]
    public AudioClip carNgengClip;

    private AudioSource engineSource;

    private CarController carController;
    private mobil mobilController;
    private Rigidbody carRigidbody;

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

        engineSource = gameObject.AddComponent<AudioSource>();
        engineSource.clip = carNgengClip;
        engineSource.loop = true;
        engineSource.playOnAwake = false;
    }

    void Update()
    {
        if (carController == null && mobilController == null) return;

        float moveValue = carController != null ? carController.moveInput : mobilController.verticalInput;
        float absInput = Mathf.Abs(moveValue);
        float speed = carRigidbody != null ? carRigidbody.linearVelocity.magnitude : 0f;

        bool isMoving = absInput > 0.05f || speed > 0.5f;

        float sfxVolume = AudioManager.Instance != null ? AudioManager.Instance.SFXVolume : 0.5f;
        engineSource.volume = sfxVolume;

        if (isMoving && !engineSource.isPlaying && engineSource.clip != null)
            engineSource.Play();
        else if (!isMoving && engineSource.isPlaying)
            engineSource.Pause();
    }
}
