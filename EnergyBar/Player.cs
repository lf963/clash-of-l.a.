using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour {

	[SerializeField]
	private Button addAthleteEnergyBtn;

	[SerializeField]
	private Stat energy_Athlete;

	public Stat Energy_Athlete {
		get {
			return energy_Athlete;
		}
		set {
			this.energy_Athlete.CurrentVal = value.CurrentVal;
		}
	}

	[SerializeField]
	private Stat energy_Tower;

	public Stat Energy_Tower {
		get {
			return energy_Tower;
		}
		set {
			this.energy_Tower.CurrentVal = value.CurrentVal;
		}
	}

	[SerializeField]
	private Stat health_USC;

	public Stat Health_USC {
		get {
			return health_USC;
		}
		set {
			this.health_USC.CurrentVal = value.CurrentVal;
		}
	}

	[SerializeField]
	private Stat health_UCLA;

	public Stat Health_UCLA {
		get {
			return health_UCLA;
		}
		set {
			this.health_UCLA.CurrentVal = value.CurrentVal;
		}
	}

	private void Awake() {
		energy_Athlete.Initialize (LevelManager.myPlayer.energy_Athlete_MAX);
		energy_Tower.Initialize (LevelManager.myPlayer.energy_Tower_MAX);
		health_USC.Initialize (LevelManager.myPlayer.health_USC_MAX);
		health_UCLA.Initialize (LevelManager.myPlayer.health_UCLA_MAX);
	}
	
	// Update is called once per frame
	void Update () {
		// health.CurrentVal += 1;
		// energy.CurrentVal += 0.5f;
		if (Input.GetKeyDown (KeyCode.Z)) {
			energy_Athlete.CurrentVal -= 10;
		}
		if (Input.GetKeyDown (KeyCode.X)) {
			energy_Athlete.CurrentVal += 10;
		}
		if (Input.GetKeyDown (KeyCode.C)) {
			energy_Tower.CurrentVal -= 10;
		}
		if (Input.GetKeyDown (KeyCode.V)) {
			energy_Tower.CurrentVal += 10;
		}

		if (Input.GetKeyDown (KeyCode.G)) {
			health_USC.CurrentVal -= 10;
		}
		if (Input.GetKeyDown (KeyCode.H)) {
			health_USC.CurrentVal += 10;
		}
		if (Input.GetKeyDown (KeyCode.J)) {
			health_UCLA.CurrentVal -= 10;
		}
		if (Input.GetKeyDown (KeyCode.K)) {
			health_UCLA.CurrentVal += 10;
		}


		//update max value of energy_Athlete
		if (Input.GetKeyDown (KeyCode.O)) {
			energy_Athlete.MaxVal += 10;
			LevelManager.myPlayer.energy_Athlete_MAX += 10;
		}
		//update max value of energy_Tower
		if (Input.GetKeyDown (KeyCode.P)) {
			energy_Tower.MaxVal += 10;
			LevelManager.myPlayer.energy_Tower_MAX += 10;
		}
	}

	public void addAthleteEnergy(){
		energy_Athlete.CurrentVal += 30;
	}
}
