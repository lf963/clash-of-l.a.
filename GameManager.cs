using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager> {

	//Manager for background music and attack sounds
	private MusicManager musicManager;

	public TowerBtn ClickedBtn {
		get;
		set;
	}
	[SerializeField]
	private Text timerTextInGame;

	[SerializeField]
	private Text timerTextInWinMenu;

	[SerializeField]
	private Text timerTextInGameoverMenu;

	private float startTime;	//game starting time
	private float finishedTime;
	private bool gameFinished = false;	//game finish or not

	// killer's cooldown
	private int cooldownStart = 0;
	private int cooldownTime = 20;
	private bool shouldUpdateCooldown = true;
	private bool startSpecialAttack = false;
	[SerializeField]
	private Sprite activeBtnSprite;
	[SerializeField]
	private Sprite inactiveBtnSprite;

	[SerializeField]
	private Text rewardTextInWinMenu;

	[SerializeField]
	private Text rewardTextInGameoverMenu;

	private int reward = 0;	//when game finish, how much you get

	[SerializeField]
	private Player player;

	private bool chargeCooled_Athlete = true;

	private bool chargeCooled_Tower = true;

	private bool startEnemyWave = true;
	private int enemyCount = 0;
	private int enemyThredhold = 5;

	private int currency;

	private int lives;

	//private int wave = 0;

	[SerializeField]
	private Button backHome;

	private bool gameOver = false;

	[SerializeField]
	private GameObject gameOverMenu;

	[SerializeField]
	private GameObject winMenu;

	[SerializeField]
	public Button killerBtn;

	private int health = 100;
	private int enemyHealth = 100;

	public List<Monster> DamagingUCLA = new List<Monster>();
	public List<Enemy> DamagingUSC = new List<Enemy>();

	private bool underattack_ucla = false;
	private bool underattack_usc = false;

	private GameObject nexusUCLA;
	private GameObject nexusUSC;

	private Animator animatorUCLA;
	private Animator animatorUSC;

	[SerializeField]
	private Sprite deadUCLA;

	[SerializeField]
	private Sprite deadUSC;

	[SerializeField]
	private Text currencyTxt;

	[SerializeField]
	private Text livesTxt;

	[SerializeField]
	private GameObject statsPanel;

	[SerializeField]
	private Text statText;

	[SerializeField]
	private Font pixelFont;

	/// <summary>
	/// The current selected tower.
	/// </summary>
	private Tower selectedTower;

	private bool enableTapAttack = false;

	private List<Monster> activeMonsters = new List<Monster>();
	private List<Enemy> activeEnemies = new List<Enemy>();

	private bool gameOverJumpBack = false;

	public bool EnableTapAttack { get; set; }

	public int Currency {
		get { return currency; }
		set { 
			this.currency = value;
			this.currencyTxt.text = value.ToString() + " <color=lime>$</color>";
		}
	}

	public Player Player {
		get;
		set;
	}

	public int Lives
	{
		get {
			return lives;
		}
		set {
			this.lives = value;
		}
	}

	public bool OnClickEvent { get; set; }

	public bool WaveActive
	{
		get {
			return activeMonsters.Count > 0;
		}
	}

	// chap 7
	public ObjectPool Pool {
		get;
		set;
	}
	// chap 7
	private void Awake() {
		Pool = GetComponent<ObjectPool> ();
	}

	// Use this for initialization
	void Start () {
		//set killer's cooldown
		killerBtn.interactable = false;

		//read from our player object
		Currency = LevelManager.myPlayer.money;
		startTime = Time.time;
		musicManager = FindObjectOfType<MusicManager> ();
		gameOverMenu.SetActive(false);
		winMenu.SetActive(false);
		nexusUSC = GameObject.Find ("BluePortal");
		nexusUCLA = GameObject.Find ("SparkPortal");
		animatorUSC = nexusUSC.GetComponent<Animator> ();
		animatorUCLA = nexusUCLA.GetComponent<Animator> ();


	}
	
	// Update is called once per frame
	void Update () {
		
		if (gameFinished) 
			return;
		
		float t = Time.time - startTime;
		string minutes = ((int)t / 60).ToString ();
		string seconds = (t % 60).ToString ("f0");	//truncate decimal point
		timerTextInGame.text =  minutes + " : " + seconds;
		HandleEscape ();

		if (chargeCooled_Athlete) {
			// StartCoroutine(ChargeEnergy_Athlete(3, 0.5f));
			StartCoroutine(ChargeEnergy_Athlete(3, 0.5f));
		}

		if (chargeCooled_Tower) {
			// StartCoroutine (ChargeEnergy_Tower(1, 1));
			StartCoroutine (ChargeEnergy_Tower(2, 0.5f));
		}

		if (shouldUpdateCooldown) {
			StartCoroutine(UpdateCooldown());
		}

		if (startEnemyWave && enemyCount <= 30) {
			StartCoroutine (EnemyWave());
		}

		//press Plus or Minus to increase or decrease money
		if (Input.GetKeyDown (KeyCode.KeypadPlus)) {
			LevelManager.myPlayer.money += 1;
			Currency += 1;
		}

		if (Input.GetKeyDown (KeyCode.KeypadMinus)) {
			LevelManager.myPlayer.money -= 1;
			Currency -= 1;
		}

		if (DamagingUCLA.Count != 0 && !underattack_ucla) {
			StartCoroutine (UCLATakeDamage ());
		}

		if (DamagingUSC.Count != 0 && !underattack_usc) {
			StartCoroutine (USCTakeDamage ());
		}
			
		// GameOver or Win: back to LevelMap
		if ((player.Health_USC.CurrentVal == 0 || player.Health_UCLA.CurrentVal == 0) && !gameOverJumpBack) {
			gameOverJumpBack = true;
			gameFinished = true;
			finishedTime = t;
			musicManager.VolumeLow (musicManager.bgMusic);
			//Pause game
			//timeScale = 1.0: time is passing as fast as realtime.
			//timeScale = 0.5: time is passing 2x slower than realtime.
			//timeScale = 0.0: the game is bascially paused if all your functions are frame rate independent.

			if (player.Health_USC.CurrentVal == 0) {	//UCLA win
				animatorUSC.SetBool ("IsDead", true);
				timerTextInGameoverMenu.text = timerTextInGame.text;
				rewardCalulation (false);
				gameOverMenu.SetActive (true);
			} else { 	//USC win
				animatorUCLA.SetBool ("IsDead", true);
				timerTextInWinMenu.text = timerTextInGame.text;
				rewardCalulation (true);
				unlockNextLevel (btnLevel);
				winMenu.SetActive (true);
			}
			//backHome.onClick.Invoke();
			Time.timeScale = 0.0F;
		}

		if (enableTapAttack) {
			TapAttack ();
		}

	}

	void unlockNextLevel(string btnLevel){
		//the nextLevel we have to unlock
		int nextLevel = Int32.Parse(btnLevel.Substring (8));

		if (nextLevel < 10)	//we only have 10 levels
			LevelManager.myPlayer.level_Unlock [nextLevel] = true;
	}

	void rewardCalulation(bool isWin){
		if (isWin) {
			reward = (int)(100 / finishedTime + player.Health_USC.CurrentVal);
			rewardTextInWinMenu.text = reward.ToString ();
		}
		else {
			reward = (int)(0.5*finishedTime);
			rewardTextInGameoverMenu.text = reward.ToString ();
		}
		LevelManager.myPlayer.money += reward;
		
	}

	public void PickTower(TowerBtn towerBtn) {
		if (player.Energy_Tower.CurrentVal >= towerBtn.Price) {
			// Store the clicked button
			this.ClickedBtn = towerBtn;
			// Activate the hover icon with the mouse
			Hover.Instance.Activate(towerBtn.Sprite);
			// Set OnclickEvent to true
			OnClickEvent = true;
		}
	}

	public void BuyTower() {
		if (player.Energy_Tower.CurrentVal >= ClickedBtn.Price) {
			player.Energy_Tower.CurrentVal -= ClickedBtn.Price;
			Hover.Instance.Deactivate ();
			OnClickEvent = false;
		}
	}

	public void SelectTower(Tower tower) {
		if (selectedTower != null) {
			selectedTower.Select ();
		}

		selectedTower = tower;
		selectedTower.Select ();
	}

	public void DeselectTower() {
		if (selectedTower != null) {
			selectedTower.Select ();
		}

		selectedTower = null;
	}

	private void HandleEscape() {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			Hover.Instance.Deactivate ();
		}
	}

	// chap 7
	public void StartWave(int monsterIndex) {
		int cost = 0;
		switch (monsterIndex) {
		case 0:
			cost = 10;
			break;
		case 1:
			//cost = 30;
			// golf
			cost = 15;
			break;
		case 2:
			//cost = 25;
			// tennis
			cost = 12;
			break;
		case 3:
			//cost = 15;
			// basketball
			cost = 10;
			break;
		case 4:
			//cost = 30;
			// volleyball
			cost = 15;
			break;
		case 5:
			//cost = 50;
			cost = 30;
			break;
		}
		if (player.Energy_Athlete.CurrentVal >= cost) {
			StartCoroutine (SpawnWave (monsterIndex));
			player.Energy_Athlete.CurrentVal -= cost;
		}
	}

	private IEnumerator SpawnWave(int monsterIndex) {
		//Generates the path
		LevelManager.Instance.GeneratePath();

		//int monsterIndex = 4;//Random.Range (0, 4);
		string type = string.Empty;

		switch (monsterIndex) {
		case 0:
			type = "student_red_player_walk_right_1";
			break;
		case 1:
			type = "GolfWalkRight";
			break;
		case 2:
			type = "TennisWalkRight";
			break;
		case 3:
			type = "BasketballWalkRight";
			break;
		case 4:
			type = "VolleyballWalkRight";
			break;
		case 5:
			type = "FootballWalkRight";
			break;
		}

		Monster monster = Pool.GetObject(type).GetComponent<Monster>();
		monster.Spawn (health);

		yield return new WaitForSeconds (2.5f);
	}
		
	private IEnumerator ChargeEnergy_Athlete(float increaseAmount, float cooldown) {
		// player.Energy_Tower.CurrentVal += increaseAmount;
		player.Energy_Athlete.CurrentVal += increaseAmount;
		chargeCooled_Athlete = false;
		yield return new WaitForSeconds (cooldown);
		chargeCooled_Athlete = true;
	}

	private IEnumerator ChargeEnergy_Tower(float increaseAmount, float cooldown) {
		// player.Energy_Athlete.CurrentVal += increaseAmount;
		player.Energy_Tower.CurrentVal += increaseAmount;
		chargeCooled_Tower = false;
		yield return new WaitForSeconds (cooldown);
		chargeCooled_Tower = true;
	}

	/// Removes a monster from the game
	public void RemoveMonster(Monster monster)
	{
		//Removes the monster from the active list
		activeMonsters.Remove(monster);

		//IF we don't have more active monsters and the game isn't over, then we need to show the wave button
		if (!WaveActive && !gameOver)
		{
			//Shows the wave button
			//waveBtn.SetActive(true);
		}
	}

	public void GameOver()
	{
		if (!gameOver)
		{
			gameOver = true;
			//gameOverMenu.SetActive(true);
		}
	}

	public void Restart(){
		//When WinMenu or GameoverMenu pop out, we set timeScale = 0 because we want to pause the whole game
		//So when restart, we have to set it back to 1.0
		Time.timeScale = 1.0F;
		SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
	}




	private IEnumerator EnemyWave() {
		// Controll the way
		startEnemyWave = false;
		enemyCount += 1;

		//Generates the path
		LevelManager.Instance.GenerateEnemyPath();

		// get level
		int level = 1;
		// first three enemies should be students
		if (enemyCount >= 4) { 
			if (btnLevel == "btnLevel1")
				level = 1;
			else if (btnLevel == "btnLevel2")
				level = 2;
			else if (btnLevel == "btnLevel3")
				level = 3;
			else if (btnLevel == "btnLevel4" || btnLevel == "btnLevel5")
				level = 4;
			else if (btnLevel == "btnLevel6" || btnLevel == "btnLevel7")
				level = 5;
			else
				level = 6;
		}

		int monsterIndex = UnityEngine.Random.Range (0, level);
		string type = string.Empty;

		switch (monsterIndex) {
		case 0:
			type = "student_blue_player_walk_left_1";
			break;
		case 1:
			type = "EnemyBasketballWalkLeft";
			break;
		case 2:
			type = "EnemyGolfWalkLeft";
			break;
		case 3:
			type = "EnemyVolleyballWalkLeft";
			break;
		case 4:
			type = "EnemyTennisWalkLeft";
			break;
		case 5:
			type = "EnemyFootballWalkLeft";
			break;
		}

		Enemy enemy = Pool.GetObject(type).GetComponent<Enemy>();
		enemy.SpawnEnemy (enemyHealth);

		// increase enemy's health every 3 waves
//		if (wave % 3 == 0) {
//			enemyHealth += 5;
//		}

		if (enemyCount % enemyThredhold == 0) {
			yield return new WaitForSeconds (6.5f);
		} else {
			yield return new WaitForSeconds (1.5f);
		}
		startEnemyWave = true;
	}

	/// Removes a monster from the game
	public void RemoveEnemy(Enemy monster)
	{
		//Removes the monster from the active list
		activeEnemies.Remove(monster);

		//IF we don't have more active monsters and the game isn't over, then we need to show the wave button
		if (!WaveActive && !gameOver)
		{
			//Shows the wave button
			//waveBtn.SetActive(true);
		}
	}

	public IEnumerator USCTakeDamage() {
		animatorUSC.SetBool ("Attacked", true);
		underattack_usc = true;
		player.Health_USC.CurrentVal -= DamagingUSC.Count;
		// Debug.Log ("USC Portal is under Attacked!! health:" + player.Health_USC.CurrentVal);
		yield return new WaitForSeconds (0.5f);
		underattack_usc = false;
		animatorUSC.SetBool ("Attacked", false);
	}

	public IEnumerator UCLATakeDamage() {
		animatorUCLA.SetBool ("Attacked", true);
		underattack_ucla = true;

		for (int i = 0; i < DamagingUCLA.Count; i++) {
			Debug.Log ("++++++" + DamagingUCLA [i].name + "+++++" + DamagingUCLA.Count);
			if (DamagingUCLA[i].name != "FootballWalkRight" || DamagingUCLA[i].name != "VolleyballWalkRight" || DamagingUCLA[i].name != "BasketballWalkRight") {
				player.Health_UCLA.CurrentVal -= DamagingUCLA[i].Damage / 10;
			}
		}
		// player.Health_UCLA.CurrentVal -= 2 * DamagingUCLA.Count;
		// Debug.Log ("UCLA Portal is under Attacked!! health:" + player.Health_UCLA.CurrentVal);
		yield return new WaitForSeconds (0.5f);
		underattack_ucla = false;
		animatorUCLA.SetBool ("Attacked", false);
	}

	public void ShowStats() {
		statsPanel.SetActive (!statsPanel.activeSelf);
	}

	public void SetTooltipText(string txt) {
		statText.font = pixelFont;
		statText.text = txt;
	}

	public void ShowInfo(string type) {
		string tooltip = string.Empty;

		switch (type) {
			case "Students":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>Students</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n<i>{3}</i>", "Low", "Low", "Low", "Lowest level short-range attackers\nwho attack with cheer sticks");
			tooltip = string.Format ("<color=#ffa500ff><size=20><b>Students</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n", "10", "100", "(short) 20");
				break;
			case "Golf Players":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>Golf Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n<i>{3}</i>", "Low", "Low", "Middle", "Short-range attackers using golf\nrackets as weapons");
			tooltip = string.Format ("<color=#ffa500ff><size=20><b>Golf Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n", "30", "300", "(short) 10");
				break;
			case "Tennis Players":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>Tennis Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n<i>{3}</i>", "Middle", "Middle", "Middle", "Short-range attackers using tennis\nrackets as weapons");
			tooltip = string.Format ("<color=#ffa500ff><size=20><b>Tennis Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n", "25", "150", "(short) 30");
				break;
			case "Basketball Players":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>Basketball Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n<i>{3}</i>", "Low", "Low", "Low", "Lowest level long-range attackers");
				tooltip = string.Format ("<color=#ffa500ff><size=20><b>Basketball Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n", "15", "100", "(long) 20");
				break;
			case "Volleyball Players":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>Volleyball Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n<i>{3}</i>",  "Middle", "Middle", "Middle", "Long-range attackers who will fight\nback when being attacked");
				tooltip = string.Format ("<color=#ffa500ff><size=20><b>Volleyball Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n",  "30", "150", "(long) 30");
				break;
			case "Football Players":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>Football Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n<i>{3}</i>",  "High", "High", "High", "Long-range attackers with highest\nHP and STR");
			tooltip = string.Format ("<color=#ffa500ff><size=20><b>Football Players</b></size></color>\n<b>Cost:</b> {0} \n<b>HP:</b> {1} \n<b>Strength:</b> {2} \n",  "50", "200", "(long) 50");
				break;
			case "Cheerleader":
				// tooltip = string.Format ("<color=#ffa500ff><size=20><b>USC Cheerleader</b></size></color>\n<b>Cost:</b> {0}\n<i>{1}</i>", "High", "Increase the movement speed of\nUSC athletes in a specific range\nfor 5 sec");
				tooltip = string.Format ("<color=#ffa500ff><size=20><b>USC Cheerleader</b></size></color>\n<b>Cost:</b> {0}\n<i>{1}</i>", "47", "Increase the speed of\nUSC athletes for 5 sec\n");
				break;
			case "MarchingBand":
			tooltip = string.Format ("<color=#ffa500ff><size=20><b>USC Marching Band</b></size></color>\n<b>Cost:</b> {0}\n<i>{1}</i>", "47", "Increase HP of USC athletes\nin a specific range\n");
				break;
			case "Killer":
			tooltip = string.Format ("<color=#ffa500ff><size=20><b>USC Fight On!</b></size></color>\nSpecial Attack\nLet's cheer for USC together\nTap to kill UCLA's althetes\n");
				break;
		}

		GameManager.Instance.SetTooltipText (tooltip);
		GameManager.Instance.ShowStats ();
	}

	// Trigger by Killer btn
	public void SpecialAttack() {
		if (!startSpecialAttack) {
			startSpecialAttack = true;
			killerBtn.transform.Find ("Cooldown").gameObject.GetComponent<Image> ().fillAmount = 0;
			killerBtn.GetComponent<Image> ().sprite = activeBtnSprite;
			StartCoroutine (ActivateTapAttack ());
			musicManager.bgMusic.Pause ();
			musicManager.attackMusic.Play ();
		}
	}

	private IEnumerator ActivateTapAttack() {
		enableTapAttack = true;
		yield return new WaitForSeconds (5f);
		enableTapAttack = false;
		cooldownStart = 0;
		startSpecialAttack = false;
		killerBtn.transform.Find ("Cooldown").gameObject.GetComponent<Image> ().fillAmount = 1;
		killerBtn.GetComponent<Image> ().sprite = inactiveBtnSprite;
		musicManager.bgMusic.UnPause ();
		musicManager.attackMusic.Stop ();
	}

	private void TapAttack() {
		int i = 0;
		while (i < Input.touchCount) {

			if (Input.GetTouch(i).phase == TouchPhase.Began)
			{
				//Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(i).position);
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.GetTouch(i).position), -Vector2.up);

				Debug.Log("TAP !");

				if (hit.collider != null) {

					if(hit.collider.tag=="UCLAAthlete")
					{
						Debug.Log("HIT BOMB! ");
						hit.collider.GetComponent<Enemy>().Health.CurrentVal -= 20;
					}

				}
			}
			++i;
		}
	}

	private IEnumerator UpdateCooldown() {
			shouldUpdateCooldown = false;
			// Debug.Log ("update cooldown " + cooldownStart);
			if (!killerBtn.interactable && !startSpecialAttack) {
				// Debug.Log ("fillAmountttttttt" + killerBtn.transform.Find("Cooldown").gameObject.GetComponent<Image>().fillAmount);
				killerBtn.transform.Find("Cooldown").gameObject.GetComponent<Image>().fillAmount -= 1 / (float)cooldownTime;
			}
			if (cooldownStart >= cooldownTime) {
				killerBtn.interactable = true;
			}
			else {
				killerBtn.interactable = false;
			}
			cooldownStart += 1;
			yield return new WaitForSeconds (1.0f);
			shouldUpdateCooldown = true;
	}
		
}
