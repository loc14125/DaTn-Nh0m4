using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopUp : MonoBehaviour
{
    public float moveSpeed = 1f;
    public float fadeSpeed = 1f;
    private TextMeshPro textMesh;
    private Color textColor;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
        textColor = textMesh.color;
    }

    public void Setup(int damage)
    {
        textMesh.text = damage.ToString();
    }

    void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;
        textColor.a -= fadeSpeed * Time.deltaTime;
        textMesh.color = textColor;

        if (textColor.a <= 0)
            Destroy(gameObject);
    }
}
