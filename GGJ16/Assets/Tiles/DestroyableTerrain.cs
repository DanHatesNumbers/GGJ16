using UnityEngine;
using System.Collections;

public class DestroyableTerrain : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnCollisionEnter2D(Collision2D Col)
    {
        if ((Col.gameObject.name == "LavaBottom") || (Col.gameObject.name == "KillZ"))
        {
            Destroy(this.gameObject);
        }
        if (Col.gameObject.name == "FireBall")
        {
            if (this.GetComponent<Rigidbody2D>() == null)
            {
                this.gameObject.AddComponent<Rigidbody2D>();
                
            }
            Destroy(Col.gameObject);
        }
    }
}
