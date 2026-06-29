using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CarEngineAudioController : MonoBehaviour
{
    [Header("Titik Looping Mesin")]
    [Tooltip("Detik ke berapa audio kembali mengulang (misal: detik ke-6)")]
    public float waktuUlang = 6.0f; 
    
    [Tooltip("Detik ke berapa audio di-reset (misal: detik ke-15)")]
    public float waktuBatas = 15.0f;

    private AudioSource audioSource;
    private bool isStopping = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.loop = false; // Matikan loop bawaan karena kita pakai loop manual
        
        audioSource.time = 0f;
        audioSource.Play();
    }

    void Update()
    {
        if (audioSource == null || audioSource.clip == null) return;

        // Jika mesin sedang berjalan normal (belum disuruh mati)
        if (!isStopping)
        {
            // Jika waktu pemutaran menyentuh detik ke-15 (waktuBatas)
            if (audioSource.time >= waktuBatas)
            {
                // Langsung potong/ulangi dari detik ke-6 (waktuUlang)
                audioSource.time = waktuUlang;
            }
            // Jaga-jaga mengatasi bug Unity jika audio terhenti sendiri sebelum Update mengecek
            else if (!audioSource.isPlaying)
            {
                audioSource.Play();
                audioSource.time = waktuUlang;
            }
        }
    }

    // Panggil fungsi ini dari script lain saat mesin akan dimatikan
    // Audio akan terus bermain sisa suaranya tanpa me-loop lagi
    public void StopEngine()
    {
        isStopping = true;
    }
}
