using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sinhquai : MonoBehaviour
{
    [SerializeField] GameObject heoPrefab;
    [SerializeField] float spwanRate = 1.5f, spwanRadius = 4f;
    private float spwanTimer = 0;
    private Vector2 spawnRadius;

    private Animator _ani;
   
    //Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        //_rb = GetComponent<Rigidbody2D>();
        _ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        spwanTimer += Time.deltaTime;
        if (spwanTimer > spwanRate)
        {
            spawnEnemy();
            spwanTimer = 0;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spwanRadius);
    }

    private void spawnEnemy()
    {
        Vector2 randomPostion = (Vector2)transform.position + Random.insideUnitCircle.normalized * spawnRadius;
        Instantiate(heoPrefab, randomPostion, Quaternion.identity);
       // Vector3.Distance(randomPostion, spawnRadius);
    }
}
