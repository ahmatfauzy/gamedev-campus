using UnityEngine;

public class ParkingSpot : MonoBehaviour
{
    [Header("Visual Settings")]
    [SerializeField] private Color defaultColor = new Color(1f, 0.3f, 0.3f, 0.5f);
    [SerializeField] private Color occupiedColor = new Color(0.2f, 1f, 0.2f, 0.5f);
    [SerializeField] private Color glowColor = new Color(0.2f, 1f, 0.2f, 0.8f);

    [Header("Parking Requirements")]
    [SerializeField] private float stopSpeedThreshold = 0.5f;
    [SerializeField] private float requiredStayDuration = 1f;

    [Header("References")]
    [SerializeField] private GameManager gameManager;

    private MeshRenderer meshRenderer;
    private Material materialInstance;
    private Collider spotCollider;
    private GameObject carRoot;
    private Collider carBodyCollider;
    private Rigidbody carRigidbody;
    private bool isOccupied;
    private float stayTimer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        spotCollider = GetComponent<Collider>();

        if (meshRenderer != null)
        {
            materialInstance = meshRenderer.material;
            materialInstance.color = defaultColor;
            materialInstance.EnableKeyword("_EMISSION");
            materialInstance.SetColor("_EmissionColor", defaultColor * 0.3f);
        }
    }

    private void Update()
    {
        if (isOccupied) return;
        if (carRoot == null) return;

        stayTimer += Time.deltaTime;

        bool isFullyInside = IsCarFullyInsideXZ();
        bool isStopped = IsCarStopped();

        if (isFullyInside && isStopped && stayTimer >= requiredStayDuration)
        {
            isOccupied = true;
            OnParked();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOccupied) return;
        TryCaptureCar(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (isOccupied) return;
        if (carRoot == null)
            TryCaptureCar(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (isOccupied) return;

        GameObject root = GetCarRoot(other.gameObject);
        if (root == carRoot)
        {
            carRoot = null;
            carBodyCollider = null;
            carRigidbody = null;
            stayTimer = 0f;
        }
    }

    private void TryCaptureCar(Collider other)
    {
        if (!IsPlayerCar(other)) return;

        carRoot = GetCarRoot(other.gameObject);
        carRigidbody = carRoot.GetComponent<Rigidbody>();
        carBodyCollider = carRoot.GetComponent<Collider>();

        if (carBodyCollider == null)
            carBodyCollider = carRoot.GetComponentInChildren<Collider>();

        stayTimer = 0f;
    }

    private GameObject GetCarRoot(GameObject obj)
    {
        Transform t = obj.transform;
        while (t != null)
        {
            if (t.CompareTag("Player") || t.GetComponent<mobil>() != null)
                return t.gameObject;
            t = t.parent;
        }
        return obj;
    }

    private bool IsPlayerCar(Collider other)
    {
        return other.CompareTag("Player")
            || other.GetComponent<mobil>() != null
            || other.GetComponentInParent<mobil>() != null;
    }

    private bool IsCarFullyInsideXZ()
    {
        if (carBodyCollider == null || spotCollider == null) return false;

        Bounds carBounds = carBodyCollider.bounds;
        Bounds spotBounds = spotCollider.bounds;

        float carMinX = carBounds.min.x;
        float carMaxX = carBounds.max.x;
        float carMinZ = carBounds.min.z;
        float carMaxZ = carBounds.max.z;

        bool insideX = carMinX >= spotBounds.min.x && carMaxX <= spotBounds.max.x;
        bool insideZ = carMinZ >= spotBounds.min.z && carMaxZ <= spotBounds.max.z;

        return insideX && insideZ;
    }

    private bool IsCarStopped()
    {
        if (carRigidbody == null) return true;
        return carRigidbody.linearVelocity.magnitude < stopSpeedThreshold;
    }

    private void OnParked()
    {
        if (materialInstance != null)
        {
            materialInstance.color = occupiedColor;
            materialInstance.SetColor("_EmissionColor", glowColor);
        }

        if (gameManager != null)
        {
            gameManager.OnMissionComplete();
        }
    }
}
