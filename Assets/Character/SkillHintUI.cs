using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class SkillHintUI : MonoBehaviour
{
    public static SkillHintUI Instance;

    public GameObject panel;
    public TextMeshProUGUI hintText;
    public float showTime = 3f;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        panel.SetActive(false);
    }

    public void ShowHint(string text)
    {
        StopAllCoroutines();
        panel.SetActive(true);
        hintText.text = text;
        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(showTime);
        panel.SetActive(false);
    }
}