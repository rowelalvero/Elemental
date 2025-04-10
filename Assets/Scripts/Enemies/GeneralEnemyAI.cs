using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralEnemyAI : MonoBehaviour
{
    [Header("Roaming & Attack Settings")]
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 3f;
    [SerializeField] private bool stopMovingWhileAttacking = false;

    [Header("Attack Settings")]
    [SerializeField] private float attackCooldown = 2f;

    [SerializeField] private List<MonoBehaviour> attackScripts = new(); // List of attack scripts

    private List<IEnemy> validAttacks = new(); // Valid attack scripts
    private IEnemy currentAttackScript; // Current attack script

    private EnemyPathfinding pathfinding;
    private bool canAttack = true;
    private bool waitingForAttackToEnd = false;

    private enum State
    {
        Roaming,
        Attacking,
    }

    private State state;
    private Vector2 roamDirection;
    private float timeRoaming = 0f;
    private float attackCooldownTimer = 0f;
    private float attackWaitTime = 3f;  // Delay before returning to roam after attack

    private void Awake()
    {
        pathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;

        // Validate attack scripts
        foreach (var script in attackScripts)
        {
            if (script is IEnemy enemy)
            {
                validAttacks.Add(enemy);
            }
            else
            {
                Debug.LogWarning($"{script.name} does not implement IEnemy and will be ignored.");
            }
        }

        if (validAttacks.Count > 0)
        {
            // Initially set a random attack
            RandomizeAndSetAttack();
        }
        else
        {
            Debug.LogError($"No valid IEnemy attack scripts found on {name}.");
        }
    }

    private void Start()
    {
        roamDirection = GetRandomDirection();
    }

    private void Update()
    {
        MovementStateControl();
    }

    private void MovementStateControl()
    {
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            if (state == State.Roaming)
            {
                Roaming(distanceToPlayer);
            }
            else if (state == State.Attacking)
            {
                Attacking(distanceToPlayer);
            }
        }
        else
        {
            // Handle case when player is not in the scene
            HandlePlayerNotInScene();
        }
    }

    private void Roaming(float distanceToPlayer)
    {
        timeRoaming += Time.deltaTime;

        if (!waitingForAttackToEnd)
        {
            pathfinding.MoveTo(roamDirection); // Move in the roaming direction
        }

        // Switch to Attacking if the player is within attack range
        if (distanceToPlayer < attackRange)
        {
            state = State.Attacking; // Attack when within range
            StopAndFindPlayer(); // Stop movement and look for the player
        }

        // Change roam direction after some time
        if (timeRoaming > roamChangeDirFloat)
        {
            roamDirection = GetRandomDirection();
        }
    }

    private void Attacking(float distanceToPlayer)
    {
        if (canAttack && !waitingForAttackToEnd && currentAttackScript != null)
        {
            canAttack = false;
            waitingForAttackToEnd = true;

            pathfinding.StopMoving(); // Stop movement during attack

            currentAttackScript.Attack(OnAttackFinished); // Execute attack and wait for it to finish
        }
    }

    private void OnAttackFinished()
    {
        // Once the attack has finished, randomize the next attack
        waitingForAttackToEnd = false;

        // Check if the player is in attack range after the attack
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);

        if (distanceToPlayer > attackRange)
        {
            // Wait for 3 seconds before returning to roam
            StartCoroutine(WaitForRoamingTransition());
        }
        else
        {
            // If the player is still in range, stay attacking
            state = State.Attacking;
        }

        // Randomize the attack to be used after cooldown
        RandomizeAndSetAttack();

        // Start cooldown before the next attack
        StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator WaitForRoamingTransition()
    {
        // Wait for 3 seconds before switching back to Roaming
        yield return new WaitForSeconds(attackWaitTime);
        state = State.Roaming; // Switch to Roaming state after the delay
        StopAndFindPlayer();  // Stop movement and start roaming
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private void RandomizeAndSetAttack()
    {
        // Randomly pick a new attack script from the valid attacks list
        if (validAttacks.Count > 0)
        {
            currentAttackScript = validAttacks[Random.Range(0, validAttacks.Count)];
            Debug.Log($"Selected new attack: {currentAttackScript.GetType().Name}");
        }
        else
        {
            Debug.LogWarning("No valid attacks to select.");
        }
    }

    private Vector2 GetRandomDirection()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void StopAndFindPlayer()
    {
        pathfinding.StopMoving(); // Immediately stop movement
        Debug.Log("Enemy stopped and is now finding the player.");
    }

    private void HandlePlayerNotInScene()
    {
        // If no player, keep the enemy roaming randomly
        if (state == State.Roaming && !waitingForAttackToEnd)
        {
            roamDirection = GetRandomDirection();
            pathfinding.MoveTo(roamDirection);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);   // Attack range
    }
}
