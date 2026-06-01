using UnityEngine;

public class CarController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float turnSpeed = 100f;

    private float moveInput;
    private float turnInput;

    void Update()
    {
        moveInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }

    void FixedUpdate()
    {
        // Gerak maju mundur
        transform.Translate(Vector3.forward * moveInput * moveSpeed * Time.deltaTime);

        // Belok
        transform.Rotate(Vector3.up * turnInput * turnSpeed * Time.deltaTime);
    }
}