using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SecretWays : MonoBehaviour
{

    Tilemap m_Renderer;

    private void Start()
    {
        m_Renderer = GetComponentInParent<Tilemap>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_Renderer.color = new Color(1f, 1f, 1f, 0.5f);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            m_Renderer.color = new Color(1f, 1f, 1f, 1f);
        }
    }

}
