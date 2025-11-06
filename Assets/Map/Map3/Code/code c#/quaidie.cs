using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class codeenemydichuyen : MonoBehaviour
{
    public float moveSpeed = 0.5f;
    private Transform playTransForm;
    void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null)
        {
            playerObject = FindObjectOfType<GameObject>();
        }
        if (playerObject != null)
        {
            playTransForm = playerObject.transform;
        }
        else
        {
            Debug.Log("no Player");
        }
    }

    void Update()
    {
        if (playTransForm != null)
        {
            Vector3 direction = (playTransForm.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.deltaTime);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            collision.gameObject.SetActive(false);
        }
        if (collision.tag == "dan")
        {
            Destroy(gameObject);
        }
    }
}
