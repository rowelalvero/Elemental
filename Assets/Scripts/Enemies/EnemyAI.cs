using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f; // Aggro range
    [SerializeField] private float attackRange = 1.5f;   // Default melee range
    [SerializeField] private float rangedStopDistance = 4f; // Ranged attack distance
    [SerializeField] private float meleeStopDistance = 2f; // Melee attack reach
    [SerializeField] private float speed = 2f; // Movement speed
    [SerializeField] private float attackCooldown = 1.5f; // Cooldown before next attack
    [SerializeField] private bool isRanged = false; // Defines AI attack behavior
    
    private Transform player;
    private bool isAttacking = false;
    private Rigidbody2D rb;
    private Vector2 lastRoamDirection;
    
    private enum State
    {
        Roaming,
        Chasing,
        Attacking
    }
    
    private State state;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Start()
    {
        state = State.Roaming;
        StartCoroutine(RoamingRoutine());
    }

    private void Update()
    {
        if (player == null) return; // Prevents errors if player is missing
        
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        
        if (distanceToPlayer <= detectionRadius)
        {
            state = State.Chasing;
            if (!isAttacking)
            {
                if (isRanged)
                    HandleRangedAI(distanceToPlayer);
                else
                    HandleMeleeAI(distanceToPlayer);
            }
        }
        else if (state == State.Chasing)
        {
            // Player escaped, return to roaming
            state = State.Roaming;
            StartCoroutine(RoamingRoutine());
        }
    }

    private void HandleRangedAI(float distance)
    {
        if (distance > rangedStopDistance + 0.5f) // Buffer added to avoid rapid switching
        {
            MoveTowards(player.position);
        }
        else if (distance < rangedStopDistance - 1f)
        {
            MoveAwayFrom(player.position);
        }
        else if (!isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private void HandleMeleeAI(float distance)
    {
        if (distance > meleeStopDistance + 0.5f) // Buffer added
        {
            MoveTowards(player.position);
        }
        else if (distance < meleeStopDistance - 1f)
        {
            MoveAwayFrom(player.position);
        }
        else if (!isAttacking)
        {
            StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        isAttacking = true;
        Debug.Log("Enemy attacks!");
        
        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.velocity = direction * speed;
    }

    private void MoveAwayFrom(Vector2 target)
    {
        Vector2 direction = ((Vector2)transform.position - target).normalized;
        rb.velocity = direction * speed;
    }

    // Roaming Behavior
    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            MoveTowards(roamPosition);
            yield return new WaitForSeconds(2f);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        lastRoamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        return (Vector2)transform.position + lastRoamDirection * 2f;
    }
}
