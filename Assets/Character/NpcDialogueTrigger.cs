using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NpcDialogueTrigger : MonoBehaviour
{
    public DialogueUI dialogueUI;

    private bool hasTalked = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasTalked)
        {
            hasTalked = true;
            dialogueUI.StartDialogue();
        }
    }
}