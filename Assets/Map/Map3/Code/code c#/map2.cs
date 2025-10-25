using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class map2 : MonoBehaviour
{
    public string nameScene = "map2";
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player") 
        { 
            collision.gameObject.SetActive(false);
        }
        SceneManager.LoadScene(nameScene);
    }
}
