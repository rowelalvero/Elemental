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

    [SerializeField] private List<MonoBehaviour> attackScripts = new();

    private List<IEnemy> validAttacks = new();
    private IEnemy currentAttackScript;

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
            currentAttackScript = validAttacks[Random.Range(0, validAttacks.Count)];
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

    private void Roaming(float distanceToPlayer)
    {
        timeRoaming += Time.deltaTime;

        if (!waitingForAttackToEnd)
        {
            pathfinding.MoveTo(roamDirection);
        }

        // Switch to Attacking if the player is within attack range
        if (distanceToPlayer < attackRange)
        {
            state = State.Attacking; // Attack when within range
        }

        // Change roam direction after some time
        if (timeRoaming > roamChangeDirFloat)
        {
            roamDirection = GetRandomDirection();
        }
    }

    private void Attacking(float distanceToPlayer)
    {
        if (distanceToPlayer > attackRange && !waitingForAttackToEnd)
        {
            state = State.Roaming; // Return to roaming if the player moves out of attack range
        }

        if (canAttack && !waitingForAttackToEnd && currentAttackScript != null)
        {
            canAttack = false;
            waitingForAttackToEnd = true;

            pathfinding.StopMoving(); // Stop moving during attack

            currentAttackScript.Attack(OnAttackFinished);
        }
    }

    private void OnAttackFinished()
    {
        // Once the attack has finished, return to roaming state
        waitingForAttackToEnd = false;
        state = State.Roaming;

        // Start cooldown before the next attack
        StartCoroutine(AttackCooldownRoutine());
    }

    private IEnumerator AttackCooldownRoutine()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    private Vector2 GetRandomDirection()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);   // Attack range
    }
}
