using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    [Header("Patrol Settings")]
    public float moveSpeed = 2f;
    public float moveDistance = 2f;
    public float idleTime = 1f;

    [Header("Chase & Attack Settings")]
    public float detectRange = 2f;      
    public float attackRange = 1f;      
    public float attackCooldown = 1.5f; 
    public int damage = 10;

    [Header("Health")]
    public int maxHealth = 100;

    private int currentHealth;
    private Vector3 startPos;
    private bool movingRight = true;
    private bool isIdle = false;
    private bool isDead = false;
    private bool canAttack = true;

    private Transform player;
    private Animator animator;

    private enum State { Patrol, Chase, Attack, Die }
    private State currentState = State.Patrol;

    void Start()
    {
        startPos = transform.position;
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;

        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        SetAnim("isWalking", true);
    }

    void Update()
    {
        if (isDead || player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                if (distanceToPlayer <= detectRange)
                    ChangeState(State.Chase);
                break;

            case State.Chase:
                ChasePlayer(distanceToPlayer);
                break;

            case State.Attack:
                
                break;

            case State.Die:
               
                break;
        }
    }


    void Patrol()
    {
        if (isIdle) return;

        float targetX = startPos.x + (movingRight ? moveDistance : -moveDistance);
        transform.position = Vector3.MoveTowards(transform.position,
            new Vector3(targetX, transform.position.y, transform.position.z),
            moveSpeed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x - targetX) < 0.01f)
        {
            StartCoroutine(IdleAndTurn());
        }

        FlipSprite(movingRight);
    }

    IEnumerator IdleAndTurn()
    {
        isIdle = true;
        SetAnim("isWalking", false);
        yield return new WaitForSeconds(idleTime);
        movingRight = !movingRight;
        isIdle = false;
        SetAnim("isWalking", true);
    }

    
    void ChasePlayer(float distance)
    {
        if (distance > detectRange + 1f)
        {
            
            ChangeState(State.Patrol);
            return;
        }

        if (distance <= attackRange)
        {
            ChangeState(State.Attack);
            return;
        }

        
        Vector3 dir = (player.position - transform.position).normalized;
        transform.position += dir * moveSpeed * 1.5f * Time.deltaTime;
        FlipSprite(player.position.x > transform.position.x);

        SetAnim("isWalking", true);
    }


    IEnumerator AttackPlayer()
    {
        SetAnim("isWalking", false);
        SetAnim("isAttacking", true);

        while (currentState == State.Attack && !isDead)
        {
            if (player == null) yield break;

            float distance = Vector3.Distance(transform.position, player.position);
            if (distance > attackRange)
            {
                SetAnim("isAttacking", false);
                ChangeState(State.Chase);
                yield break;
            }

            if (canAttack)
            {
             
                Debug.Log("Enemy attacks player!");
                canAttack = false;
                yield return new WaitForSeconds(attackCooldown);
                canAttack = true;
            }

            yield return null;
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            ChangeState(State.Die);
        }
    }

    void Die()
    {
        isDead = true;
        SetAnim("isWalking", false);
        SetAnim("isAttacking", false);
        SetAnim("isDead", true);

     
        GetComponent<Collider>().enabled = false;
        Destroy(gameObject, 3f); 
    }

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        switch (newState)
        {
            case State.Patrol:
                SetAnim("isAttacking", false);
                SetAnim("isWalking", true);
                break;
            case State.Chase:
                SetAnim("isAttacking", false);
                SetAnim("isWalking", true);
                break;
            case State.Attack:
                StartCoroutine(AttackPlayer());
                break;
            case State.Die:
                Die();
                break;
        }
    }

    void SetAnim(string param, bool value)
    {
        if (animator != null)
            animator.SetBool(param, value);
    }

    void FlipSprite(bool faceRight)
    {
        Vector3 scale = transform.localScale;
        scale.x = faceRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }
}
