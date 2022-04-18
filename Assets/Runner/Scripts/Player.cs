using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
	public AudioClip JumpS;
	public AudioClip FallS;
	public AudioClip CoinS;
	public AudioClip CompleteS;
	AudioSource audioSource;
	const float jumpCheckPreventionTime = 0.5f;
	public delegate void CollectCoinCallback();
	public CollectCoinCallback onCollectCoin;
	public GameObject player;
	[Header("Physic Setting")]
	public LayerMask groundLayerMask;
	[Header("Move & Jump Setting")]
	public float moveSpeed = 10;
	public float fallWeight = 5.0f;
	public float jumpWeight = 0.5f;
	public float jumpVelocity = 100.0f;
	protected bool jumping = false;			
	protected Vector3 moveVec = Vector3.zero; 
	protected float jumpTimestamp;		
	protected Animator animator;				
	protected Rigidbody rigidbody;         
	[SerializeField] GameController gamecontroller;
	private void Awake()
	{
		this.audioSource = GetComponent<AudioSource>();
		animator = GetComponentInChildren<Animator>();
		rigidbody = GetComponent<Rigidbody>();
		
	}

	void PlaySound(string action)
	{
		switch (action)
		{
			case "JUMP":
				audioSource.clip = JumpS;
				break;
			case "COIN":
				audioSource.clip = CoinS;
				break;
			case "FALL":
				audioSource.clip = FallS;
				break;
			case "COMPLETE":
				audioSource.clip = CompleteS;
				break;
		}
		audioSource.Play();
	}

	void UpdateWhenJumping()
	{
		bool isFalling = rigidbody.velocity.y <= 0;
		float weight = isFalling ? fallWeight : jumpWeight;
		rigidbody.velocity = new Vector3(moveVec.x * moveSpeed, rigidbody.velocity.y, moveVec.z * moveSpeed);
		rigidbody.velocity += Vector3.up * Physics.gravity.y * weight * Time.deltaTime;
		GroundCheck();
	}

	void UpdateWhenGrounded()
	{
		rigidbody.velocity = moveVec * moveSpeed;
		if (moveVec != Vector3.zero)
		{
			transform.LookAt(this.transform.position + moveVec.normalized);
		}
		CheckShouldFall();
	}

	private void FixedUpdate()
	{
		if (jumping == false)
		{
			UpdateWhenGrounded();
		}
		else
		{
			UpdateWhenJumping();
		}
	}

	void Update()
	{
		UpdateAnimation();
	}

	public void OnJump()
    {
		HandleJump();
    }

	public void OnMove(InputValue input)
    {
		Vector2 inputVec = input.Get<Vector2>();

		moveVec = new Vector3(inputVec.x, 0, inputVec.y);
    }

	protected bool HandleJump()
	{
		if (jumping)
		{
			return false;
		}
		PlaySound("JUMP");
		jumping = true;
		jumpTimestamp = Time.time;
		rigidbody.velocity = new Vector3(0, jumpVelocity, 0);

		return true;
	}

	void CheckShouldFall()
	{
		if(jumping)
		{
			return;	
		}

		bool hasHit = Physics.CheckSphere(transform.position, 0.1f, groundLayerMask);

		if (hasHit == false)
		{
			jumping = true;
		}
	}
	void GroundCheck()
	{
		if(jumping == false)
		{
			return;
		}

		if (Time.time < jumpTimestamp + jumpCheckPreventionTime)
		{
			return;
		}

		bool hasHit = Physics.CheckSphere(transform.position, 0.1f, groundLayerMask);
		
		if(hasHit)
		{
			jumping = false;
		}
	}
	void UpdateAnimation()
	{
		if (animator == null)
		{
			return;
		}

		animator.SetBool("jumping", jumping);
		animator.SetFloat("moveSpeed", moveVec.magnitude);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Coin")
		{
			HandleCoinCollect(other);
			PlaySound("COIN");
		}
		if (other.transform.tag == "DeathPlane")
		{
			PlaySound("FALL");
			player.transform.position = new Vector3(-14, 11, -21);
		}
		if (other.transform.tag == "Saw")
		{
			PlaySound("FALL");
			gamecontroller.EndGame();
		}
	}
	private void OnCollisionEnter(Collision other)
	{
		
		
	}
	void HandleCoinCollect(Collider collision)
	{
		Coin coin = collision.transform.GetComponent<Coin>();
		if(coin == null)
		{
			return;
		}
		coin.Collect();

		if(onCollectCoin != null)
		{
			onCollectCoin();
		}
	}
}
