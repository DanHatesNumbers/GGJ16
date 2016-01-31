using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using AssemblyCSharp;
using System;

public class PlayerMovement : NetworkBehaviour {

	public GameObject Player;
    public GameObject Fireball;
    public AudioClip JumpSoundClip;
    public AudioClip PickupSoundClip;
    public AudioClip JoinSound;
    public AudioClip ShootSound;

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

    private AnimationStateEnum playerAnimation;

	// Use this for initialization
	void Start () {
		InputActions = new List<InputAction> ();
		var inputJump = new InputAction 
		{
			IsTriggered = () => Input.GetButtonDown("Jump") && Player.GetComponent<Collider2D>().IsTouchingLayers(),
			PlayerAction = InputAction.JumpAction,
            AudioClip = JumpSoundClip
		};

		var inputMoveLeft = new InputAction 
		{
			IsTriggered = () => Input.GetAxis("Horizontal") < leftThreshold,
			PlayerAction = InputAction.MoveLeftAction
		};

		var inputMoveRight = new InputAction 
		{
			IsTriggered = () => Input.GetAxis("Horizontal") > rightThreshold,
			PlayerAction = InputAction.MoveRightAction
		};

        var inputFireball = new InputAction
        {
            IsTriggered = () => (Input.GetAxis("Fire1") != 0) && CanFire,
            PlayerAction = (p, d) => StartCoroutine("SpawnFireball"),
            AudioClip = ShootSound
        };

		InputActions.Add(inputJump);
		InputActions.Add(inputMoveLeft);
		InputActions.Add(inputMoveRight);
        InputActions.Add(inputFireball);
        TimeSinceLastFire = 0f;
        FacingLeft = false;
        CanFire = true;

        var networkId = GetComponent<NetworkIdentity>();
        GetComponentInChildren<Camera>().enabled = hasAuthority;
        Debug.Log(String.Format("Is Local Player? {0}", hasAuthority));

        GetComponent<AudioSource>().PlayOneShot(JoinSound);
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
                    if (action.AudioClip != null)
                    {
                        AudioSource source = GetComponent<AudioSource>();
                        source.PlayOneShot(action.AudioClip);
                    }
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
