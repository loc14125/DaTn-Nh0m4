using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Tilemaps;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    [SerializeField] private GameObject dan;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float atkSpeed, countDown = 0;
    public float dichuyen;
    public float speed;
    public float jumpForce;
    public bool dcNhay;

    private bool isFaceright = true;
    private Rigidbody2D rb;
    private Animator ani;
    private float horizontal;

    private string currentAni;
    private float vertical;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ani = GetComponent<Animator>();
    }

    void Update()
    {
        dichuyen = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(dichuyen * speed, rb.velocity.y);
        // Jump 
        if (Input.GetKeyDown(KeyCode.Space) && dcNhay) 
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        // lật trái phải
        Flip();
        ani.SetFloat("move", Mathf.Abs(dichuyen));

        move();
        Atk();
    }
    void Flip() 
    {
        if (isFaceright && dichuyen < 0 || !isFaceright && dichuyen > 0) 
        {
            isFaceright = !isFaceright;
            Vector3 kichthuoc = transform.localScale;
            kichthuoc.x = kichthuoc.x * -1;
            transform.localScale = kichthuoc;
        }
    }
    void move() 
    {
        if (Mathf.Abs(horizontal) > 0.1f || Mathf.Abs(vertical) > 0.1f)
        {
            transform.rotation = Quaternion.Euler(new Vector3(0, (horizontal > 0.1f) ? 0 : -180, 0));
            changeAni("atk");
        }
        else 
        {
            changeAni("idle");
        }
    }
    void Atk() 
    {
        countDown -= Time.deltaTime;
        if (countDown > 0)
            return;

        if (Input.GetKeyDown(KeyCode.J))
        {
            changeAni("atk");
            Instantiate(dan, firePoint.position, transform.rotation);
            countDown = atkSpeed;
        }
    }

    private void Instantiate(GameObject dan, Vector3 position)
    {
        throw new NotImplementedException();
    }

    private void changeAni(string aniName) 
    {
        if (currentAni != aniName) 
        {
            ani.ResetTrigger(aniName);
            currentAni = aniName;
            ani.SetTrigger(currentAni);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("vp")) 
        {
            //diemLuu.score++;
        }
    }
}
