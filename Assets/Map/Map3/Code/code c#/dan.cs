using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dan : MonoBehaviour
{
    public float _speed;
    public float lifeTime;
    public GameObject effect_dan;
    Rigidbody2D _rb;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        _rb.velocity = transform.right * _speed * Time.deltaTime;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("heo"))
        {
            Destroy(this.gameObject);
            GameObject Explore = Instantiate(effect_dan, transform.position, Quaternion.identity);
            Destroy(Explore, 0.1f);

            var name = other.attachedRigidbody.name;
            Destroy(GameObject.Find(name));

        }
    }
}

