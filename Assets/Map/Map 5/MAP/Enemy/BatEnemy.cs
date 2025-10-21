using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatEnemy : MonoBehaviour
{
    public float detectRange = 10f;       
    public float attackRange = 1f;       
    public float speed = 2f;             
    public float attackSpeed = 5f;       
    public float health = 100f;          
    private Transform player;
    private Animator anim;
    private Vector3 startPos;
    private bool isDead = false;
    private bool isAttacking = false;

    private enum State { Idle, Run, Attack, Die }
    private State currentState = State.Idle;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
        startPos = transform.position;
        ChangeState(State.Idle);
    }

    void Update()
    {
        if (isDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentState)
        {
            case State.Idle:
                IdleState(distance);
                break;

            case State.Run:
                RunState(distance);
                break;

            case State.Attack:
                AttackState(distance);
                break;
        }
    }

   

    void IdleState(float distance)
    {
       
        transform.position = startPos + new Vector3(0, Mathf.Sin(Time.time * 2f) * 0.1f, 0);

        if (distance <= detectRange)
        {
            ChangeState(State.Run);
        }
    }

    void RunState(float distance)
    {
        
        Vector2 direction = (player.position - transform.position).normalized;
        transform.position += (Vector3)direction * speed * Time.deltaTime;

        
        if (direction.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else
            transform.localScale = new Vector3(-1, 1, 1);

        if (distance <= attackRange)
        {
            ChangeState(State.Attack);
        }
        else if (distance > detectRange + 1f)
        {
            
            ChangeState(State.Idle);
        }
    }

    void AttackState(float distance)
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.SetTrigger("Attack");
            StartCoroutine(AttackRoutine());
        }

        if (distance > attackRange + 1f && !isAttacking)
        {
            ChangeState(State.Run);
        }
    }

    System.Collections.IEnumerator AttackRoutine()
    {
        
        float timer = 0.5f;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            Vector2 direction = (player.position - transform.position).normalized;
            transform.position += (Vector3)direction * attackSpeed * Time.deltaTime;
            yield return null;
        }

        isAttacking = false;
        ChangeState(State.Run);
    }

   

    void ChangeState(State newState)
    {
        if (currentState == newState) return;
        currentState = newState;

        
        anim.SetBool("Idle", false);
        anim.SetBool("Run", false);

        switch (newState)
        {
            case State.Idle:
                anim.SetBool("Idle", true);
                break;
            case State.Run:
                anim.SetBool("Run", true);
                break;
            case State.Attack:
                
                break;
            case State.Die:
                anim.SetTrigger("Die");
                break;
        }
    }


    public void TakeDamage(float damage)
    {
        if (isDead) return;

        health -= damage;
        if (health <= 0)
        {
            isDead = true;
            ChangeState(State.Die);
            Destroy(gameObject, 1.5f);
        }
    }
}