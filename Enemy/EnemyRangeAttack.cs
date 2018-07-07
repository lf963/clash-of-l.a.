using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangeAttack : MonoBehaviour {

	/// This is the projectiles type
	[SerializeField]
	private string projectileType;

	/// The projectile's speed
	[SerializeField]
	private float projectileSpeed;

	/// The projectile's animator
	private Animator myAnimator;

	/// The damage that the projectile will deal
	[SerializeField]
	private int damage;

	/// <summary>
	/// The tower's current target
	/// </summary>
	private Monster target;

	[SerializeField]
	private Monster rabbit;

	private bool reachedPortal = false;

	/// <summary>
	/// A queue of monsters that the tower can attack
	/// </summary>
	private Queue<Monster> monsters = new Queue<Monster>();

	/// indicates, if the tower can attack
	private bool canAttack = true;

	/// <summary>
	/// Attack timer, for checking if we can attack or not
	/// </summary>
	private float attackTimer;

	/// <summary>
	/// Cooldown for the attack
	/// </summary>
	[SerializeField]
	private float attackCooldown;

	/// <summary>
	/// The element type of the projectile
	/// </summary>
	public Element ElementType { get; protected set; }

	/// <summary>
	/// The projectile's price
	/// </summary>
	public int Price { get; set; }

	/// <summary>
	/// Property for accessing the projectile's speed
	/// </summary>
	public float ProjectileSpeed
	{
		get { return projectileSpeed; }
	}

	/// <summary>
	/// Property for accessing the projectile's target
	/// </summary>
	public Monster Target
	{
		get { return target; }
	}

	/// <summary>
	/// Property for accessing the projectile's damage
	/// </summary>
	public int Damage
	{
		get
		{
			return damage;
		}
	}

	void Start() {
		rabbit = LevelManager.Instance.InvisibleUSCRabbit;
		rabbit.IsActive = true;
		monsters.Clear ();
		target = null;
	}

	// Use this for initialization
	void Awake()
	{
		myAnimator = transform.parent.GetComponent<Animator>();
		// Level = 1;
	}

	// Update is called once per frame
	void Update()
	{
		Attack();
	}

	/// <summary>
	/// Makes the tower attack a target
	/// </summary>
	private void Attack()
	{
		if (!canAttack)//If we can't attack
		{
			//Count how much time has passed since last attack
			attackTimer += Time.deltaTime;

			//If the time passed is higher than the cooldown, then we need to reset
			//and be able to attack again
			if (attackTimer >= attackCooldown)
			{
				canAttack = true;
				attackTimer = 0;
			}
		}
		//If we don't have a target and we have more targets in range
		if ((target == null || target == rabbit) && monsters.Count > 0 && monsters.Peek().IsActive)
		{
			target = monsters.Dequeue();
		}
		if (target == null && reachedPortal) {
			target = rabbit;
		}
		if ((target != null && target.IsActive))//If we have a target that is active || reach portal
		{
			if (canAttack)//If we can attack then we shoot at the target
			{
				Shoot();

				myAnimator.SetTrigger("Attack");

				canAttack = false;
			}

		}
		// if (target != null && !target.Alive || target != null && !target.IsActive)
		if (target != null && !target.IsActive)
		{
			target = null;
		}


	}

	/// <summary>
	/// Makes the tower shoot
	/// </summary>
	private void Shoot()
	{
		//Gets a projectile from the object pool
		EnemyProjectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<EnemyProjectile>();

		//Makes sure that the projectile is instantiated on the towers position
		projectile.transform.position = transform.position;

		projectile.Initialize(this);
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "USCAthlete") //Adds new monsters to the queu when they enter the range
		{
			monsters.Enqueue(other.GetComponent<Monster>());
		}
		if (other.tag == "BluePortal") {
			reachedPortal = true;
		}
	}

	// public abstract Debuff GetDebuff();

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "USCAthlete")
		{
			target = null;
		}
		if (other.tag == "BluePortal") {
			reachedPortal = false;
			if (this.transform && this.transform.parent && this.transform.parent.GetComponent<Monster> ()) {
				this.transform.parent.GetComponent<Monster> ().CanMove = true;
			}
		}
	}
}
