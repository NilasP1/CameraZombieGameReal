using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerascript : MonoBehaviour
{
    public Camera playerCamera;
    public Camera[] otherCameras;
    public GameObject player;
    public float cameraSensitivity = 2f;
    public AudioSource clickSound;
    public AudioSource reloadSound;
    public AudioSource movingSound;
    public AudioSource backgroundSound;
    public AudioSource alwaysInCameraSound;
    public AudioSource alwaysOnSound;

    public GameObject effectObject1;
    public GameObject effectObject2;
    public GameObject effectObject3;
    public float effectDuration = 0.2f;

    public int maxBullets = 10;
    private int currentBullets;
    private bool isReloading = false;
    private bool isInCameraCyclingMode = false;
    private int currentCameraIndex = 0;
    private PlayerMovement playerMovementScript;
    private CameraFollow cameraFollowScript;
    private Vector3 lastRotation;
    private bool isPlayingMovingSound = false;
    private bool zombiesStarted = false;

    private int totalBulletsFired = 0; // Track total bullets fired
    private int maxTotalBullets = 100; // Limit for total bullets

    void Start()
    {
        if (playerCamera != null) playerCamera.enabled = true;

        foreach (var camera in otherCameras)
        {
            if (camera != null) camera.enabled = false;
        }

        if (player != null)
        {
            playerMovementScript = player.GetComponent<PlayerMovement>();
            cameraFollowScript = player.GetComponent<CameraFollow>();
        }

        if (effectObject1 != null) effectObject1.SetActive(false);
        if (effectObject2 != null) effectObject2.SetActive(false);
        if (effectObject3 != null) effectObject3.SetActive(false);

        lastRotation = Vector3.zero;
        currentBullets = maxBullets;

        if (alwaysOnSound != null && !alwaysOnSound.isPlaying) alwaysOnSound.Play();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInCameraCyclingMode) ExitCameraCyclingMode();
            else EnterCameraCyclingMode();
        }

        if (isInCameraCyclingMode)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow)) SwitchCamera(-1);
            if (Input.GetKeyDown(KeyCode.RightArrow)) SwitchCamera(1);

            RotateActiveCamera();

            if (Input.GetMouseButtonDown(0)) UseBullet();
            if (Input.GetKeyDown(KeyCode.R)) StartReload();
        }

        CheckCameraMovement();

        // Check if no enemies are left in the scene
        if (AreNoEnemiesLeft())
        {
            if (backgroundSound != null && backgroundSound.isPlaying)
                backgroundSound.Stop();

            GameVictory();
        }
    }

    void UseBullet()
    {
        if (isReloading)
        {
            UnityEngine.Debug.Log("Reloading... Please wait!");
            return;
        }

        if (totalBulletsFired >= maxTotalBullets)
        {
            UnityEngine.Debug.Log("No more bullets left!");
            return;
        }

        if (currentBullets > 0)
        {
            currentBullets--;
            totalBulletsFired++;
            UnityEngine.Debug.Log("Bullets left: " + currentBullets);
            UnityEngine.Debug.Log("Total bullets fired: " + totalBulletsFired);
            PlayClickSound();

            if (currentBullets == 0 && totalBulletsFired < maxTotalBullets)
            {
                UnityEngine.Debug.Log("Out of bullets! Reloading...");
                StartReload();
            }
        }
        else
        {
            UnityEngine.Debug.Log("Out of bullets! Press 'R' to reload.");
        }
    }

    void StartReload()
    {
        if (isReloading || currentBullets >= maxBullets) return;

        if (totalBulletsFired >= maxTotalBullets)
        {
            UnityEngine.Debug.Log("No more bullets left to reload!");
            return;
        }

        isReloading = true;
        UnityEngine.Debug.Log("Reloading...");

        if (reloadSound != null) reloadSound.Play();

        Invoke(nameof(FinishReload), 2.0f); // 2-second reload time
    }

    void FinishReload()
    {
        currentBullets = maxBullets;
        isReloading = false;
        UnityEngine.Debug.Log("Reload complete!");
    }

    void PlayClickSound()
    {
        if (clickSound != null) clickSound.Play();
        ActivateEffect(effectObject1);
        ActivateEffect(effectObject2);
        ActivateEffect(effectObject3);
    }

    void ActivateEffect(GameObject effectObject)
    {
        if (effectObject != null)
        {
            effectObject.SetActive(true);
            Invoke(nameof(DeactivateEffects), effectDuration);
        }
    }

    void DeactivateEffects()
    {
        if (effectObject1 != null) effectObject1.SetActive(false);
        if (effectObject2 != null) effectObject2.SetActive(false);
        if (effectObject3 != null) effectObject3.SetActive(false);
    }

    void EnterCameraCyclingMode()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = false;
        if (cameraFollowScript != null) cameraFollowScript.enabled = false;

        isInCameraCyclingMode = true;
        playerCamera.enabled = false;

        if (otherCameras.Length > 0)
        {
            currentCameraIndex = 0;
            otherCameras[currentCameraIndex].enabled = true;
        }

        if (!zombiesStarted)
        {
            StartZombies();
            zombiesStarted = true;
        }

        if (backgroundSound != null && !backgroundSound.isPlaying) backgroundSound.Play();
        if (alwaysInCameraSound != null && !alwaysInCameraSound.isPlaying) alwaysInCameraSound.Play();
    }

public void ExitCameraCyclingMode()
    {
        if (playerMovementScript != null) playerMovementScript.enabled = true;
        if (cameraFollowScript != null) cameraFollowScript.enabled = true;

        isInCameraCyclingMode = false;

        if (currentCameraIndex < otherCameras.Length) otherCameras[currentCameraIndex].enabled = false;
        if (playerCamera != null) playerCamera.enabled = true;

        StopMovingSound();

        if (backgroundSound != null && backgroundSound.isPlaying) backgroundSound.Stop();
        if (alwaysInCameraSound != null && alwaysInCameraSound.isPlaying) alwaysInCameraSound.Stop();
    }

    void SwitchCamera(int direction)
    {
        if (otherCameras.Length == 0) return;

        otherCameras[currentCameraIndex].enabled = false;

        currentCameraIndex += direction;
        if (currentCameraIndex < 0) currentCameraIndex = otherCameras.Length - 1;
        if (currentCameraIndex >= otherCameras.Length) currentCameraIndex = 0;

        otherCameras[currentCameraIndex].enabled = true;
    }

    void RotateActiveCamera()
    {
        if (otherCameras.Length == 0) return;

        Camera activeCamera = otherCameras[currentCameraIndex];

        float mouseX = Input.GetAxis("Mouse X") * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * cameraSensitivity;

        Vector3 currentRotation = activeCamera.transform.localEulerAngles;

        float newRotationX = currentRotation.x - mouseY;
        float newRotationY = currentRotation.y + mouseX;

        activeCamera.transform.localRotation = Quaternion.Euler(newRotationX, newRotationY, 0f);
    }

    void StartZombies()
    {
        zombieAi[] allZombies = FindObjectsOfType<zombieAi>();
        foreach (zombieAi zombie in allZombies)
        {
            zombie.shouldMove = true;
        }
    }

    void CheckCameraMovement()
    {
        if (otherCameras.Length == 0) return;

        Camera activeCamera = otherCameras[currentCameraIndex];
        Vector3 currentRotation = activeCamera.transform.localEulerAngles;

        if (Vector3.Distance(currentRotation, lastRotation) > 0.01f)
        {
            if (!isPlayingMovingSound) PlayMovingSound();
        }
        else
        {
            if (isPlayingMovingSound) StopMovingSound();
        }

        lastRotation = currentRotation;
    }

    void PlayMovingSound()
    {
        if (movingSound != null && !movingSound.isPlaying)
        {
            movingSound.Play();
            isPlayingMovingSound = true;
        }
    }

    void StopMovingSound()
    {
        if (movingSound != null && movingSound.isPlaying)
        {
            movingSound.Stop();
            isPlayingMovingSound = false;
        }
    }

    bool AreNoEnemiesLeft()
    {
        // Check if there are no active game objects with the tag "enemy" in the scene
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in enemies)
        {
            if (enemy.activeSelf) // If any enemy is still active
                return false;
        }
        return true;
    }

    void GameVictory()
    {
        // Placeholder for victory logic
        UnityEngine.Debug.Log("You have won the game!");
        // Add additional victory-related code here, such as showing a victory screen
    }

    public int GetBulletCount()
    {
        return currentBullets;
    }

    // Add this method to your Camerascript

    public int GetTotalBulletsLeft()
    {
        return maxTotalBullets - totalBulletsFired; // Total bullets = max bullets - bullets fired
    }

}
