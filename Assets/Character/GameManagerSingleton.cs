using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerSingleton : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        // Ngăn việc sinh ra 2 GameManager khi load scene mới
        GameManagerSingleton[] objs = FindObjectsOfType<GameManagerSingleton>();
        if (objs.Length > 1)
            Destroy(gameObject);
    }
}
