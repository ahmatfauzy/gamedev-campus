using UnityEngine;
using UnityEngine.EventSystems;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    public float moveInput;
    private float turnInput;
    
    // Variabel untuk menampung input dari UI Canvas
    private float uiGasInput = 0f;
    private float uiMundurInput = 0f;
    private bool isBraking = false;

    void Start()
    {
        // Sistem untuk otomatis menyambungkan tombol UI tanpa harus diset manual di editor
        // Mencari dengan variasi huruf besar/kecil
        HubungkanTombol("gas", TekanGas, LepasGas);
        HubungkanTombol("Gas", TekanGas, LepasGas);
        HubungkanTombol("GAS", TekanGas, LepasGas);
        
        HubungkanTombol("mundur", TekanMundur, LepasMundur);
        HubungkanTombol("Mundur", TekanMundur, LepasMundur);
        HubungkanTombol("MUNDUR", TekanMundur, LepasMundur);
        
        HubungkanTombol("rem", TekanRem, LepasRem);
        HubungkanTombol("Rem", TekanRem, LepasRem);
        HubungkanTombol("REM", TekanRem, LepasRem);
    }

    private void HubungkanTombol(string namaTombol, UnityEngine.Events.UnityAction actionDown, UnityEngine.Events.UnityAction actionUp)
    {
        GameObject tombol = GameObject.Find(namaTombol);
        if (tombol != null)
        {
            // Tambahkan EventTrigger jika belum ada di tombol tersebut
            EventTrigger trigger = tombol.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = tombol.AddComponent<EventTrigger>();
            }

            // Tambahkan aksi saat ditekan (PointerDown)
            EventTrigger.Entry entryDown = new EventTrigger.Entry();
            entryDown.eventID = EventTriggerType.PointerDown;
            entryDown.callback.AddListener((data) => { actionDown(); });
            trigger.triggers.Add(entryDown);

            // Tambahkan aksi saat dilepas (PointerUp)
            EventTrigger.Entry entryUp = new EventTrigger.Entry();
            entryUp.eventID = EventTriggerType.PointerUp;
            entryUp.callback.AddListener((data) => { actionUp(); });
            trigger.triggers.Add(entryUp);
        }
    }

    void Update()
    {
        // Tetap membaca keyboard agar bisa pakai panah/WASD
        float keyboardMove = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");

        // Logika Rem: Jika rem ditekan, mobil dipaksa berhenti
        if (isBraking)
        {
            moveInput = 0f;
        }
        else
        {
            // Gabungkan input dari UI (Gas = 1, Mundur = -1)
            float uiMove = uiGasInput + uiMundurInput;

            // Jika ada tombol UI yang ditekan, gunakan UI. Jika tidak, gunakan keyboard
            if (uiMove != 0f)
            {
                moveInput = uiMove;
            }
            else
            {
                moveInput = keyboardMove;
            }
        }
    }

    void FixedUpdate()
    {
        // Gerak maju mundur
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);

        // Belok
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
    }

    // --- FUNGSI UNTUK TOMBOL UI KANVAS ---

    public void TekanGas()
    {
        uiGasInput = 1f;
        isBraking = false; // Matikan rem otomatis jika digas
    }

    public void LepasGas()
    {
        uiGasInput = 0f;
    }

    public void TekanMundur()
    {
        uiMundurInput = -1f;
        isBraking = false;
    }

    public void LepasMundur()
    {
        uiMundurInput = 0f;
    }

    public void TekanRem()
    {
        isBraking = true;
    }

    public void LepasRem()
    {
        isBraking = false;
    }
}