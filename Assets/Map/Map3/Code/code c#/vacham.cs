using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class vacham : MonoBehaviour
{
    public int vp = 0;
    public int hp = 5;
    public TextMeshProUGUI VPText;
    void Start()
    {
        VPText.SetText(vp.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("vp")) 
        {
            vp++;
            VPText.SetText(vp.ToString());
            Destroy(collision.gameObject);
        }
    }
}
