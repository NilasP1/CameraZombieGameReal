using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class zombieAi : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 2f;
    public float stopDistance = 1f;
    public Transform spawnLocation;
    public Animator animator;
    public GameObject zombiePrefab;

    public GameObject triggerArea1;
    public GameObject triggerArea2;
    public GameObject triggerArea3;

    private bool isDead = false;
    private bool isInTriggerArea = false;
    public bool shouldMove = false; // Control when zombies move

    private camerascript cameraScript; // Reference to the camera script

    // Static variable to track total zombies killed
    private static int zombieDeathCount = 0;
    private const int maxZombiesToKill = 50; // Once 50 zombies are killed, stop duplicating

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Find the camera script in the scene
        cameraScript = FindObjectOfType<camerascript>();
    }

    void Update()
    {
        if (target == null || animator == null) return;

        if (!isDead && shouldMove)
        {
            MoveTowardsTarget();
            HandleAnimations();
        }

        CheckForDeath();
    }

    void MoveTowardsTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);
            transform.LookAt(target);
        }
    }

    void HandleAnimations()
    {
        if (isDead) return;

        float distance = Vector3.Distance(transform.position, target.position);

        if (distance > stopDistance && distance <= 5f)
        {
            animator.Play("Z_Walk1_InPlace");
        }
        else if (distance <= stopDistance)
        {
            animator.Play("Z_Idle");
        }
    }

    void ZDie()
    {
        // Prevent zombie death if bullets are at zero
        if (cameraScript != null && cameraScript.GetBulletCount() == 0)
        {
            UnityEngine.Debug.Log("Zombie can't die, no bullets left!");
            return;
        }

        if (animator != null && !isDead)
        {
            isDead = true;
            animator.SetBool("IsDead", true);
            animator.Play("Z_FallingForward");

            zombieDeathCount++; // Increment the death counter

            // If the count reaches 50, stop spawning and duplicating zombies
            if (zombieDeathCount >= maxZombiesToKill)
            {
                UnityEngine.Debug.Log("Maximum zombies killed. No more duplicates will spawn.");
                Invoke(nameof(DeactivateZombie), 2f); // Deactivate zombie after death animation
                return;
            }

            // If under 50 zombies are killed, spawn new zombies
            Invoke(nameof(TeleportZombie), 2f);
            Invoke(nameof(SpawnDuplicate), 3f);
        }
    }

    void CheckForDeath()
    {
        if (target == null) return;

        if (Input.GetMouseButtonDown(0) && isInTriggerArea)
        {
            ZDie();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == triggerArea1 || other.gameObject == triggerArea2 || other.gameObject == triggerArea3)
        {
            isInTriggerArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == triggerArea1 || other.gameObject == triggerArea2 || other.gameObject == triggerArea3)
        {
            isInTriggerArea = false;
        }
    }

    void TeleportZombie()
    {
        if (spawnLocation != null)
        {
            transform.position = spawnLocation.position;
            isDead = false;

            animator.SetBool("IsDead", false);
            animator.Play("Z_Idle");
        }
    }

    void SpawnDuplicate()
    {
        // If 50 zombies have died, no new zombies will be spawned
        if (zombieDeathCount >= maxZombiesToKill) return;

        if (spawnLocation != null && zombiePrefab != null)
        {
            Instantiate(zombiePrefab, spawnLocation.position, spawnLocation.rotation);
        }
    }

    void DeactivateZombie()
    {
        // Deactivate zombie after death animation (2 seconds after death animation starts)
        if (isDead)
        {
            gameObject.SetActive(false);
        }
    }
}
