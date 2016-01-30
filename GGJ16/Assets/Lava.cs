using UnityEngine;
using System.Collections;

public class Lava : MonoBehaviour {

    void OnCollisionEnter(Collision collision)
    {
        var hit = collision.gameObject;
        var hitCombat = hit.GetComponent<Health>();
        Debug.Log("Hit detected");
        if (hitCombat != null)
        {
            Debug.Log("Lava Hit");
            hitCombat.TakeDamage(Health.maxHealth);

            Destroy(gameObject);
        }
    }
}
