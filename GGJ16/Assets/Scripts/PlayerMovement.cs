using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class PlayerMovement : NetworkBehaviour {

	public GameObject Player;
    public GameObject Fireball;

    private float TimeSinceLastFire;
    public const float FireballCooldown = 0.5f;

    public bool FacingLeft;
    public bool CanFire;

	public List<InputAction> InputActions;

	public const float leftThreshold = -0.1f;
	public const float rightThreshold = 0.1f;

    public const string IdleLeftAnimation = "IdleLeft";
    public const string IdleRightAnimation = "IdleRight";
    public const string RunLeftAnimation = "RunLeft";
    public const string RunRightAnimation = "RunRight";
    public const string JumpLeftAnimation = "JumpLeft";
    public const string JumpRightAnimation = "JumpRight";

    private const float PowerupDuration = 10f;

    private Dictionary<string, float> PowerupTimers;

    private AnimationStateEnum playerAnimation;

	// Use this for initialization
	void Start () {
		InputActions = new List<InputAction> ();
		var inputJump = new InputAction 
		{
			IsTriggered = () => 
                    Input.GetButtonDown("Jump") 
                    && (IsPowerupActive(PowerupNames.SolidYellow) || Player.GetComponent<Collider2D>().IsTouchingLayers())
                    && !IsPowerupActive(PowerupNames.SolidBlack),
            PlayerAction = (c, delta) => 
            {
                var forceAmount = 13f;
                //Debug.Log(String.Format("Jump input action delta: {0}, total force: {1}", delta, forceAmount));
                var rb = c.GetComponent<Rigidbody2D>();
                rb.AddForce(new Vector2(0f, forceAmount), ForceMode2D.Impulse);
            }
		};

		var inputMoveLeft = new InputAction 
		{
			IsTriggered = () => Input.GetAxis("Horizontal") < leftThreshold,
            PlayerAction = (c, delta) => 
            {
                var forceAmount = -0.2f * delta * 100;
                Debug.Log(String.Format("Move left input action delta: {0}, total force: {1}", delta, forceAmount));
                var rb = c.GetComponent<Rigidbody2D>();
                rb.AddForce(new Vector2(forceAmount, 0f), ForceMode2D.Impulse);
                c.GetComponent<PlayerMovement>().FacingLeft = true;
            }
		};

		var inputMoveRight = new InputAction 
		{
			IsTriggered = () => Input.GetAxis("Horizontal") > rightThreshold,
            PlayerAction = (c, delta) => 
            {
                var forceAmount = 0.2f * delta * 100;
                Debug.Log(String.Format("Move right input action delta: {0}, total force: {1}", delta, forceAmount));
                var rb = c.GetComponent<Rigidbody2D>();
                rb.AddForce(new Vector2(forceAmount, 0f), ForceMode2D.Impulse);
                c.GetComponent<PlayerMovement>().FacingLeft = false;
            }
		};

        var inputFireball = new InputAction
        {
            IsTriggered = () => (Input.GetAxis("Fire1") != 0) && CanFire,
            PlayerAction = (p, d) => StartCoroutine("SpawnFireball")
        };

		InputActions.Add(inputJump);
		InputActions.Add(inputMoveLeft);
		InputActions.Add(inputMoveRight);
        InputActions.Add(inputFireball);
        TimeSinceLastFire = 0f;
        FacingLeft = false;
        CanFire = true;

        PowerupTimers = new Dictionary<string, float>();

        var networkId = GetComponent<NetworkIdentity>();
        GetComponentInChildren<Camera>().enabled = hasAuthority;
        Debug.Log(String.Format("Is Local Player? {0}", hasAuthority));
	}
	
	// Update is called once per frame
	void Update ()
	{
        var animator = GetComponentInChildren<Animator>();
        var rigidBody = GetComponent<Rigidbody2D>();

        if (hasAuthority)
        {
            TimeSinceLastFire += Time.deltaTime;
            foreach (var action in InputActions)
            {
                if (action.IsTriggered())
                {
                    action.PlayerAction(Player, Time.deltaTime);
                }
            }

            var keys = new List<string>(PowerupTimers.Keys);
            foreach(var key in keys)
            {
                PowerupTimers[key] -= Time.deltaTime;
                Debug.Log(String.Format("Powerup {0} remaining duration {1}", key, PowerupTimers[key]));
                if(PowerupTimers[key] <= 0f)
                {
                    PowerupTimers[key] = 0f;
                }
            }
        }
        var playerVelocity = this.GetComponent<Rigidbody2D>().velocity;
        if (playerVelocity.x > 0.5f)
        {
            if (playerVelocity.y > 0.5f)
            {
                animator.Play(JumpRightAnimation);
            }
            else
            {
                animator.Play(RunRightAnimation);
            }
            FacingLeft = false;
        }
        else if (playerVelocity.x > 0f)
        {
            animator.Play(IdleRightAnimation);
            FacingLeft = false;
        }

        if (playerVelocity.x < -0.5f)
        {
            if (playerVelocity.y > 0.5f)
            {
                animator.Play(JumpLeftAnimation);
            }
            else
            {
                animator.Play(RunLeftAnimation);
            }
            FacingLeft = true;
        }
        else if (playerVelocity.x < 0f)
        {
            animator.Play(IdleLeftAnimation);
            FacingLeft = true;
        }
	}

    [Client]
    void OnCollisionEnter2D(Collision2D col)
    {
        var collidedObjectName = col.gameObject.name;
        switch (collidedObjectName)
        {
            case PowerupNames.SolidBlack:
                CmdActivatePowerup(PowerupNames.SolidBlack);
                RemovePowerup(col.gameObject);
                break;
            case PowerupNames.SolidYellow:
                CmdActivatePowerup(PowerupNames.SolidYellow);
                RemovePowerup(col.gameObject);
                break;
        }
    }

    bool IsPowerupActive(string powerupName)
    {
        if(PowerupTimers.ContainsKey(powerupName))
        {
            return PowerupTimers[powerupName] > 0f;
        }
        return false;
    }

    [Command]
    void CmdActivatePowerup(string powerupName)
    {
        Debug.Log(String.Format("Activating powerup {0}", powerupName));
        PowerupTimers[powerupName] = PowerupDuration;
    }

    void RemovePowerup(GameObject obj)
    {
        Debug.Log(String.Format("Removing powerup object {0}", obj.name));
        NetworkServer.Destroy(obj);
        Destroy(obj);
    }

    IEnumerator SpawnFireball()
    {
        CanFire = false;

        CmdSpawnFireball();

        yield return new WaitForSeconds(FireballCooldown);

        CanFire = true;
    }

    [Command]
    void CmdSpawnFireball()
    {
        Vector2 position = Player.transform.position;
        if (FacingLeft)
        {
            position.x -= Player.GetComponent<Collider2D>().bounds.size.x;
        }
        else
        {
            position.x += Player.GetComponent<Collider2D>().bounds.size.x;
        }
        var fireball = (GameObject)Instantiate(Fireball, position, Quaternion.Euler(new Vector3(0, 0, 0)));
        var fireballVelocity = FacingLeft ? new Vector2(-750f, 0f) : new Vector2(750f, 0f);
        fireball.GetComponent<Rigidbody2D>().AddForce(fireballVelocity, ForceMode2D.Impulse);

        NetworkServer.Spawn(fireball);
    }
}
