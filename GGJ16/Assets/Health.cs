using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class Health : NetworkBehaviour {

    public const int maxHealth = 100;

    [SyncVar]
    public int health = maxHealth;

    public void TakeDamage(int amount)
    {
        if (!isServer) return;

        health -= amount;

        if (health <= 0)
        {
            NetworkServer.Destroy(this.gameObject);
            Destroy(this.gameObject);
            Network.Disconnect();
        }
    }
}
