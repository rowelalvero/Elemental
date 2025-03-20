using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float rangedStopDistance = 4f;
    [SerializeField] private float meleeStopDistance = 2f;
    [SerializeField] private float speed = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private bool isRanged = false;

    private Transform player;
    private bool isAttacking = false;
    private bool isRoaming = true;
    private Rigidbody2D rb;
    private Vector2 lastRoamDirection;
    private Coroutine roamingCoroutine;

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
        roamingCoroutine = StartCoroutine(RoamingRoutine());
    }

    private void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            if (state != State.Chasing)
            {
                state = State.Chasing;
                StopRoaming();
            }

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
            state = State.Roaming;
            StartRoaming();
        }
    }

    private void HandleRangedAI(float distance)
    {
        if (distance > rangedStopDistance + 0.5f)
        {
            Debug.Log("aggro ranged!");
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
        if (distance > meleeStopDistance + 0.5f)
        {
            Debug.Log("aggro melee!");
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

    // Movement Functions
    private void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    private void MoveAwayFrom(Vector2 target)
    {
        Debug.Log("Back away!");
        Vector2 direction = ((Vector2)transform.position - target).normalized;
        rb.linearVelocity = direction * speed;
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

    private void StartRoaming()
    {
        if (roamingCoroutine != null)
            StopCoroutine(roamingCoroutine);

        roamingCoroutine = StartCoroutine(RoamingRoutine());
    }

    private void StopRoaming()
    {
        if (roamingCoroutine != null)
        {
            StopCoroutine(roamingCoroutine);
            roamingCoroutine = null;
        }

        rb.linearVelocity = Vector2.zero;
    }

    private Vector2 GetRoamingPosition()
    {
        lastRoamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
        return (Vector2)transform.position + lastRoamDirection * 2f;
    }
}
