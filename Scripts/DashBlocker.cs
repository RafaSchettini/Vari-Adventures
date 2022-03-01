using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashBlocker : MonoBehaviour
{

    public PlayerController playerController;
    
    // Start is called before the first frame update
    void Start()
    {
        GameObject playerC = GameObject.FindGameObjectWithTag("Player");

        playerController = playerC.GetComponent<PlayerController>();

        playerController.canDash = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) 
        {
            playerController.canDash = true;
        }
    }

}
