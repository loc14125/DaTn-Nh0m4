using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterController : MonoBehaviour
{
    private int pressCount = 0;
    private float originalScale;
    public GameObject character; 


    void Start()
    {
        originalScale = character.transform.localScale.x;
    }

    void Update()
    {
      /*  if (Input.GetKeyDown(KeyCode.J))
        {
            pressCount++;

            if (pressCount == 3)
            {
                character.transform.localScale = new Vector3(originalScale * 2, originalScale * 2, originalScale * 2);
            }
        }*/
    }
}
