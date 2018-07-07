using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour {

	/// The units movement speed
	[SerializeField]
	private float speed;

	[SerializeField]
	private int damage;

	[SerializeField]
	private MonsterStat health;

	/// The projectile's speed
	[SerializeField]
	private float projectileSpeed;

	private bool canMove = true;
	private List<Enemy> enemies = new List<Enemy>();

	private bool buffered = false;

	public bool Buffered {
		get { return buffered; }
		set { this.buffered = value; }
	}

	public float Speed
	{
		get
		{
			return speed;
		}

		set
		{
			this.speed = value;
		}
	}

	public int Damage {
		get { return damage; }
	}

	public bool CanMove {
		set { this.canMove = value; }
	}

	/// The element type of the projectile
	public Element ElementType { get; protected set; }

	/// Property for accessing the projectile's speed
	public float ProjectileSpeed
	{
		get { return projectileSpeed; }
	}

	public MonsterStat Health {
		get { return health; }
	}

	/// This stack contains the path that the Unit can walk on
	/// This path should be generated with the AStar algorithm
	private Stack<Node> path;

	/// The Unit's grid position
	public Point GridPosition { get; set; }

	/// Indicates if the Unit is active
	public bool IsActive { get; set; }

	/// A reference to the Unit's animator
	protected Animator myAnimator;

	/// The unit's next destination
	private Vector3 destination;

	// private bool isAttacking = false;

	// private SpriteRenderer spriteRenderer;

	void Start () {
		// spriteRenderer = GetComponent<SpriteRenderer>();
	}

	private void Update()
	{
		// HandleDebuffs();
		if (canMove) {
			Move ();
		}

		if (health.CurrentVal == 0) {
			Release ();
			// Destroy(this);
		}

		if (enemies.Count == 0) {
			canMove = true;
		}

		if (enemies.Count != 0) {
			canMove = false;
		}

	}

	public void Spawn(int health) {
		transform.position = LevelManager.Instance.BluePortal.transform.position;

		this.health.MaxVal = health;
		this.health.CurrentVal = this.health.MaxVal;

		// Starts to scale the monsters
		StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(2, 2),false));

		//Sets the monsters path
		SetPath(LevelManager.Instance.Path);
	}

	public IEnumerator Scale(Vector3 from, Vector3 to, bool remove) {
		//Stephanie: Used in creating and removing the mosters
		//The scaling progress
		float progress = 0;

		//As long as the progress is les than 1, then we need to keep scaling
		while (progress <= 1)
		{
			//Scales themonster
			transform.localScale = Vector3.Lerp(from, to, progress);
			progress += Time.deltaTime;
			yield return null;
		}

		//Makes sure that is has the correct scale after scaling
		transform.localScale = to;

		IsActive = true;

//		if (remove)
//		{
//			Release();
//		}
	}

	private void Awake()
	{
		//Sets up references to the components
		myAnimator = GetComponent<Animator>();
		// spriteRenderer = GetComponent<SpriteRenderer>();
		//		MaxSpeed = speed;
		health.Initialize();
	}

	/// Makes the unity move along the given path
	public void Move() {
		if (IsActive)
		{
			//Move the unit towards the next destination
			transform.position = Vector2.MoveTowards(transform.position, destination, Speed * Time.deltaTime);

			//Checks if we arrived at the destination
			if (transform.position == destination)
			{
				//If we have a path and we have more nodes, then we need to keep moving
				if (path != null && path.Count > 0)
				{
					//Animates the Unit based on the gridposition
					Animate(GridPosition, path.Peek().GridPosition);

					//Sets the new gridPosition
					GridPosition = path.Peek().GridPosition;

					//Sets a new destination
					destination = path.Pop().WorldPosition;

				}
			}
		}

	}

	/// Gives the Unit a path to walk on
	/// <param name="newPath">The unit's new path</param>
	/// <param name="active">Indicates if the unit is active</param>
	public void SetPath(Stack<Node> newPath)
	{	
		if (newPath != null) //If we have a path
		{
			//Sets the new path as the current path
			this.path = newPath;

			//Animates the Unit based on the gridposition
			Animate(GridPosition, path.Peek().GridPosition);

			//Sets the new gridPosition
			GridPosition = path.Peek().GridPosition;

			//Sets a new destination
			destination = path.Pop().WorldPosition;
		}
	}

	/// Animates the Unit according to the current action
	public void Animate(Point currentPos, Point newPos)
	{
		//The code below animates the unit based on the moving direction
		//Debug.Log(currentPos.X + " " + currentPos.Y);
		if (currentPos.Y == newPos.Y) {
			//If we are moving left
			if (currentPos.X > newPos.X) {
				//myAnimator.SetInteger("Vertical", 0);
				// Debug.Log("set to -1");
				myAnimator.SetInteger ("Horizontal", -1);
			}
			//If we are moving right
			else if (currentPos.X < newPos.X) {
				//myAnimator.SetInteger("Vertical", 0);
				// Debug.Log("set to 1");
				myAnimator.SetInteger ("Horizontal", 1);
			}
		}
	}

	private void OnTriggerStay2D(Collider2D other)
	{
		if (other.tag == "RedPortal")//If we collide with the red portal
		{
			myAnimator.SetBool ("Attack", true);
		}
	}

	/// When the monster collides with something
	private void OnTriggerEnter2D(Collider2D other)
	{
		
		if (other.tag == "RedPortal") {//If we collide with the red portal

			GameManager.Instance.DamagingUCLA.Add (this);

			Debug.Log ("Arrive RedPortal");
		}

		if (other.tag == "UCLAAthlete") {
			enemies.Add (other.GetComponent<Enemy> ());
			// canMove = false;
			if (projectileSpeed == -1) {
				StartCoroutine (Attack (other.GetComponent<Enemy> ()));
			}
		}
	}

	// Then the monster exit collisions
	private void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "RedPortal") {
			//Deletes from Damaging Queue
			if (GameManager.Instance.DamagingUCLA.Contains(this)) {
				GameManager.Instance.DamagingUCLA.Remove(this);
			}
		}
		if (other.tag == "UCLAAthlete") {
			enemies.Remove (other.GetComponent<Enemy> ());
			myAnimator.SetBool ("Attack", false);
			// canMove = true;
		}
	}

	private IEnumerator Attack(Enemy target) {
		myAnimator.SetBool ("Attack", true);
		while (target.Health.CurrentVal != 0) {
			target.TakeDamage (damage);
			yield return new WaitForSeconds(0.5f);
		}
		myAnimator.SetBool ("Attack", false);
		yield return null;
	}

	public void TakeDamage(int damage) {
		health.CurrentVal -= damage;
		// Debug.Log ("USC: Current Health: " + health.CurrentVal);
	}

	/// <summary>
	/// Releases a monster from the game into the object pool
	/// </summary>
	public void Release()
	{
		//Removes all debuffs
		//debuffs.Clear();

		//Makes sure that it isn't active
		IsActive = false;

		//Deletes from Damaging Queue
		if (GameManager.Instance.DamagingUCLA.Contains(this)) {
			GameManager.Instance.DamagingUCLA.Remove(this);
		}

		//Makes sure that it has the correct start position
		GridPosition = LevelManager.Instance.BlueSpawn;

		//Removes the monster from the game
		GameManager.Instance.RemoveMonster(this);

		//Releases the object in the object pool
		GameManager.Instance.Pool.ReleaseObject(gameObject);
	}

}
