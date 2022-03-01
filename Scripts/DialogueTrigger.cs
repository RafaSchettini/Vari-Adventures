using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    public SpriteRenderer button;

    private bool hasPlayer;

    private void Start()
    {
        button.enabled = false;
    }

    public void TriggerDialogue()
    {
        FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
    }

    public void FinishDialogue()
    {
        FindObjectOfType<DialogueManager>().EndDialogue();
    }

    private void Update()
    {
        if (hasPlayer && Input.GetKeyDown("e"))
        {
            TriggerDialogue();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            button.enabled = true;
            hasPlayer = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            FinishDialogue();
            button.enabled = false;
            hasPlayer = false;
        }
    }

}
