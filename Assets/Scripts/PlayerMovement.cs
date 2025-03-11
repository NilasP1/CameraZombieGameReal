using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float stamina = 100f;
    public float staminaDrainRate = 20f;
    public float staminaRegenRate = 10f;
    public float minStaminaToRun = 5f;
    public float bobbingSpeed = 4f;
    public float bobbingAmount = 0.1f;

    private CharacterController controller;
    private Vector3 moveDirection;
    private float timer = 0f;
    private Vector3 originalCameraPosition;
    public Transform cameraTransform;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        if (cameraTransform != null)
            originalCameraPosition = cameraTransform.localPosition;
    }

    void Update()
    {
        float moveX = 0f;
        float moveZ = 0f;

        if (Input.GetKey(KeyCode.W)) moveZ = 1f;
        if (Input.GetKey(KeyCode.S)) moveZ = -1f;
        if (Input.GetKey(KeyCode.A)) moveX = -1f;
        if (Input.GetKey(KeyCode.D)) moveX = 1f;

        bool isRunning = Input.GetKey(KeyCode.LeftShift) && stamina > minStaminaToRun;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (isRunning)
        {
            stamina -= staminaDrainRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, 100f);
        }
        else
        {
            stamina += staminaRegenRate * Time.deltaTime;
            stamina = Mathf.Clamp(stamina, 0f, 100f);
        }

        moveDirection = transform.forward * moveZ + transform.right * moveX;
        moveDirection.Normalize();
        moveDirection *= currentSpeed;

        controller.Move(moveDirection * Time.deltaTime);

        // Head bobbing effect
        if (moveX != 0 || moveZ != 0)
        {
            timer += bobbingSpeed * Time.deltaTime;
            float bobOffset = Mathf.Sin(timer) * bobbingAmount;
            cameraTransform.localPosition = originalCameraPosition + new Vector3(0, bobOffset, 0);
        }
        else
        {
            timer = 0f;
            cameraTransform.localPosition = originalCameraPosition;
        }
    }
}
