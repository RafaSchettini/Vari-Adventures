using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Spikes : MonoBehaviour
{

    //public ParticleSystem dieEffect;
    
    public string sceneToLoad;
    
    private PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }

    IEnumerator RespawnWaitTime()
    {
        Destroy(GameObject.FindGameObjectWithTag("Player"));
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(sceneToLoad);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Player"))
        {
            StartCoroutine(RespawnWaitTime());
        }
    }

    /*public void Die()
    {
        dieEffect.Play();
        StartCoroutine(RespawnWaitTime());
    }
    */
}
