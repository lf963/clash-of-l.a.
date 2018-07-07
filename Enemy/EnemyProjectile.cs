using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour {
	/// <summary>
	/// The projectile's target
	/// </summary>
	private Monster target;

	/// <summary>
	/// The projectile's tower
	/// </summary>
	private EnemyRangeAttack parent;

	/// <summary>
	/// The projectile's animator
	/// </summary>
//	private Animator myAnimator;


	/// The element type of the projectile
	private Element elementType;

	// Use this for initialization
	void Start ()
	{
		//Creates a reference to the animator
		//myAnimator = GetComponent<Animator>();
	}

	// Update is called once per frame
	void Update ()
	{
		MoveToTarget();
	}

	public void Initialize(EnemyRangeAttack parent)
	{
		this.target = parent.Target;
		this.parent = parent;
		this.elementType = parent.ElementType;
	}

	private void MoveToTarget()
	{
		if (target != null && target.IsActive) //If the target isn't null and the target isn't dead
		{
			//Move towards the position
			Vector3 targetPosition = new Vector3(target.transform.position.x, target.transform.position.y + 1, target.transform.position.z);
			//Debug.Log(target.transform.position);
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * parent.ProjectileSpeed);

			//Calculates the direction of the projectile
			Vector2 dir = target.transform.position - transform.position;

			//Calculates the angle of the projectile
			float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

			//Sets the rotation based on the angle
			transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		}
		else if (!target.IsActive)
		{
			GameManager.Instance.Pool.ReleaseObject(gameObject);
		}
		if (transform.position == target.transform.position) {
			GameManager.Instance.Pool.ReleaseObject(gameObject);
		}
	}

	/// <summary>
	/// Tries to apply a debuff to the target
	/// </summary>
	private void ApplyDebuff()
	{
		//Checks if the target is immune to the debuff
		if (target.ElementType != elementType)
		{
			//Does a roll to check if we have to apply a debuff
			float roll = Random.Range(0, 100);

//			if (roll <= parent.Proc)
//			{
//				//applies the debuff
//				// target.AddDebuff(parent.GetDebuff());
//			}
		}


	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		//If we hit a monster
		if (other.tag == "USCAthlete")
		{
			if (target.gameObject == other.gameObject)
			{
				//Makes the monster take damage based on the tower stats
				// target.TakeDamage(parent.Damage, elementType);
				target.TakeDamage(parent.Damage);

				//Triggers the impact animation
				// myAnimator.SetTrigger("Impact");

				//Tries to apply a debuff
				ApplyDebuff();

				GameManager.Instance.Pool.ReleaseObject(gameObject);

			}
		}

	}
}
