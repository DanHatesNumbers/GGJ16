using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System;

public class FireballBehaviour : MonoBehaviour {
    // Use this for initialization
    public float FireballForceScale;
    public float FireballTimeout;
    private float SpawnTime;
	void Start () {
        SpawnTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        if(Time.time > SpawnTime + FireballTimeout)
        {
            Destroy(this.gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        Debug.Log(String.Format("Collided with {0}", col.gameObject.name));
        if(col.gameObject.name == "Player" && !col.gameObject.GetComponent<PlayerMovement>().hasAuthority)
        {
            var dir = this.gameObject.transform.position - col.gameObject.transform.position;
            col.gameObject.GetComponent<Rigidbody2D>().AddForce(dir * FireballForceScale, ForceMode2D.Impulse);
            Destroy(this.gameObject);
        }
    }
}
