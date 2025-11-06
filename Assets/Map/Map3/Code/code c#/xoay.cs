using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class xoay : MonoBehaviour
{
    [SerializeField] private float _speedRotate, _speedMove;
    Rigidbody2D _rb;
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion rotate = transform.rotation;
        float horizontal = Input.GetAxisRaw("Horizontal");
        //float vertical = Input.GetAxisRaw("Vertical");
        float newAngel = rotate.eulerAngles.z + _speedRotate * Time.deltaTime * horizontal;
        rotate.eulerAngles = new Vector3(0, 0, newAngel);
        transform.rotation = rotate;

        //_rb.velocity = new Vector2(_rb.velocity.x, vertical * _speedMove * Time.deltaTime);
        //_rb.velocity = transform.up * vertical * Time.deltaTime;
    }
}
