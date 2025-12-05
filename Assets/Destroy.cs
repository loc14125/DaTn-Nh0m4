using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    public float lifeTime = 0.6f; // thời gian tồn tại của hiệu ứng

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}