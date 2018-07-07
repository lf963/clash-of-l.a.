using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : Singleton<LevelManager> {
	
	//Manager for background music and attack sounds
	private MusicManager musicManager;

	/// <summary>
	/// A prefab for creating a single tile
	/// </summary>

	//public GameObject []ThumbButton;

	//Button on Level Map
	public Button []levelButtonOnMap;

	//Monster Buttons
	public Button []mosterBtn;

	[SerializeField]
	private Button cheerleaderBtn;

	[SerializeField]
	private Button marchingBandBtn;

	//the amount of money on screen	
	public Text moneyText;
	[SerializeField]
	private GameObject[] tilePrefabs;

	[SerializeField]
	private CameraMovement cameraMovement;

	//static private string FilePath;	//save file to this path

	private SaveLoad saveLoad;

	//myPlayer includes user's game progress
	//please refer to /Assets/Scripts/UserInfo.cs
	//initialize game progress with the following parameter
	//[0]: student
	//[1]: golf
	//[2]: tennis
	//[3]: basketball
	//[4]: volleyball
	//[5]: football
	static public UserInfo myPlayer = new UserInfo(0,100,100,100,100,100,
												   new bool[]{true,false,false,false,false,false},
												   new bool[]{true,false,false,false,false,false,false,false,false,false});
	
	//Load game before our scene is loaded
	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	static void OnBeforeSceneLoadRuntimeMethod(){
		//SaveLoad.FilePath: where we save our game
		//if running game on Unity editor
		if(Application.isEditor)
			SaveLoad.FilePath = System.IO.Path.Combine(Application.dataPath, "Resources/save.txt");
		else
			SaveLoad.FilePath = System.IO.Path.Combine(Application.persistentDataPath, "save.txt");

		//If the user has already saved his game progress, we overwrite myPlayer with his game progress
		if(File.Exists(SaveLoad.FilePath))
			SaveLoad.Load ();
	}

	/// <summary>
	/// The maps transform, this is needed for adding new tiles
	/// </summary>
	[SerializeField]
	private Transform map;

	/// <summary>
	/// Spawn points for the portals
	/// </summary>
	private Point blueSpawn, sparkSpawn;

	[SerializeField]
	private GameObject bluePortalPrefab;

	[SerializeField]
	private GameObject sparkPortalPrefab;

	[SerializeField]
	private Enemy invisibleRabbitPrefab;

	[SerializeField]
	private Monster invisibleUSCRabbitPrefab;

	private Point mapSize;

	// chap 7
	public Portal BluePortal {
		get;
		set;
	}

	public Portal SparkPortal {
		get;
		set;
	}

	public Enemy InvisibleRabbit { get; set; }

	public Monster InvisibleUSCRabbit { get; set; }

	/// <summary>
	/// The full path from start to goal
	/// </summary>
	private Stack<Node> fullPath;

	private Stack<Node> enemyFullPath;

	/// <summary>
	/// A dictionary that contains all tiles in our game
	/// </summary>
	/// <value>The tiles.</value>
	public Dictionary<Point,TileScript> Tiles {
		get;
		set;
	}

	/// <summary>
	/// A property for returning size of a tile
	/// </summary>
	public float TileSize {
		get {
			return tilePrefabs[0].GetComponent<SpriteRenderer> ().sprite.bounds.size.x;
		}
	}

	private Stack<Node> path;

	/// <summary>
	/// Property for accessing the path
	/// </summary>
	public Stack<Node> Path
	{
		get
		{
			if (fullPath == null)
			{
				GeneratePath();
			}

			return new Stack<Node>(new Stack<Node>(fullPath));
		}
	}

	/// <summary>
	/// Property for accessing the enemy path
	/// </summary>
	public Stack<Node> EnemyPath
	{
		get
		{
			if (enemyFullPath == null)
			{
				GenerateEnemyPath();
			}

			return new Stack<Node>(new Stack<Node>(enemyFullPath));
		}
	}

	public Point BlueSpawn
	{
		get
		{
			return blueSpawn;
		}
	}

	public Point RedSpawn
	{
		get
		{
			return sparkSpawn;
		}
	}

	// Use this for initialization
	void Start () {
		Time.timeScale = 1.0F;


		PlayBgMusic ();
		//When we just start the game, none of the buttons is clicked so it is empty
		//BackToMap: this button is placed in GameScene
		//If we click BackToMap, we return from GameScene to LevelMap Scene
		if (!String.IsNullOrEmpty (btnLevel) && btnLevel != "BackToMap") {
			CreateLevel (btnLevel);
			ShowMonsterButton();

		}

		else {
			//set level button according to whether it is unlocked or not
			ShowLevelButton ();
		}


	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void ShowMonsterButton(){
		for (int i=0; i < mosterBtn.Length; i++) {
			if(myPlayer.players[i] == false)
				mosterBtn[i].interactable = false;
			else
				mosterBtn[i].interactable = true;
		}
	}


	//set level button according to whether it is unlocked or not
	void ShowLevelButton(){
		for (int i = 0; i < myPlayer.level_Unlock.Length; i++) {
			if (myPlayer.level_Unlock [i])
				levelButtonOnMap [i].interactable = true;
			else
				levelButtonOnMap [i].interactable = false;
		}
	}



	void PlayBgMusic() {
		musicManager = FindObjectOfType<MusicManager> ();
		musicManager.bgMusic.Play ();
	}

	private void CreateLevel(string btnLevel) {
		Tiles = new Dictionary<Point, TileScript> ();

		// A tmp instantioation of the tile map, we will use a text document to load this later
		string[] mapData = ReadLevelText(btnLevel);

		mapSize = new Point (mapData [0].ToCharArray ().Length, mapData.Length);

		// Calculates the x map size
		int mapX = mapData [0].ToCharArray ().Length;

		// Calculates the y map size
		int mapY = mapData.Length;

		Vector3 maxTile = Vector3.zero;

		// Calculates the world start point, this is the top left corner of the screen
		Vector3 worldStart = Camera.main.ScreenToWorldPoint (new Vector3 (0, Screen.height));

		for (int y = 0; y < mapY; y++) { // The y positons
			// Gets all the tiles, that we need to place on the screen
			char[] newTiles = mapData[y].ToCharArray();

			for (int x = 0; x < mapX; x++) { // The x positions
				// Places the tile in the world
				PlaceTile(newTiles[x].ToString(), x, y, worldStart);
			}
		}

		maxTile = Tiles [new Point (mapX - 1, mapY - 1)].transform.position;

		cameraMovement.SetLimits (new Vector3(maxTile.x + TileSize, maxTile.y - TileSize));

		SpawnPortals ();
	}

	/// <summary>
	/// Place a tile in the gameworld
	/// </summary>
	/// <param name="tileType">The type of tile to place for example 0</param>
	/// <param name="x">The x coordinate of the tile</param>
	/// <param name="y">The y coordinate of the tile</param>
	/// <param name="worldStart">The world start position</param>
	private void PlaceTile(string tileType, int x, int y, Vector3 worldStart) {
		// Parses the tiletype to an int, so that we can use it as an indexer when we create a new tile
		int tileIndex = int.Parse (tileType);

		// Creates a new tile and makes a reference to that tile in the newTile variable
		TileScript newTile = Instantiate (tilePrefabs[tileIndex]).GetComponent<TileScript>();

		// Uses the new tile variable to change the position of the tile
		newTile.Setup (new Point (x, y), new Vector3 (worldStart.x + (TileSize * x), worldStart.y - (TileSize * y), 0), map, tileIndex);

		//***Test: if tile is grass, then we cannot walk***
		//tileIndex = 0: grass
		//tileIndex = 1: grass
		//tileIndex = 3: sand
		//tileIndex = 4: stone
		//tileIndex = 5: water
		//tileIndex = 6: stone2
		if (tileIndex == 0 || tileIndex == 1 || tileIndex == 2 || tileIndex == 4 || tileIndex == 5) {
			newTile.WalkAble = false;
		}
		if (tileIndex == 3 || tileIndex == 6 || tileIndex == 5) {
			newTile.isEmpty = false;
		}

	}

	private string[] ReadLevelText(string btnLevel) {
		//construct levelMap dictionary
		//btnLevel1 => Level1
		//btnLevel2 => Level2 ... and so on
		Dictionary<string,string> mapLevel = new Dictionary<string,string> ();
		for (int i = 1; i <= 10; i++) 
			mapLevel.Add ("btnLevel"+i.ToString(), "Level" + i.ToString());
		
		TextAsset bindData;
		//read different txt file according to the content of btnLevel
		bindData = Resources.Load (mapLevel[btnLevel]) as TextAsset;

		//Replace every NewLine with string.Empty
		string data = bindData.text.Replace (Environment.NewLine, string.Empty);
		return data.Split ('-');
	}

	private void SpawnPortals() {
		if (btnLevel == "btnLevel6") {
			blueSpawn = new Point (2, 9);
		} else if (btnLevel == "btnLevel9") {
			blueSpawn = new Point (1, 2);
		}
		else if (btnLevel == "btnLevel5") {
			blueSpawn = new Point (1, 10);
		}
		else if (btnLevel == "btnLevel8") {
			blueSpawn = new Point (1, 2);
		}
		else if (btnLevel == "btnLevel10") {
			blueSpawn = new Point (2, 2);
		}
		else {
			blueSpawn = new Point (1, 2);
		}

		GameObject tmp = (GameObject)Instantiate (bluePortalPrefab, Tiles [blueSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
		BluePortal = tmp.GetComponent<Portal>();
		BluePortal.name = "BluePortal";

		if (btnLevel == "btnLevel4") {
			sparkSpawn = new Point (23, 10);
		} 
		else if (btnLevel == "btnLevel5") {
			sparkSpawn = new Point (19, 10);
		} 
		else if (btnLevel == "btnLevel6") {
			sparkSpawn = new Point (18, 2);
		} 
		else if (btnLevel == "btnLevel7") {
			sparkSpawn = new Point (20, 2);
		}
		else if (btnLevel == "btnLevel8") {
			sparkSpawn = new Point (20, 2);
		}
		else if (btnLevel == "btnLevel9") {
			sparkSpawn = new Point (20, 10);
		}
		else if (btnLevel == "btnLevel10") {
			sparkSpawn = new Point (2, 8);
		}
		else {
			sparkSpawn = new Point (23, 4);
		}

		GameObject tmp2 = (GameObject)Instantiate (sparkPortalPrefab, Tiles [sparkSpawn].GetComponent<TileScript> ().WorldPosition, Quaternion.identity);
		SparkPortal = tmp2.GetComponent<Portal>();
		SparkPortal.name = "SparkPortal";

		InvisibleRabbit = Instantiate (invisibleRabbitPrefab, Tiles [sparkSpawn].GetComponent<TileScript> ().WorldPosition, Quaternion.identity);

		InvisibleUSCRabbit = Instantiate (invisibleUSCRabbitPrefab, Tiles [blueSpawn].GetComponent<TileScript> ().WorldPosition, Quaternion.identity);

		//Instantiate (sparkPortalPrefab, Tiles [sparkSpawn].GetComponent<TileScript>().WorldPosition, Quaternion.identity);
	}

	public bool InBounds(Point position) {
		return position.X >= 0 && position.Y >= 0 && position.X < mapSize.X && position.Y < mapSize.Y;
	}

	/// <summary>
	/// Generates a path with the AStar algorithm
	/// </summary>
	public void GeneratePath()
	{
		//Generates a path from start to finish and stores it in fullPath
		fullPath = AStar.GetPath(BlueSpawn, sparkSpawn);
	}

	/// <summary>
	/// Generates a path for enemy movement
	/// </summary>
	public void GenerateEnemyPath()
	{
		//Generates a path from start to finish and stores it in fullPath
		enemyFullPath = AStar.GetPath(sparkSpawn, BlueSpawn);
	}

	//Pause game
	//timeScale = 1.0: time is passing as fast as realtime.
	//timeScale = 0.5: time is passing 2x slower than realtime.
	//timeScale = 0.0: the game is bascially paused if all your functions are frame rate independent.
	void PauseGame(){
		if (Time.timeScale == 1.0F) {
			Time.timeScale = 0.0F;
			musicManager.bgMusic.Pause ();
			foreach (Button btn in mosterBtn) {
				btn.interactable = false;
				cheerleaderBtn.interactable = false;
				marchingBandBtn.interactable = false;
				GameManager.Instance.killerBtn.interactable = false;
			}
		}
		else {
			Time.timeScale = 1.0F;
			musicManager.bgMusic.UnPause ();
			ShowMonsterButton ();
			cheerleaderBtn.interactable = true;
			marchingBandBtn.interactable = true;
			GameManager.Instance.killerBtn.interactable = true;
		}
			
	}

	//when quit application, execute this function
	void OnApplicationQuit()
	{
		Debug.Log("Application ending after " + Time.time + " seconds");
		SaveLoad.Save();
	}
}
