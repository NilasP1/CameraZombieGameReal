using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class DoorDurability : MonoBehaviour
{
    public static int durability = 5;  // Shared durability among all instances
    public AudioSource destructionSound;  // AudioSource for destruction sound
    public AudioSource gameOverSound;     // AudioSource for game over sound

    private bool gameIsOver = false;  // Flag to check if the game is over
    public camerascript cameraScript; // Reference to CameraScript

    // OnTriggerEnter should take a Collider as a parameter, not Collision
    void OnTriggerEnter(Collider other)
    {
        // Log collision here
        UnityEngine.Debug.Log("Collision detected with: " + other.gameObject.name);

        // Check if the object colliding is tagged as "Enemy" and if game is not over
        if (!gameIsOver && other.gameObject.CompareTag("Enemy"))
        {
            if (destructionSound != null && destructionSound.clip != null)
            {
                destructionSound.PlayOneShot(destructionSound.clip);
            }

            Destroy(other.gameObject);  // Use 'other' instead of 'collision'
            DecreaseDurability();
        }
    }

    // OnTriggerExit should also use Collider as a parameter
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            UnityEngine.Debug.Log("Exited collider");
        }
    }

    void DecreaseDurability()
    {
        if (gameIsOver) return;  // Do nothing if the game is over

        durability--;

        // If durability reaches zero, trigger game over scenario
        if (durability <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        // Implement game over logic (e.g., disable objects, trigger event, etc.)
        UnityEngine.Debug.Log("GameOver");

        // Play the game over sound on loop
        if (gameOverSound != null && !gameOverSound.isPlaying)
        {
            gameOverSound.loop = true;
            gameOverSound.Play();
        }

        // Deactivate all objects with the "Enemy" tag
        DeactivateEnemies();

        // Set gameIsOver to true to prevent further damage
        gameIsOver = true;

        // Disable CameraScript and exit camera mode
        if (cameraScript != null)
        {
            cameraScript.ExitCameraCyclingMode();  // Exit camera cycling mode
            cameraScript.enabled = false;  // Disable the CameraScript entirely
        }

        // Optionally, you can disable the door or other game objects here
        // Example: this.gameObject.SetActive(false);
    }

    // Function to deactivate all enemies
    void DeactivateEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            enemy.SetActive(false);  // Deactivate the enemy object
        }
    }
}
