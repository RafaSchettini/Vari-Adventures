using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionTrigger : MonoBehaviour
{
    public Animator anim;

    public SpriteRenderer button;

    private bool hasPlayer;

    private void Start()
    {
        button.enabled = false;
        
    }

    private void Update()
    {
        if (hasPlayer && Input.GetKeyDown("e"))
        {
            anim.SetBool("isOpen", true);
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
            anim.SetBool("isOpen", false);
            button.enabled = false;
            hasPlayer = false;
        }
    }

}
