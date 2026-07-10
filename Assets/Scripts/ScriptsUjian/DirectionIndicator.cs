using UnityEngine;

public class DirectionIndicator : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform parkingSpotTarget;

    [Header("Bob & Float Settings")]
    [SerializeField] private float floatHeight = 0.5f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float rotationSpeed = 30f;

    [Header("Visual Settings")]
    [SerializeField] private Color indicatorColor = new Color(1f, 0.85f, 0.2f, 0.35f);
    [SerializeField] private RuntimeAnimatorController coinController;

    private Vector3 startPosition;
    private Material materialInstance;
    private MeshRenderer meshRenderer;
    private Collider indicatorCollider;
    private bool isActive = true;

    private void Start()
    {
        startPosition = transform.position;

        SetupCoinShape();
        SetupMaterial();
        SetupAnimator();

        indicatorCollider = GetComponent<Collider>();
        if (indicatorCollider == null)
        {
            var bc = gameObject.AddComponent<BoxCollider>();
            bc.isTrigger = true;
            indicatorCollider = bc;
        }
        else
        {
            indicatorCollider.isTrigger = true;
            indicatorCollider.enabled = true;
        }

        if (parkingSpotTarget == null)
            Debug.LogWarning("DirectionIndicator: parkingSpotTarget belum di-assign!");
    }

    private void SetupCoinShape()
    {
        transform.localScale = new Vector3(1.5f, 12f, 1.5f);

        var meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
            meshFilter = gameObject.AddComponent<MeshFilter>();

        meshFilter.mesh = Resources.GetBuiltinResource<Mesh>("Cylinder.fbx");
    }

    private void SetupMaterial()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
            meshRenderer = gameObject.AddComponent<MeshRenderer>();

        var shader = Shader.Find("Unlit/Color");
        materialInstance = new Material(shader);
        materialInstance.color = indicatorColor;
        meshRenderer.material = materialInstance;
    }

    private void SetupAnimator()
    {
        if (coinController == null) return;

        var animator = GetComponent<Animator>();
        if (animator == null)
            animator = gameObject.AddComponent<Animator>();

        animator.runtimeAnimatorController = coinController;
    }

    private void Update()
    {
        if (!isActive) return;

        FloatEffect();
    }

    private void FloatEffect()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;

        if (other.CompareTag("Player") || other.GetComponent<mobil>() != null || other.GetComponentInParent<mobil>() != null)
        {
            isActive = false;
            meshRenderer.enabled = false;
            if (indicatorCollider != null) indicatorCollider.enabled = false;
        }
    }
}
