using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void LateUpdate()
{
        float moveInput = Input.GetAxis("Vertical");

        if (moveInput < 0)
        {
            // kamera depan mobil (reverse)
            transform.position = target.position + target.forward * 7 + Vector3.up * 3;
        }
        else
        {
            // normal
            transform.position = target.position + target.TransformDirection(offset);
        }

        transform.LookAt(target);
    }
}