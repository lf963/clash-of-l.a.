using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Element { BASKETBALL, VOLLEYBALL, FOOTBALL, NONE }

public class RangeAttack : MonoBehaviour {

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

//	[SerializeField]
//	private float debuffDuration;

//	[SerializeField]
//	private float proc;

	/// <summary>
	/// The tower's current target
	/// </summary>
	private Enemy target;

	[SerializeField]
	private Enemy rabbit;

	private bool reachedPortal = false;

	/// <summary>
	/// The monster's current level
	/// </summary>
	// public int Level { get; protected set; }

	/// <summary>
	/// A queue of monsters that the tower can attack
	/// </summary>
	private Queue<Enemy> monsters = new Queue<Enemy>();

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
	public Enemy Target
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

//	public float DebuffDuration
//	{
//		get
//		{
//			return debuffDuration;
//		}
//
//		set
//		{
//			this.debuffDuration = value;
//		}
//	}

//	public float Proc
//	{
//		get
//		{
//			return proc;
//		}
//
//		set
//		{
//			this.proc = value;
//		}
//	}

	void Start() {
		
//		Debug.Log ("----count---- " + monsters.Count);
//		Debug.Log ("---target--- " + target);
		rabbit = LevelManager.Instance.InvisibleRabbit;
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
		// Debug.Log ("-----" + this.GetInstanceID() + " " + target.GetInstanceID() + "-----");
		//Gets a projectile from the object pool
		Projectile projectile = GameManager.Instance.Pool.GetObject(projectileType).GetComponent<Projectile>();

		//Makes sure that the projectile is instantiated on the towers position
		projectile.transform.position = transform.position;

		projectile.Initialize(this);
	}

	public void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "UCLAAthlete") //Adds new monsters to the queu when they enter the range
		{
			monsters.Enqueue(other.GetComponent<Enemy>());
		}
		if (other.tag == "RedPortal") {
			reachedPortal = true;
		}
	}

	// public abstract Debuff GetDebuff();

	public void OnTriggerExit2D(Collider2D other)
	{
		if (other.tag == "UCLAAthlete")
		{
			target = null;
		}
		if (other.tag == "RedPortal") {
			reachedPortal = false;
			if (this.transform && this.transform.parent && this.transform.parent.GetComponent<Monster> ()) {
				this.transform.parent.GetComponent<Monster> ().CanMove = true;
			}
		}
	}
}
