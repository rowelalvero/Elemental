using System;
using System.Collections;
using UnityEngine;

public class EnemyChargeAttack : MonoBehaviour, IEnemy
{
    [Header("Attack Settings")]
    [SerializeField] private float windUpTime = 1f;
    [SerializeField] private float dashSpeed = 8f;
    [SerializeField] private float dashDistance = 1.5f; // Distance the charge will cover
    [SerializeField] private GameObject warningUIPrefab; // Warning UI prefab
    [SerializeField] private GameObject damageCollider;  // Attack collider prefab
    [SerializeField] private Transform spawnAttack;      // Anchor point for the attack
    [SerializeField] private bool stopMovingWhileCharging = false; // Option to stop moving while charging

    private bool isAttacking = false;
    private bool isCharging = false; // New flag to control charging state
    private Transform player;
    private Rigidbody2D rb;
    private Animator animator;
    private Vector2 savedAttackDirection;
    private GameObject warningUIInstance;
    private Action onAttackFinished;
    private const string PlayerTag = "Player";
    private readonly int AttackHash = Animator.StringToHash("isAttacking");

    // IEnemy implementation
    public bool IsAttacking => isAttacking;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindWithTag(PlayerTag)?.transform;

        if (warningUIPrefab != null)
        {
            // Create the warning UI and attach it to the enemy's position
            warningUIInstance = Instantiate(warningUIPrefab, transform.position, Quaternion.identity);
            warningUIInstance.transform.SetParent(transform); // Attach to enemy
            warningUIInstance.SetActive(false); // Initially hidden
        }

        if (damageCollider != null)
        {
            damageCollider.SetActive(false); // Initially inactive
        }
    }

    public void Attack(Action onComplete)
    {
        if (!isAttacking && !isCharging)
        {
            onAttackFinished = onComplete;
            StartCoroutine(AttackRoutine());
        }
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // Step 1: Prepare for attack
        rb.linearVelocity = Vector2.zero;  // Stop all movement
        savedAttackDirection = (player.position - transform.position).normalized; // Calculate direction to player

        // Step 2: Wind-up (charging state)
        isCharging = true;  // Begin charging state, disallow other movement

        if (animator != null)
        {
            animator.SetBool(AttackHash, true); // Trigger the charge animation
        }

        // Show warning UI during wind-up time
        if (warningUIInstance != null)
        {
            // Calculate the position at the end of the dash
            Vector2 dashEndPosition = (Vector2)transform.position + savedAttackDirection * dashDistance;
            warningUIInstance.transform.position = dashEndPosition; // Move the warning UI to the dash end point
            warningUIInstance.SetActive(true);
        }

        yield return new WaitForSeconds(windUpTime); // Wait for wind-up time to complete

        // Remove warning UI before starting the dash
        if (warningUIInstance != null)
        {
            warningUIInstance.SetActive(false); // Hide warning UI
        }

        // Step 3: Prepare collider for damage
        if (damageCollider != null)
        {
            damageCollider.SetActive(true); // Activate the damage collider
        }

        // Step 4: Perform Dash - Gradual movement towards player
        float distanceCovered = 0f;  // Track how much distance we've covered during the dash

        while (distanceCovered < dashDistance) // Keep moving until the total dash distance is covered
        {
            // Move towards the target direction at the desired speed
            float moveDistance = dashSpeed * Time.deltaTime;
            rb.linearVelocity = savedAttackDirection * dashSpeed; // Move towards the player

            distanceCovered += moveDistance;  // Increment the distance covered

            // Update the position of the enemy
            transform.position += (Vector3)savedAttackDirection * moveDistance;

            yield return null; // Wait for the next frame
        }

        // After the dash is complete, stop movement and reset states
        rb.linearVelocity = Vector2.zero; // Stop the movement

        // Step 5: Deactivate damage collider after the attack
        if (damageCollider != null)
        {
            damageCollider.SetActive(false); // Deactivate damage collider after the dash
        }

        // Clean-up and reset attack state
        if (animator != null)
        {
            animator.SetBool(AttackHash, false); // Reset attack animation state
        }

        isCharging = false; // End charging state
        isAttacking = false; // Reset attacking state
        onAttackFinished?.Invoke(); // Notify that the attack is finished
        onAttackFinished = null; // Clear the callback
        rb.linearVelocity = Vector2.zero;
    }

    private void OnDestroy()
    {
        if (warningUIInstance != null)
            Destroy(warningUIInstance); // Clean up warning UI when object is destroyed
    }

    public Transform GetAttackCollider()
    {
        return damageCollider != null ? damageCollider.transform : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, dashDistance); // Visualize dash distance in the editor
    }
}
