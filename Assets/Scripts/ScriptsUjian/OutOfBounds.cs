using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    [Header("Boundary Limits")]
    [SerializeField] private float xMin = -70f;
    [SerializeField] private float xMax = 70f;
    [SerializeField] private float zMin = -80f;
    [SerializeField] private float zMax = 80f;
    [SerializeField] private float yMin = -10f;

    [Header("Reference")]
    [SerializeField] private GameManager gameManager;

    private void Start()
    {
        if (gameManager == null)
            gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        if (gameManager == null) return;
        if (gameManager.IsMissionComplete() || gameManager.IsMissionFailed()) return;

        Vector3 pos = transform.position;

        if (pos.x < xMin || pos.x > xMax || pos.z < zMin || pos.z > zMax || pos.y < yMin)
        {
            gameManager.OnMissionFailed();
        }
    }
}
