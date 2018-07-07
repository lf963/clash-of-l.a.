using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {

	/// The units movement speed
	[SerializeField]
	private float speed;

	[SerializeField]
	private int damage;

	[SerializeField]
	private MonsterStat health;

	private bool canMove = true;
	private List<Monster> enemies = new List<Monster>();

	// ***Dennis
	[SerializeField]
	private Player player;

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

	/// The element type of the projectile
	public Element ElementType { get; protected set; }

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

	private SpriteRenderer spriteRenderer;

	private void Update()
	{
		// HandleDebuffs();
		if (canMove) {
			Move ();
		}

		// die
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

//	public void Spawn() {
//		transform.position = LevelManager.Instance.BluePortal.transform.position;
//
//		// Starts to scale the monsters
//		StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(2, 2),false));
//
//		//Sets the monsters path
//		SetPath(LevelManager.Instance.Path);
//	}

	public void SpawnEnemy(int health) {
		// Debug.Log ("Enemy ID: " + this.GetInstanceID());
		transform.position = LevelManager.Instance.SparkPortal.transform.position;

		this.health.MaxVal = health;
		this.health.CurrentVal = this.health.MaxVal;

		// Starts to scale the monsters
		StartCoroutine(Scale(new Vector3(0.1f, 0.1f), new Vector3(2, 2),false));

		//Sets the monsters path
		SetPath(LevelManager.Instance.EnemyPath);
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
		spriteRenderer = GetComponent<SpriteRenderer>();
		//		MaxSpeed = speed;
		//		health.Initialize();
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

	/// When the monster collides with something
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "BluePortal")//If we collide with the red portal
		{
			//Debug.Log ("Arrive BluePortal");
			//Start scaling the monster down
			//StartCoroutine(Scale(new Vector3(1, 1), new Vector3(0.1f, 0.1f),true));

			//Plays the portal animation
			//other.GetComponent<Portal>().Open();

			//GameManager.Instance.Lives--;

			// ****Dennis
			//player.Health_USC.CurrentVal -= 2;
			myAnimator.SetBool ("Attack", true);
			GameManager.Instance.DamagingUSC.Add(this);
			// Debug.Log ("Arrive BluePortal");
		}

		if (other.tag == "USCAthlete") {
			enemies.Add(other.GetComponent<Monster> ());
			// canMove = false;
			StartCoroutine(Attack (other.GetComponent<Monster>()));
		}

		//		if (other.tag == "Tile")
		//		{
		//			spriteRenderer.sortingOrder = other.GetComponent<TileScript>().GridPosition.Y;
		//		}
	}

	// Then the monster exit collisions
	private void OnTriggerExit2D(Collider2D other) {
		if (other.tag == "USCAthlete") {
			enemies.Remove(other.GetComponent<Monster> ());
			// canMove = true;
		}
	}

	private IEnumerator Attack(Monster target) {
		myAnimator.SetBool ("Attack", true);
		while (target.Health.CurrentVal != 0) {
			target.TakeDamage (damage);
			yield return new WaitForSeconds(0.5f);
		}
		//target.Release ();
		myAnimator.SetBool ("Attack", false);
		yield return null;
	}

	public void TakeDamage(int damage) {
		health.CurrentVal -= damage;
		// Debug.Log ("UCLA: Current Health: " + health.CurrentVal);
	}

	/// <summary>
	/// Releases a monster from the game into the object pool
	/// </summary>
	public void Release()
	{
		//Removes all debuffs
		//debuffs.Clear();
		// Debug.Log("Release Enemy");
		//Makes sure that it isn't active
		IsActive = false;

		//Deletes from Damaging Queue
		if (GameManager.Instance.DamagingUSC.Contains(this)) {
			GameManager.Instance.DamagingUSC.Remove(this);
		}

		//Makes sure that it has the correct start position
		GridPosition = LevelManager.Instance.RedSpawn;

		//Removes the monster from the game
		GameManager.Instance.RemoveEnemy(this);

		//Releases the object in the object pool
		GameManager.Instance.Pool.ReleaseObject(gameObject);
	}

	private void OnMouseDown() {
		if (GameManager.Instance.EnableTapAttack) {
			health.CurrentVal -= 50;
		}
	}
}
