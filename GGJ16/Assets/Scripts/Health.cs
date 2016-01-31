using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public AudioClip hitSound;
    public AudioClip deathSound; 

    public const int maxHealth = 100;

    [SyncVar]
    public int health = maxHealth;

    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        health -= amount;
        GetComponent<AudioSource>().PlayOneShot(hitSound);

        if (health <= 0)
        {
            AudioSource audioSource = GetComponent<AudioSource>();
            audioSource.PlayOneShot(deathSound);

            Network.Disconnect();
            Invoke("Despawn", 1);
        }
    }

    void Despawn()
    {
        Destroy(this.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Hit detected");
        Debug.Log(collision.gameObject.name);
        if (collision.gameObject.name == "LavaBottom")
        {
            TakeDamage(maxHealth);
        }
    }
}
