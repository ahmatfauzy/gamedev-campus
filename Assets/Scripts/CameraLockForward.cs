using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineCamera))]
public class CameraLockForward : MonoBehaviour
{
    private CinemachineOrbitalFollow orbitalFollow;
    private CinemachineInputAxisController inputController;

    public float fixedPitch = 17.5f;

    void Start()
    {
        orbitalFollow = GetComponent<CinemachineOrbitalFollow>();
        inputController = GetComponent<CinemachineInputAxisController>();

        if (inputController != null)
            inputController.enabled = false;

        if (orbitalFollow != null)
        {
            orbitalFollow.HorizontalAxis.Value = 0;
            orbitalFollow.VerticalAxis.Value = fixedPitch;
        }
    }

    void LateUpdate()
    {
        if (orbitalFollow != null)
        {
            orbitalFollow.HorizontalAxis.Value = 0;
            orbitalFollow.VerticalAxis.Value = fixedPitch;
        }
    }
}
