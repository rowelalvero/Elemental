using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    //here
    [SerializeField]
    public float detectionRadius = 5f; //aggro range
    public float attackRange = 1.5f; // default melee range
    public float rangedStopDistance = 4f; // Range targeting distance
    public float meleeStopDistance = 2f; // Melee attack reach
    public float speed = 2f; //speed
    public float attackCooldown = 1.5f; //gaps before actions
    public bool isRanged = false; //changes AI Behavior

    private Transform player;
    private bool isAttacking = false; //calm state
    private Rigidbody2D rb;
    
    private float roamChangeDirFloat = 2f; //unchanged

    void Update()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius) //if player found
        {
            if (!isAttacking) //bool checker 
            {
                if (isRanged)
                    HandleRangedAI(distanceToPlayer);
                else
                    HandleMeleeAI(distanceToPlayer);
            }
        }
    }
    
    void HandleRangedAI(float distance)
    {
        if (distance > rangedStopDistance)
        {
            MoveTowards(player.position); // aggro
        }
        else if (distance < rangedStopDistance - 1f)
        {
            MoveAwayFrom(player.position); // cycle range
        }
        else if (!isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
    
    void HandleMeleeAI(float distance)
    {
        if (distance > meleeStopDistance)
        {
            MoveTowards(player.position); // aggro
        }
        else if (distance < meleeStopDistance - 1f)
        {
            MoveAwayFrom(player.position); //to not block
        }
        else if (!isAttacking)
        {
            StartCoroutine(Attack());
        }
    }
    
    IEnumerator Attack()
    {
        isAttacking = true;
        Debug.Log("Enemy attacks!");


        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }
    
    void MoveTowards(Vector2 target)
    {
        Vector2 direction = (target - (Vector2)transform.position).normalized;
        rb.linearVelocity = direction * speed;
    }

    void MoveAwayFrom(Vector2 target)
    {
        Vector2 direction = ((Vector2)transform.position - target).normalized;
        rb.linearVelocity = direction * speed;
    }
    //here
    private enum State
    {
        Roaming
    }

    private State state;
    private EnemyPathfinding enemyPathfinding;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathfinding>();
        state = State.Roaming;
    }

    private void Start()
    {
        StartCoroutine(RoamingRoutine());
    }

    private IEnumerator RoamingRoutine()
    {
        while (state == State.Roaming)
        {
            Vector2 roamPosition = GetRoamingPosition();
            enemyPathfinding.MoveTo(roamPosition);
            yield return new WaitForSeconds(roamChangeDirFloat);
        }
    }

    private Vector2 GetRoamingPosition()
    {
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
}
