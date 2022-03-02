using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;

public class TPController : MonoBehaviour
{
	[Header("References")]
    [SerializeField] Camera cam;
	[SerializeField] Rig aimLayer;

	[Header("Settings")]
	[SerializeField] float turnSpeed = 15f;

    [SerializeField] float normalSensitivity = 15f;
    [SerializeField] float aimSensitivity = 15f;

	[SerializeField] float aimDuration = 0.3f;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	public GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	public float TopClamp = 70.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	public float BottomClamp = -30.0f;
	[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
	public float CameraAngleOverride = 0.0f;
	[Tooltip("For locking the camera position on all axis")]
	public bool LockCameraPosition = false;

	float sensitivity = 0f;

	Vector2 lookInput;

	float _cinemachineTargetYaw;
	float _cinemachineTargetPitch;

	const float _threshold = 0.01f;

	bool isAiming = false;

    private void Start()
    {
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
    }

    private void FixedUpdate()
    {
		sensitivity = isAiming ? aimSensitivity : normalSensitivity;

        float yawCamera = cam.transform.eulerAngles.y;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0f, yawCamera, 0f), turnSpeed * Time.fixedDeltaTime);

		Aiming();
    }

    private void LateUpdate()
    {
		CameraRotation();
    }

    private void CameraRotation()
	{
		// if there is an input and camera position is not fixed
		if (lookInput.sqrMagnitude >= _threshold && !LockCameraPosition)
		{
			_cinemachineTargetYaw += lookInput.x * Time.deltaTime * sensitivity;
			_cinemachineTargetPitch += lookInput.y * Time.deltaTime * sensitivity;
		}

		// clamp our rotations so our values are limited 360 degrees
		_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
		_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

		// Cinemachine will follow this target
		CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, _cinemachineTargetYaw, 0.0f);
	}

	private void Aiming()
    {
    }

	public void OnLook(InputAction.CallbackContext context)
	{
		lookInput = context.ReadValue<Vector2>();
	}

	public void OnAim(InputAction.CallbackContext context)
    {
		if (context.phase == InputActionPhase.Started)
        {
			isAiming = true;
		}
		else if (context.phase == InputActionPhase.Canceled)
        {
			isAiming = false;
		}
    }

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}
}
