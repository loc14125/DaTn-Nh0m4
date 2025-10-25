using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class heo : MonoBehaviour
{
    [SerializeField] float moveSpeed = 0.5f;
    public Transform Player;
    public SpriteRenderer heoSR;
    void Start()
    {
        GameObject PlayerObject = GameObject.FindGameObjectWithTag("Player");
        if (PlayerObject == null) 
        {
            PlayerObject = FindObjectOfType<GameObject>();
        }
        if(PlayerObject != null) 
        {
            Player = PlayerObject.transform;
        }
        else
        {
            Debug.Log("Khong tim thay");
        }
    }

    void Update()
    {
        if(Player != null) 
        {
            Vector2 dir = (Player.position - transform.position).normalized;
            Vector3 faceHeo = dir * moveSpeed * Time.deltaTime;
            transform.Translate(faceHeo);

            if (faceHeo.x != 0)
            { if (faceHeo.x < 0)
                {
                    heoSR.transform.localScale = new Vector3(1, 1, 1);
                }
                else 
                {
                    heoSR.transform.localScale = new Vector3(-1, 1, 1);
                }
            }
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
